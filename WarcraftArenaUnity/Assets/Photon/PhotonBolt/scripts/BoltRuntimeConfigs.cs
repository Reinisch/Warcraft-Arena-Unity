using UnityEngine;
using Bolt;

[BoltGlobalBehaviour]
public class BoltRuntimeConfigs : Bolt.GlobalEventListener
{
    void Awake()
    {
#if ENABLE_IL2CPP
        UnitySettings.IsBuildIL2CPP = true;
#elif ENABLE_MONO
        UnitySettings.IsBuildMono = true;
#elif ENABLE_DOTNET
        UnitySettings.IsBuildDotNet = true;
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        UnitySettings.CurrentPlatform = RuntimePlatform.OSXPlayer;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        UnitySettings.CurrentPlatform = RuntimePlatform.WindowsPlayer;
#elif UNITY_STANDALONE_LINUX
        UnitySettings.CurrentPlatform = RuntimePlatform.LinuxPlayer;
#elif UNITY_IOS || UNITY_IPHONE
        UnitySettings.CurrentPlatform = RuntimePlatform.IPhonePlayer;
#elif UNITY_ANDROID
        UnitySettings.CurrentPlatform = RuntimePlatform.Android;
#endif
    }
}


