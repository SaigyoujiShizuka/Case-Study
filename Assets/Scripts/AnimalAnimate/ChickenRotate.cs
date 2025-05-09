using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenRotate : MonoBehaviour
{

    [SerializeField] private float rotateSpeed = 180f; // �ٶ�
    public Vector3 rotationAxis = Vector3.up; // ��ת�ᣨĬ��ΪY�ᣩ
    public bool is_pause = false; // ��ͣ��־
    void Start()
    {
        
    }
   

    void Update()
    {
        // �����ͣ���򲻸�����ת
        if (is_pause)
        {
            return; // �����ͣ���򲻸�����ת
        }
        // ��ȡ����ĵ�ǰ���ĵ�
        Vector3 pivotPoint = transform.position;

        // Χ�����ĵ��ָ������ת
        transform.RotateAround(pivotPoint, rotationAxis, rotateSpeed * Time.deltaTime);
    }
}
