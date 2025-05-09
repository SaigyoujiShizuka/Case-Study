using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeBridge : MonoBehaviour
{
    //单例
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
    // 数据模型
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

    // 接收原生消息的入口方法（
    public void OnNativeMessage(string message)
    {
        Debug.Log($"收到原生消息: {message}");

        // 解析JSON指令
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

    // 发送事件到原生端
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
