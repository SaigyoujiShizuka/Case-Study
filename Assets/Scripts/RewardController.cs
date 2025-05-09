using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class RewardController : MonoBehaviour
{

    //����
    public static RewardController Instance { get; private set; }
    [Header("��Դ����")]
    [SerializeField]
    private string[] _prefabPaths = new string[]
    {

        "Prefabs/Cat",
        "Prefabs/Chiken",
        "Prefabs/Cow"
    };


    private GameObject[] _childPrefabs; // �洢�����������
    private AssetLoader _assetLoader; // ��Դ������
    private bool _isPaused = false; // ����Ƿ���ͣ
    private GameObject animalsObject;
    private Coroutine _vfxCoroutine; // ��ǰ���е�Э��
    private GameObject _currentActivePrefab; // ��ǰ�����Ԥ����

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
        _assetLoader = gameObject.AddComponent<AssetLoader>(); // �����Դ���������
        InitializePrefabs(); // ��ʼ��Ԥ����  

    }
    // ��ʼ��Ԥ����
    public void InitializePrefabs()
    {
        Transform parent = _assetLoader.CreateDefaultParent(transform, "Animals");
        animalsObject= parent.gameObject; // �������帳ֵ�� animalsObject
        _childPrefabs = new GameObject[_prefabPaths.Length];

        for (int i = 0; i < _prefabPaths.Length; i++)
        {
            StartCoroutine(LoadPrefabAsync(i, parent));
        }
    }
    // �첽����Ԥ���岢ʵ����
    private IEnumerator LoadPrefabAsync(int index, Transform parent)
    {
        yield return _assetLoader.LoadAndInstantiateAsync(
            _prefabPaths[index],
            instance => _childPrefabs[index] = instance,
            parent
        );

        if (_childPrefabs[index] == null)
        {
            Debug.LogError($"Ԥ�����ʼ��ʧ�ܣ�{_prefabPaths[index]}");
        }
    }
    /*    // ������Դ��������ӵ�������ķ���
        private GameObject LoadAndAttachPrefab(string path)
        {
            // ����Ԥ����
            GameObject loadedPrefab = Resources.Load<GameObject>(path);

            if (loadedPrefab != null)
            {
                // ��� Animals �����Ƿ���ڣ�����������򴴽�

                if (animalsObject == null)
                {
                    GameObject animalsObject = new GameObject("Animals");
                    animalsObject.transform.SetParent(transform); // ����Ϊ��ǰ�����������
                    this.animalsObject = animalsObject; 
                }
                // ʵ����Ԥ���岢����Ϊ Animals ��������
                GameObject instance = Instantiate(loadedPrefab, animalsObject.transform);
                instance.transform.localPosition = Vector3.zero; // �������λ��
                instance.SetActive(false); // ����Ϊ���ɼ�
                return instance;
            }
            else
            {
                Debug.LogError($"�޷�����Ԥ���壺{path}");
                return null;
            }
        }
    */
    // Update is called once per frame
    private void Update()
    {
        // ���� 1��2��3 ��ʾ��Ӧ��������
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

        // ���¿ո����ͣ��ָ�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }

        // ���� Z ��ֹͣ���ж�������������������
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StopAllAnimationsAndHide();
        }
    }
    // ���� VFX ��Э��
    private IEnumerator ContinuousVFXRoutine(float duration, float interval)
    {
        Transform spawnPoint = _currentActivePrefab.transform.Find("VFX_SpawnPoint");
        if (spawnPoint == null) yield break;

        while (_currentActivePrefab.activeSelf && !_isPaused)
        {
            ParticleSystem vfx = VFXPool.Instance.GetFromPool(spawnPoint.position);
            StartCoroutine(ReleaseVFXAfterDuration(vfx, duration)); // �������ӳ���1��

            // ����Ƶ��
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
    // ��ʾָ�������������岢��ʼ��λ��
    //�ⲿ����
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
    //�ڲ�����
    private void ShowPrefab(int index)
    {
        if (index >= 0 && index < _childPrefabs.Length && _childPrefabs[index] != null)
        {
            // ֹ֮ͣǰ��Э��
            if (_vfxCoroutine != null)
            {
                StopCoroutine(_vfxCoroutine);
                _vfxCoroutine = null;
            }
            // ��������������
            foreach (var prefab in _childPrefabs)
            {
                if (prefab != null)
                {
                    prefab.SetActive(false);
                }
            }
            _isPaused= false; // �ָ���ͣ״̬

            // ��ʾָ���������岢��ʼ��λ��

            _currentActivePrefab = _childPrefabs[index];
            _currentActivePrefab.SetActive(true);
            _currentActivePrefab.transform.localPosition = Vector3.zero;
            // ��ȡ Animator ��������ö���
            Animator animator = _currentActivePrefab.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
            }
            if (animalsObject != null)
            {
                // ��ȡ animalsObject �µĽű������� is_pause ����
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
 


            Debug.Log($"��ʾ�� {index + 1} ��Ԥ����");

            // ������������Э��
            _vfxCoroutine = StartCoroutine(ContinuousVFXRoutine(1f, 0.3f));
            //������Э��
            StartCoroutine(RequestVibration(1000));

        }
        else
        {
            Debug.LogWarning($"���� {index} ������Χ�������岻����");
        }
    }

    // ��ͣ��ָ����ж���
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
        //��animals�¹ҿ��Ľű���is_pause��������Ϊ��ǰ��ͣ״̬

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
        // ����ָ���ͣ�ҵ�ǰ�м����Ԥ���壬��������Э��
        if (!_isPaused && _currentActivePrefab != null && _currentActivePrefab.activeSelf)
        {
            _vfxCoroutine = StartCoroutine(ContinuousVFXRoutine(1f, 0.3f));
        }

        Debug.Log(_isPaused ? "��������ͣ" : "�����ѻָ�");
    }

    // ֹͣ���ж�������������������
    public void StopAllAnimationsAndHide()
    {
        foreach (var prefab in _childPrefabs)
        {
            if (prefab != null)
            {
                // ֹͣ����
                Animator animator = prefab.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false;
                }

                // ����������
                prefab.SetActive(false);
            }
        }

        Debug.Log("���ж�����ֹͣ������������������");
    }
    // ������Э��
    private IEnumerator RequestVibration(int durationMs)
    {
        bool hasPermission = true;

#if UNITY_ANDROID
        // Androidƽ̨Ȩ�޼��
        using (var permissionClass = new AndroidJavaClass("android.content.pm.PackageManager"))
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity")) 
        {
            string packageName = activity.Call<string>("getPackageName");
            int granted = permissionClass.GetStatic<int>("PERMISSION_GRANTED");
            hasPermission = permissionClass.CallStatic<int>("checkPermission", "android.permission.VIBRATE", packageName) == granted;
        }

        if (!hasPermission)
        {
            // ����Ȩ������
            NativeBridge.Instance.SendToNative("request_permission", "vibrate");
            
            // �ȴ�Ȩ�޻ص����ؼ��޸��㣩
            bool isCallbackReceived = false;
            PermissionManager.OnVibrationPermissionResult += (result) => {
                isCallbackReceived = true;
                if(result) Handheld.Vibrate();
            };
            
            yield return new WaitUntil(() => isCallbackReceived);
            yield break; // ��ȷ����Э��
        }
#endif

        // ͨ�����߼�
#if !UNITY_EDITOR
    Handheld.Vibrate();
#endif

        yield return null;

    }
}