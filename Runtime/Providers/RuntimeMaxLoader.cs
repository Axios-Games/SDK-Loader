namespace Axios.SDK
{
    #if MAX_SDK_ENABLED
    using UnityEngine;
    using System.Collections;
    using System.Threading.Tasks;
    using System.Threading;

    [CreateAssetMenu(fileName = "RuntimeMaxLoader", menuName = "SDK/Max SDK", order = 100)]
    public class RuntimeMaxLoader : RuntimeSDKLoader
    {
        [Header("Max SDK Settings:")] 
        [SerializeField] private string _SDKKey;
        public override string SDKName => "Max SDK";
       
        public override async Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            var isInitialized = false;
            
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                isInitialized = true;
            };

            MaxSdk.SetSdkKey(_SDKKey);
            MaxSdk.InitializeSdk();
            

            var hasMaxStarted = Task.Run(async () =>
            {
                while (!isInitialized) await Task.Yield();
            }, cancellationToken);
            
            if (await Task.WhenAny(hasMaxStarted, Task.Delay(timeoutSeconds * 1000, cancellationToken)) == hasMaxStarted) 
            {
                _status = RuntimeSdkStatus.Initialized;
                Debug.Log($"[SDK Loader] {SDKName} Initialization Complete");
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
            return !string.IsNullOrEmpty( _SDKKey);
        }

        public override async Task Stop()
        {
            _status = RuntimeSdkStatus.Stopped;
            await Task.Yield();
        }
    }
    #elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeMaxLoader", menuName = "SDK/Max SDK", order = 100)]
    public class RuntimeMaxLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "MAX_SDK_ENABLED";
        public override string SDKName => "Max SDK";
    }
    #endif
}