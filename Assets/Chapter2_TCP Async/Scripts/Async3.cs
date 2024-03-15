using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;

namespace Aucer
{
    public class Async3 : MonoBehaviour//狀態檢測Poll - 聊天室 Client
    {
        //定義套接字
        Socket socket;

        //UGUI
        [SerializeField] private InputField InputField;
        [SerializeField] private Text text;
        [SerializeField] private Button connButton;
        [SerializeField] private Button sendButton;

        //接收緩衝區
        byte[] readBuff = new byte[1024];
        string recvStr = "";

        private void Start()
        {
            // Set up button click events
            connButton.onClick.AddListener(Connection);
            sendButton.onClick.AddListener(Send);
            
        }

        //點擊連接按鈕
        public void Connection() 
        {
            //Socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Connect(異步)
            socket.BeginConnect("127.0.0.1", 8888, ConnectCallBack, socket);
        }

        //Connect 回調
        private void ConnectCallBack(IAsyncResult ar)
        {
            try {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                Debug.Log("Socket Connect succ");
                socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
            }
            catch(SocketException ex) {
                Debug.Log("Socket Connect fail "+ ex.ToString());
            }
        }

        //Receive 回調
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                string s = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                recvStr = s + "\n" + recvStr;
                socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Receive fail " + ex.ToString());
            }
        }

        //點擊發送按鈕
        public void Send() 
        {
            //Send
            string sendStr = InputField.text;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }

        //Send 回調
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndSend(ar);
                Debug.Log("Socket Send succ " + count);
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Send fail " + ex.ToString());
            }
        }

        //UI更新 只能在MainThread主線程
        private void Update()
        {
            if (socket == null) {
                return;
            }

            if (socket.Poll(0, SelectMode.SelectRead))
            {
                byte[] readBuff = new byte[1024];
                int count = socket.Receive(readBuff);
                string recvStr =
                    System.Text.Encoding.UTF8.GetString(readBuff, 0 , count);
                text.text = recvStr;
            }
        }
    }
}
