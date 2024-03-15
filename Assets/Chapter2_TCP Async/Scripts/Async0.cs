using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;

namespace Aucer
{
    public class Async0 : MonoBehaviour//異步Connect, 同步Send
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
            Debug.Log("Connection");
            //Socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Connect
            socket.BeginConnect("127.0.0.1", 8888, ConnectCallBack, socket);
        }

        //Connect 回調
        private void ConnectCallBack(IAsyncResult ar)
        {
            try {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                Debug.Log("Socket Connect Succ");
                socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
            }
            catch(SocketException ex) {
                Debug.Log("Socket Connect fail "+ ex.ToString());
            }
        }

        //Receive 回調
        private void ReceiveCallBack(IAsyncResult ar)
        {
            Debug.Log("Receive");
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
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
            Debug.Log("Send");
            //Send
            string sendStr = InputField.text;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            socket.Send(sendBytes);
        }

        //UI更新 只能在MainThread主線程
        private void Update()
        {
            text.text = recvStr;
        }
    }
}
