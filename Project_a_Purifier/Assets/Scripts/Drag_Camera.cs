using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag_Camera : MonoBehaviour
{
    //카메라 이동 범위
    private float xMin = -5.5f;
    private float xMax = 5.5f;

    public float speed = 3.0f;

    private Vector3 cameraMoveDir;                              //카메라 이동방향
    private Vector2 prevPos = Vector2.zero;


    private Vector3 ClampMove(Vector3 _cameraMoveDir)
    {
        Vector3 curPos = transform.position + _cameraMoveDir;

        float x = Mathf.Clamp(curPos.x, xMin, xMax);
        return new Vector3(x, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        if(Input.touchCount > 0)
            DragMovement();
        else
            prevPos = Vector2.zero;
    }

    /// <summary>
    /// 드래그로 카메라 이동
    /// </summary>
    private void DragMovement()
    {
        if(prevPos == Vector2.zero)
        {
            prevPos = Input.GetTouch(0).position;
            return;
        }

        Vector2 dir = (Input.GetTouch(0).position - prevPos).normalized;

        if (dir.x < 0.0f)
        {
            cameraMoveDir = Vector3.right * speed * Time.deltaTime;
            transform.position = ClampMove(cameraMoveDir);
        }
        else if(dir.x > 0.0f)
        {
            cameraMoveDir = Vector3.left * speed * Time.deltaTime;
            transform.position = ClampMove(cameraMoveDir);
        }
        else
        {
            prevPos = Input.GetTouch(0).position;       
        }
    }
}
