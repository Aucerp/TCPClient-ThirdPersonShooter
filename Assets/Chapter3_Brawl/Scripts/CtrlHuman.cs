using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CtrlHuman : BaseHuman
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        //Move
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Terrian")
            {
                MoveTo(hit.point);
                //發送協議
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += hit.point.x.ToString() + ",";
                sendStr += hit.point.y.ToString() + ",";
                sendStr += hit.point.z.ToString();
                NetManager.Send(sendStr);
            }
        }

        //Attack
        if (Input.GetMouseButtonDown(1))
        {
            if (isAttacking) return;
            if (isMoving) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(targetPosition);
            Attack();
            //發送協議
            string sendStr = "Attack|";
            sendStr += NetManager.GetDesc() + ",";
            sendStr += transform.eulerAngles.y + ",";
            NetManager.Send(sendStr);

            //攻擊判定
            float characterHeight = 1f;
            float characterWidth = .6f;
            float shootRange = 12f;
            Vector3 shooterPos = transform.position + Vector3.up * (characterHeight / 2);
            Vector3 shootPos = shooterPos + transform.forward * characterWidth;
            Vector3 targetPos = shooterPos + transform.forward * shootRange;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(shootPos, mousePos, Color.red, 0.5f);

            if (Physics.Linecast(shootPos, targetPos, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj == gameObject)
                    return;
                SyncHuman h = hitObj.GetComponent<SyncHuman>();
                if (h == null)
                    return;
                sendStr = "Hit|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += h.desc + ",";
                NetManager.Send(sendStr);
            }
        }
    }
}
