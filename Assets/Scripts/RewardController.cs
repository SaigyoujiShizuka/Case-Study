using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class RewardController : MonoBehaviour
{

    //单例
    public static RewardController Instance { get; private set; }
    [Header("资源加载")]
    [SerializeField]
    private string[] _prefabPaths = new string[]
    {

        "Prefabs/Cat",
        "Prefabs/Chiken",
        "Prefabs/Cow"
    };


    private GameObject[] _childPrefabs; // 存储子物体的数组
    private AssetLoader _assetLoader; // 资源加载器
    private bool _isPaused = false; // 标记是否暂停
    private GameObject animalsObject;
    private Coroutine _vfxCoroutine; // 当前运行的协程
    private GameObject _currentActivePrefab; // 当前激活的预制体

    // Start is called before the first frame update
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        _assetLoader = gameObject.AddComponent<AssetLoader>(); // 添加资源加载器组件
        InitializePrefabs(); // 初始化预制体  

    }
    // 初始化预制体
    public void InitializePrefabs()
    {
        Transform parent = _assetLoader.CreateDefaultParent(transform, "Animals");
        animalsObject= parent.gameObject; // 将父物体赋值给 animalsObject
        _childPrefabs = new GameObject[_prefabPaths.Length];

        for (int i = 0; i < _prefabPaths.Length; i++)
        {
            StartCoroutine(LoadPrefabAsync(i, parent));
        }
    }
    // 异步加载预制体并实例化
    private IEnumerator LoadPrefabAsync(int index, Transform parent)
    {
        yield return _assetLoader.LoadAndInstantiateAsync(
            _prefabPaths[index],
            instance => _childPrefabs[index] = instance,
            parent
        );

        if (_childPrefabs[index] == null)
        {
            Debug.LogError($"预制体初始化失败：{_prefabPaths[index]}");
        }
    }
    /*    // 加载资源并将其添加到子物体的方法
        private GameObject LoadAndAttachPrefab(string path)
        {
            // 加载预制体
            GameObject loadedPrefab = Resources.Load<GameObject>(path);

            if (loadedPrefab != null)
            {
                // 检查 Animals 对象是否存在，如果不存在则创建

                if (animalsObject == null)
                {
                    GameObject animalsObject = new GameObject("Animals");
                    animalsObject.transform.SetParent(transform); // 设置为当前对象的子物体
                    this.animalsObject = animalsObject; 
                }
                // 实例化预制体并设置为 Animals 的子物体
                GameObject instance = Instantiate(loadedPrefab, animalsObject.transform);
                instance.transform.localPosition = Vector3.zero; // 设置相对位置
                instance.SetActive(false); // 设置为不可见
                return instance;
            }
            else
            {
                Debug.LogError($"无法加载预制体：{path}");
                return null;
            }
        }
    */
    // Update is called once per frame
    private void Update()
    {
        // 按键 1、2、3 显示对应的子物体
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowPrefab(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowPrefab(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowPrefab(2);
        }

        // 按下空格键暂停或恢复
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }

        // 按下 Z 键停止所有动画并隐藏所有子物体
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StopAllAnimationsAndHide();
        }
    }
    // 播放 VFX 的协程
    private IEnumerator ContinuousVFXRoutine(float duration, float interval)
    {
        Transform spawnPoint = _currentActivePrefab.transform.Find("VFX_SpawnPoint");
        if (spawnPoint == null) yield break;

        while (_currentActivePrefab.activeSelf && !_isPaused)
        {
            ParticleSystem vfx = VFXPool.Instance.GetFromPool(spawnPoint.position);
            StartCoroutine(ReleaseVFXAfterDuration(vfx, duration)); // 假设粒子持续1秒

            // 粒子频率
            yield return new WaitForSecondsRealtime(interval);
        }
    }
    private IEnumerator ReleaseVFXAfterDuration(ParticleSystem vfx, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        VFXPool.Instance.ReturnToPool(vfx);
    }
    /*    private IEnumerator PlayVFX(ParticleSystem vfx, float duration)
        {
            yield return new WaitForSeconds(duration);
            VFXPool.Instance.ReturnToPool(vfx);
        }
    */
    // 显示指定索引的子物体并初始化位置
    //外部方法
    public void ShowRewardByType(string type)
    {
        int index = type.ToLower() switch
        {
            "cat" => 0,
            "chicken" => 1,
            "cow" => 2,
            _ => -1
        };

        if (index != -1) ShowPrefab(index);
    }
    //内部方法
    private void ShowPrefab(int index)
    {
        if (index >= 0 && index < _childPrefabs.Length && _childPrefabs[index] != null)
        {
            // 停止之前的协程
            if (_vfxCoroutine != null)
            {
                StopCoroutine(_vfxCoroutine);
                _vfxCoroutine = null;
            }
            // 隐藏其他子物体
            foreach (var prefab in _childPrefabs)
            {
                if (prefab != null)
                {
                    prefab.SetActive(false);
                }
            }
            _isPaused= false; // 恢复暂停状态

            // 显示指定的子物体并初始化位置

            _currentActivePrefab = _childPrefabs[index];
            _currentActivePrefab.SetActive(true);
            _currentActivePrefab.transform.localPosition = Vector3.zero;
            // 获取 Animator 组件并启用动画
            Animator animator = _currentActivePrefab.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
            }
            if (animalsObject != null)
            {
                // 获取 animalsObject 下的脚本并设置 is_pause 变量
                ChickenRotate chickenRotate = animalsObject.GetComponent<ChickenRotate>();
                if (chickenRotate != null)
                {
                    chickenRotate.is_pause = _isPaused;
                }
                ChickenMove chickenMove = animalsObject.GetComponent<ChickenMove>();
                if (chickenMove != null)
                {
                    chickenMove.is_pause = _isPaused;
                }
            }
 


            Debug.Log($"显示第 {index + 1} 个预制体");

            // 启动粒子生成协程
            _vfxCoroutine = StartCoroutine(ContinuousVFXRoutine(1f, 0.3f));
            //启动震动协程
            StartCoroutine(RequestVibration(1000));

        }
        else
        {
            Debug.LogWarning($"索引 {index} 超出范围或子物体不存在");
        }
    }

    // 暂停或恢复所有动画
    public void TogglePause()
    {
        _isPaused = !_isPaused;


        foreach (var prefab in _childPrefabs)
        {
            if (prefab != null)
            {
                Animator animator = prefab.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = !_isPaused;
                }
            }
        }
        //将animals下挂靠的脚本的is_pause变量设置为当前暂停状态

        if (animalsObject != null)
        {
            ChickenRotate chickenRotate = animalsObject.GetComponent<ChickenRotate>();
            if (chickenRotate != null)
            {
                chickenRotate.is_pause = _isPaused;
            }
            ChickenMove chickenMove = animalsObject.GetComponent<ChickenMove>();
            if (chickenMove != null)
            {
                chickenMove.is_pause = _isPaused;
            }
        }
        // 如果恢复暂停且当前有激活的预制体，重新启动协程
        if (!_isPaused && _currentActivePrefab != null && _currentActivePrefab.activeSelf)
        {
            _vfxCoroutine = StartCoroutine(ContinuousVFXRoutine(1f, 0.3f));
        }

        Debug.Log(_isPaused ? "动画已暂停" : "动画已恢复");
    }

    // 停止所有动画并隐藏所有子物体
    public void StopAllAnimationsAndHide()
    {
        foreach (var prefab in _childPrefabs)
        {
            if (prefab != null)
            {
                // 停止动画
                Animator animator = prefab.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false;
                }

                // 隐藏子物体
                prefab.SetActive(false);
            }
        }

        Debug.Log("所有动画已停止，所有子物体已隐藏");
    }
    // 震动请求协程
    private IEnumerator RequestVibration(int durationMs)
    {
        bool hasPermission = true;

#if UNITY_ANDROID
        // Android平台权限检查
        using (var permissionClass = new AndroidJavaClass("android.content.pm.PackageManager"))
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity")) 
        {
            string packageName = activity.Call<string>("getPackageName");
            int granted = permissionClass.GetStatic<int>("PERMISSION_GRANTED");
            hasPermission = permissionClass.CallStatic<int>("checkPermission", "android.permission.VIBRATE", packageName) == granted;
        }

        if (!hasPermission)
        {
            // 发送权限请求
            NativeBridge.Instance.SendToNative("request_permission", "vibrate");
            
            // 等待权限回调（关键修复点）
            bool isCallbackReceived = false;
            PermissionManager.OnVibrationPermissionResult += (result) => {
                isCallbackReceived = true;
                if(result) Handheld.Vibrate();
            };
            
            yield return new WaitUntil(() => isCallbackReceived);
            yield break; // 明确结束协程
        }
#endif

        // 通用震动逻辑
#if !UNITY_EDITOR
    Handheld.Vibrate();
#endif

        yield return null;

    }
}