using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    // 同步加载接口
    public GameObject LoadAndInstantiate(string path, Transform parent = null)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"资源加载失败：{path}");
            return null;
        }

        return InstantiatePrefab(prefab, parent);
    }

    // 异步加载接口
    public IEnumerator LoadAndInstantiateAsync(string path, System.Action<GameObject> callback, Transform parent = null)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        yield return request;

        if (request.asset == null)
        {
            Debug.LogError($"异步加载失败：{path}");
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

    // 创建默认父对象
    public Transform CreateDefaultParent(Transform root, string parentName = "DynamicObjects")
    {
        GameObject parentObj = GameObject.Find(parentName) ?? new GameObject(parentName);
        parentObj.transform.SetParent(root);
        return parentObj.transform;
    }
    //回收
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
