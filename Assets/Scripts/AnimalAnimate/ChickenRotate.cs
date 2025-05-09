using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenRotate : MonoBehaviour
{

    [SerializeField] private float rotateSpeed = 180f; // 速度
    public Vector3 rotationAxis = Vector3.up; // 旋转轴（默认为Y轴）
    public bool is_pause = false; // 暂停标志
    void Start()
    {
        
    }
   

    void Update()
    {
        // 如果暂停，则不更新旋转
        if (is_pause)
        {
            return; // 如果暂停，则不更新旋转
        }
        // 获取物体的当前中心点
        Vector3 pivotPoint = transform.position;

        // 围绕中心点和指定轴旋转
        transform.RotateAround(pivotPoint, rotationAxis, rotateSpeed * Time.deltaTime);
    }
}
