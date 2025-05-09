using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenMove : MonoBehaviour
{
    private Vector3 initialPosition; // ��¼��ʼλ��
    public bool is_pause = false; // ��ͣ��־

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

    }
    [SerializeField, Header("�����˶�����")]
    public float amplitudeX = 5f; // �������
    public float amplitudeY = 3f; // �������
    public float speed = 2f;      // �˶��ٶ�

    private float time = 0f;      // ʱ�����

    void Update()
    {
        // ����ʱ�����
        // ����ʱ�����
        if (is_pause)
        {
            return; // �����ͣ���򲻸���λ��
        }
        time += Time.deltaTime * speed;

        // ������ڳ�ʼλ�õĺ��������ƫ��
        float xOffset = amplitudeX * Mathf.Sin(time);
        float yOffset = amplitudeY * Mathf.Sin(2 * time);

        // ���������λ��
        transform.position = initialPosition + new Vector3(xOffset, yOffset, 0);
    }
}
