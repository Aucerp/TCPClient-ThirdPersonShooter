using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;


public static class NetManagerC3 {
    //定義套接字
    static Socket socket;
    //接收緩衝區
    static byte[] readBuff = new byte[1024];
    //委託類型
    public delegate void MsgListener(string str);
    //監聽列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //消息列表
    static List<String> msgList = new List<String>();

    //添加監聽
    public static void AddListener(string msgName, MsgListener listener) {
        listeners[msgName] = listener;
    }

    //獲取描述
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    //連接
    public static void Connect(string ip, int port)
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.Connect(ip, port);
        //BeginReceive
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
    }

    private static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }catch(Exception ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //發送
    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //更新
    public static void Update()
    {
        if (msgList.Count <= 0) return;
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        //監聽回調
        if (listeners[msgName] != null)
        {
            listeners[msgName](msgArgs);
        }
    }   
}
