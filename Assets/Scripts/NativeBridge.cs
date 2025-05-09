using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeBridge : MonoBehaviour
{
    //����
    public  static NativeBridge Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    // ����ģ��
    [System.Serializable]
    private class NativeCommand
    {
        public string action;
        public string param;
    }

    [System.Serializable]
    public class NativeEvent
    {
        public string eventType;
        public string param;
        public string ToJson() => JsonUtility.ToJson(this);
    }

    // ����ԭ����Ϣ����ڷ�����
    public void OnNativeMessage(string message)
    {
        Debug.Log($"�յ�ԭ����Ϣ: {message}");

        // ����JSONָ��
        var cmd = JsonUtility.FromJson<NativeCommand>(message);
        ProcessCommand(cmd);
    }

    private void ProcessCommand(NativeCommand cmd)
    {
        switch (cmd.action.ToLower())
        {
            case "show_reward":
                RewardController.Instance.ShowRewardByType(cmd.param);
                break;
            case "toggle_pause":
                RewardController.Instance.TogglePause();
                break;
            case "stop":
                RewardController.Instance.StopAllAnimationsAndHide();
                break;
        }
    }

    // �����¼���ԭ����
    public void SendToNative(string eventType, string param = "")
    {
        var msg = new NativeEvent
        {
            eventType = eventType,
            param = param
        }.ToJson();

    #if UNITY_ANDROID
                using (var jc = new AndroidJavaClass("com.example.UnityBridge")) {
                    jc.CallStatic("sendToApp", msg);
                }
    #elif UNITY_IOS
                iOSNativeBridge.SendToApp(msg);
    #endif
        }

}
