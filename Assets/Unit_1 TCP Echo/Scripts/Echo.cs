using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace Aucer
{
    public class Echo : MonoBehaviour//全阻塞，同步Socket
    {
        //定義套接字
        Socket socket;

        //UGUI
        [SerializeField] private InputField InputField;
        [SerializeField] private Text text;
        [SerializeField] private Button connButton;
        [SerializeField] private Button sendButton;

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
            socket.Connect("127.0.0.1", 8888);
        }

        //點擊發送按鈕
        public void Send() 
        {
            Debug.Log("Send");
            //Send
            string sendStr = InputField.text;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            socket.Send(sendBytes);
            //Recv
            byte[] readBuff = new byte[1024];
            int count = socket.Receive(readBuff);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0 , count);
            text.text = recvStr;
            //Close
            socket.Close();
        }
    }
}
