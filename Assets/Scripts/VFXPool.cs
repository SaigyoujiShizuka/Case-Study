using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPool : MonoBehaviour
{
    public static VFXPool Instance;

    [SerializeField]
    private ParticleSystem _particles;
    private int initialCount = 5; // ��ʼ����
    private Queue<ParticleSystem> _pool = new Queue<ParticleSystem>();

    void Awake()
    {
        Instance = this; 
        for (int i = 0; i < initialCount; i++)
        {
            ParticleSystem ps = Instantiate(_particles);
            //����scaleΪ20
            ps.transform.localScale = new Vector3(15, 15, 15);
            ps.gameObject.SetActive(false);
            _pool.Enqueue(ps);
        }

    }
    /// <summary>
    /// �Ӷ�����л�ȡһ��ps
    /// </summary>
    /// <param name="position"></param>
    /// <returns>�趨Ϊ��ǰλ�õ�һ��ps</returns>
    public ParticleSystem GetFromPool(Vector3 position)
    {
        if (_pool.Count == 0)
        {
            ExpandPool(2); // �ؿ�ʱ�Զ�����
        }

        ParticleSystem ps = _pool.Dequeue();
        ps.transform.position = position;
        ps.gameObject.SetActive(true);
        ps.Play();
        return ps;
    }
    /// <summary>
    /// ����һ��ps
    /// </summary>
    /// <param name="ps"></param>
    public void ReturnToPool(ParticleSystem ps)
    {
        StartCoroutine(DelayedReturn(ps)); // ��ΪЭ���ӳٻ���
    }
    //�ӳٻ��գ�private
    private IEnumerator DelayedReturn(ParticleSystem ps)
    {
        // �ȴ�������ȫ�������
        yield return new WaitWhile(() => ps.isPlaying);

        ps.Stop();
        ps.gameObject.SetActive(false);
        _pool.Enqueue(ps);
    }
    //��������
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
