using UnityEngine;
using System.Threading;

public class Async_Timer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("設定鈴聲");
        System.Threading.Timer timer = new System.Threading.Timer(TimeOut, null, 5000, 0);
    }
    private void TimeOut(System.Object state)
    {
        Debug.Log("鈴鈴鈴");
    }
}
