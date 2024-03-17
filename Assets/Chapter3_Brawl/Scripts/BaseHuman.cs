using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    //是否正在移動
    protected bool isMoving = false;
    //移動目標點
    private Vector3 targetPosition;
    //移動速度
    private float speed = 1.2f;
    //動畫組件
    private Animator animator;
    //描述
    public string desc = "";
    //移動到某處
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    //移動 update
    private void MoveUpdate()
    {
        if (isMoving == false)
            return;

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }

    //Initailization
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    //Update is call once per frame
    protected void Update()
    {
        MoveUpdate();
    }
}
