using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    //Prefab
    public GameObject humanPrefab;
    //人物列表
    private BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();
    // Start is called before the first frame update

    private void Awake()
    {
        NetManagerC3.AddListener("Enter", OnEnter);
        NetManagerC3.AddListener("List", OnList);
        NetManagerC3.AddListener("Move", OnMove);
        NetManagerC3.AddListener("Leave", OnLeave);
        NetManagerC3.AddListener("Attack", OnAttack);
        NetManagerC3.AddListener("Die", OnDie);
        NetManagerC3.Connect("127.0.0.1", 8888);

        //添加一個角色
        GameObject obj = Instantiate(humanPrefab) as GameObject;
        float x = UnityEngine.Random.Range(-5, 5);
        float z = UnityEngine.Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManagerC3.GetDesc();

        //發送協議
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManagerC3.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y + ",";
        NetManagerC3.Send(sendStr);
    }
    void Start()
    {
        //請求玩家列表
        NetManagerC3.Send("List|");
    }
    private void Update()
    {
        NetManagerC3.Update();
    }

    private void OnEnter(string msgArgs)
    {
        Debug.Log("OnEnter " + msgArgs);
        //解析參數
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        //是自己
        if (desc == NetManagerC3.GetDesc())
            return;
        //添加角色
        GameObject obj = Instantiate(humanPrefab) as GameObject;
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, eulY, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
    }

    private void OnList(string msgArgs)
    {
        Debug.Log("OnList " + msgArgs);
        //解析參數
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 6;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            //是自己
            if (desc == NetManagerC3.GetDesc())
                continue;
            //添加角色
            GameObject obj = Instantiate(humanPrefab) as GameObject;
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    void OnMove(string msgArgs)
    {
        Debug.Log("OnMove " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        //移动
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        h.MoveTo(targetPos);
    }

    void OnLeave(string msgArgs)
    {
        Debug.Log("OnLeave " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        //删除
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);
        otherHumans.Remove(desc);
    }

    void OnAttack(string msgArgs)
    {
        Debug.Log("OnAttack " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        //攻击动作
        if (!otherHumans.ContainsKey(desc))
            return;
        SyncHuman h = (SyncHuman)otherHumans[desc];
        h.SyncAttack(eulY);
    }

    void OnDie(string msgArgs)
    {
        Debug.Log("OnDie " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string attDesc = split[0];
        string hitDesc = split[0];

        //自己死了
        if (hitDesc == myHuman.desc)
        {
            Debug.Log("Game Over");
            myHuman.gameObject.SetActive(false);
            return;
        }
        //死了
        if (!otherHumans.ContainsKey(hitDesc))
            return;
        Debug.Log("Enemy" + hitDesc + " Die!");
        SyncHuman h = (SyncHuman)otherHumans[hitDesc];
        h.gameObject.SetActive(false);
    }

}
