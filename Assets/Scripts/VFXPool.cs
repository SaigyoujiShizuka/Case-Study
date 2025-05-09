using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPool : MonoBehaviour
{
    public static VFXPool Instance;

    [SerializeField]
    private ParticleSystem _particles;
    private int initialCount = 5; // 初始数量
    private Queue<ParticleSystem> _pool = new Queue<ParticleSystem>();

    void Awake()
    {
        Instance = this; 
        for (int i = 0; i < initialCount; i++)
        {
            ParticleSystem ps = Instantiate(_particles);
            //设置scale为20
            ps.transform.localScale = new Vector3(15, 15, 15);
            ps.gameObject.SetActive(false);
            _pool.Enqueue(ps);
        }

    }
    /// <summary>
    /// 从对象池中获取一个ps
    /// </summary>
    /// <param name="position"></param>
    /// <returns>设定为当前位置的一个ps</returns>
    public ParticleSystem GetFromPool(Vector3 position)
    {
        if (_pool.Count == 0)
        {
            ExpandPool(2); // 池空时自动扩容
        }

        ParticleSystem ps = _pool.Dequeue();
        ps.transform.position = position;
        ps.gameObject.SetActive(true);
        ps.Play();
        return ps;
    }
    /// <summary>
    /// 回收一个ps
    /// </summary>
    /// <param name="ps"></param>
    public void ReturnToPool(ParticleSystem ps)
    {
        StartCoroutine(DelayedReturn(ps)); // 改为协程延迟回收
    }
    //延迟回收，private
    private IEnumerator DelayedReturn(ParticleSystem ps)
    {
        // 等待粒子完全播放完毕
        yield return new WaitWhile(() => ps.isPlaying);

        ps.Stop();
        ps.gameObject.SetActive(false);
        _pool.Enqueue(ps);
    }
    //扩充对象池
    private void ExpandPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            ParticleSystem ps = Instantiate(_particles);
            ps.gameObject.SetActive(false);
            _pool.Enqueue(ps);
        }
    }


}
