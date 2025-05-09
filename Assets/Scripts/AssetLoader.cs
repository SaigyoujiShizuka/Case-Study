using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    // ͬ�����ؽӿ�
    public GameObject LoadAndInstantiate(string path, Transform parent = null)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"��Դ����ʧ�ܣ�{path}");
            return null;
        }

        return InstantiatePrefab(prefab, parent);
    }

    // �첽���ؽӿ�
    public IEnumerator LoadAndInstantiateAsync(string path, System.Action<GameObject> callback, Transform parent = null)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        yield return request;

        if (request.asset == null)
        {
            Debug.LogError($"�첽����ʧ�ܣ�{path}");
            callback?.Invoke(null);
            yield break;
        }

        GameObject instance = InstantiatePrefab(request.asset as GameObject, parent);
        callback?.Invoke(instance);
    }

    private GameObject InstantiatePrefab(GameObject prefab, Transform parent)
    {
        GameObject instance = Instantiate(prefab, parent);
        instance.transform.localPosition = Vector3.zero;
        instance.SetActive(false);
        return instance;
    }

    // ����Ĭ�ϸ�����
    public Transform CreateDefaultParent(Transform root, string parentName = "DynamicObjects")
    {
        GameObject parentObj = GameObject.Find(parentName) ?? new GameObject(parentName);
        parentObj.transform.SetParent(root);
        return parentObj.transform;
    }
    //����
    public void ReleaseInstance(GameObject instance)
    {
        if (instance != null)
        {
            Destroy(instance);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
