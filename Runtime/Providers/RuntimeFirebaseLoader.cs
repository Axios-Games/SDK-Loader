using System.IO;

namespace Axios.SDK
{
    #if FIREBASE_SDK_ENABLED
    using System;
    using UnityEngine;
    using System.Collections;
    using Firebase;
    using System.Threading;
    using System.Threading.Tasks;

    [CreateAssetMenu(fileName = "RuntimeFirebaseLoader", menuName = "SDK/Firebase SDK", order = 100)]
    public class RuntimeFirebaseLoader : RuntimeSDKLoader
    {
        public override string SDKName => "Firebase SDK";
        public override async Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            if (!ValidateConfiguration())
            {
                Debug.LogError($"[SDK Loader] {SDKName} has invalid configuration and won't be initialized.");
                return;
            }

            var initTask = FirebaseApp.CheckAndFixDependenciesAsync();

            if (await Task.WhenAny(initTask, Task.Delay(timeoutSeconds * 1000, cancellationToken)) == initTask) 
            {
                if (initTask.Result == DependencyStatus.Available)
                {
                    _status = RuntimeSdkStatus.Initialized;
                    Debug.Log($"[SDK Loader] {SDKName} Initialization Complete");
                }
                else
                {
                    _status = RuntimeSdkStatus.Failed;
                    Debug.LogWarning($"[SDK Loader] {SDKName} Initialization Failed: {initTask.Result}");
                }
               
            } 
            else
            {
                _status = RuntimeSdkStatus.Failed;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                _status = RuntimeSdkStatus.Stopped;
            }
        }

        public override bool ValidateConfiguration()
        {
            #if UNITY_ANDROID
            return  File.Exists("Assets/Resources/google-services.json");
            #elif UNITY_IOS
            return  File.Exists("Assets/Resources/GoogleService-Info.plist");
            #else
            return false;
            #endif
        }

        public override async Task Stop()
        {
            _status = RuntimeSdkStatus.Stopped;
            await Task.Yield();
        }
    }

    #elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeFirebaseLoader", menuName = "SDK/Firebase SDK", order = 100)]
    public class RuntimeFirebaseLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "FIREBASE_SDK_ENABLED";
        public override string SDKName => "Firebase SDK";

    }
    #endif
}