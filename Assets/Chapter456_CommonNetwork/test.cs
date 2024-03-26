using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
    }

    //收到 MsgMove 協議
    public void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msg = (MsgMove)msgBase;
        Debug.Log("OnMsgMove " + msg.x + " " + msg.y + " " + msg.z);
    }

    private void Update()
    {
        NetManager.Update();
    }

    //玩家典籍連接按鈕
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
        //Todo: 轉圈動畫, 顯示 "連接中"
    }

    //連接成功
    private void OnConnectSucc(string msg)
    {
        Debug.Log("OnConnectSucc " + msg);
        //Todo: 關閉轉圈動畫, 彈出提示框 ( 連接成功 ) 
    }

    //連接失敗
    private void OnConnectFail(string msg)
    {
        Debug.Log("OnConnectFail " + msg);
        //Todo: 關閉轉圈動畫, 彈出提示框 ( 連接失敗, 請重試 ) 
    }

    //連接關閉
    void OnConnectClose(string err)
    {
        Debug.Log("OnConnectClose " + err);
        //Todo: 彈出提示框 ( 網路斷開 )
        //Todo: 彈出按鈕 ( 重新連線 )
    }
}
