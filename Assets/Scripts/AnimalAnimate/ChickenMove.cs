using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenMove : MonoBehaviour
{
    private Vector3 initialPosition; // 记录初始位置
    public bool is_pause = false; // 暂停标志

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

    }
    [SerializeField, Header("鸡的运动参数")]
    public float amplitudeX = 5f; // 横向振幅
    public float amplitudeY = 3f; // 纵向振幅
    public float speed = 2f;      // 运动速度

    private float time = 0f;      // 时间参数

    void Update()
    {
        // 更新时间参数
        // 更新时间参数
        if (is_pause)
        {
            return; // 如果暂停，则不更新位置
        }
        time += Time.deltaTime * speed;

        // 计算基于初始位置的横向和纵向偏移
        float xOffset = amplitudeX * Mathf.Sin(time);
        float yOffset = amplitudeY * Mathf.Sin(2 * time);

        // 更新物体的位置
        transform.position = initialPosition + new Vector3(xOffset, yOffset, 0);
    }
}
