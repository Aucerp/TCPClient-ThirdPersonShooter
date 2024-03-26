using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Linq;
public static class NetManager
{
    //定義套接字
    static Socket socket;
    //接收緩沖區
    static ByteArray readBuff;
    //寫入隊列
    static Queue<ByteArray> writeQueue;
    //是否正在連接
    static bool isConnecting = false;
    //是否正在關閉
    static bool isClosing = false;
    //消息列表
    static List<MsgBase> msgList = new List<MsgBase>();
    //消息列表長度
    static int msgCount = 0;
    //每一次Update處理的消息量
    readonly static int MAX_MESSAGE_FIRE = 10;
    //是否啟用心跳
    public static bool isUsePing = true;
    //心跳間隔時間
    public static int pingInterval = 30;
    //上一次發送PING的時間
    static float lastPingTime = 0;
    //上一次收到PONG的時間
    static float lastPongTime = 0;

    //事件
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }
    //事件委托類型
    public delegate void EventListener(String err);
    //事件監聽列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
    //添加事件監聽
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        //新增事件
        else
        {
            eventListeners[netEvent] = listener;
        }
    }
    //刪除事件監聽
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
        }
    }
    //分發事件
    private static void FireEvent(NetEvent netEvent, String err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }


    //消息委托類型
    public delegate void MsgListener(MsgBase msgBase);
    //消息監聽列表
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    //添加消息監聽
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        //添加
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        //新增
        else
        {
            msgListeners[msgName] = listener;
        }
    }
    //刪除消息監聽
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
        }
    }
    //分發消息
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }


    //連接
    public static void Connect(string ip, int port)
    {
        //狀態判斷
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("Connect fail, isConnecting");
            return;
        }
        //初始化成員
        InitState();
        //參數設置
        socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    //初始化狀態
    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //接收緩沖區
        readBuff = new ByteArray();
        //寫入隊列
        writeQueue = new Queue<ByteArray>();
        //是否正在連接
        isConnecting = false;
        //是否正在關閉
        isClosing = false;
        //消息列表
        msgList = new List<MsgBase>();
        //消息列表長度
        msgCount = 0;
        //上一次發送PING的時間
        lastPingTime = Time.time;
        //上一次收到PONG的時間
        lastPongTime = Time.time;
        //監聽PONG協議
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }

    //Connect回調
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            //開始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                                            readBuff.remain, 0, ReceiveCallback, socket);

        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail " + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }


    //關閉連接
    public static void Close()
    {
        //狀態判斷
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        //還有數據在發送
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        //沒有數據在發送
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    //發送數據
    public static void Send(MsgBase msg)
    {
        //狀態判斷
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        if (isClosing)
        {
            return;
        }
        //數據編碼
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        //組裝長度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //組裝名字
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        //組裝消息體
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        //寫入隊列
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;  //writeQueue的長度
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        //send
        if (count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length,
                0, SendCallback, socket);
        }
    }

    //Send回調
    public static void SendCallback(IAsyncResult ar)
    {

        //獲取state、EndSend的處理
        Socket socket = (Socket)ar.AsyncState;
        //狀態判斷
        if (socket == null || !socket.Connected)
        {
            return;
        }
        //EndSend
        int count = socket.EndSend(ar);
        //獲取寫入隊列第一條數據            
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        //完整發送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }
        //繼續發送
        if (ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length,
                0, SendCallback, socket);
        }
        //正在關閉
        else if (isClosing)
        {
            socket.Close();
        }
    }



    //Receive回調
    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //獲取接收數據長度
            int count = socket.EndReceive(ar);
            readBuff.writeIdx += count;
            //處理二進制消息
            OnReceiveData();
            //繼續接收數據
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                    readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //數據處理
    public static void OnReceiveData()
    {
        //消息長度
        if (readBuff.length <= 2)
        {
            return;
        }
        //獲取消息體長度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength)
            return;
        readBuff.readIdx += 2;
        //解析協議名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
        {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;
        //解析協議體
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //添加到消息隊列
        lock (msgList)
        {
            msgList.Add(msgBase);
            msgCount++;
        }
        //繼續讀取消息
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }

    //Update
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    //更新消息
    public static void MsgUpdate()
    {
        //初步判斷，提升效率
        if (msgCount == 0)
        {
            return;
        }
        //重覆處理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //獲取第一條消息
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分發消息
            if (msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
            }
            //沒有消息了
            else
            {
                break;
            }
        }
    }

    //發送PING協議
    private static void PingUpdate()
    {
        //是否啟用
        if (!isUsePing)
        {
            return;
        }
        //發送PING
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        //檢測PONG時間
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }

    //監聽PONG協議
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}
