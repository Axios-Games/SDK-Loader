namespace Axios.SDK
{
    #if ADJUST_SDK_ENABLED
    using UnityEngine;
    using com.adjust.sdk;
    using System.Threading;
    using System.Threading.Tasks;

    [CreateAssetMenu(fileName = "RuntimeAdjustLoader", menuName = "SDK/Adjust SDK", order = 100)]
    public class RuntimeAdjustLoader : RuntimeSDKLoader
    {
        [Header("Adjust SDK Settings:")] 
        [SerializeField] private string _androidAppToken;
        [SerializeField] private string _IOSAppToken;
        [SerializeField] private string _WSAAppToken;
        [SerializeField] private AdjustEnvironment _debugBuildEnvironment = AdjustEnvironment.Sandbox;
        [SerializeField] private AdjustEnvironment _releaseBuildEnvironment = AdjustEnvironment.Production;
        [SerializeField] private bool _setSendInBackground = true;
        public override string SDKName => "Adjust SDK";
        
        
        public override async Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            _status = RuntimeSdkStatus.Starting;
            
            if (!ValidateConfiguration())
            {
                Debug.LogError($"[SDK Loader] {SDKName} has invalid configuration and won't be initialized.");
                return;
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                _status = RuntimeSdkStatus.Stopped;
            }

            var isDebugBuild = Debug.isDebugBuild;
            var adjustConfig = new AdjustConfig(
                GetAppTokenForPlatform(),
                isDebugBuild ? _debugBuildEnvironment : _releaseBuildEnvironment,
                true
            );

            adjustConfig.setLogLevel(isDebugBuild ? AdjustLogLevel.Debug : AdjustLogLevel.Error);
            adjustConfig.setSendInBackground(_setSendInBackground);
            new GameObject("Adjust").AddComponent<Adjust>();

            Adjust.start(adjustConfig);
            
            await Task.Yield();

            _status = RuntimeSdkStatus.Initialized;

            Debug.Log($"[SDK Loader] {SDKName} Initialization Complete");
        }
        private string GetAppTokenForPlatform()
        {
            #if UNITY_ANDROID
            return _androidAppToken;
            #elif UNITY_IOS
            return _iOSAppToken;
            #elif (UNITY_WSA || UNITY_WP8)
            return _wsaAppToken;
            #else
            return string.Empty;
            #endif
        }
        public override bool ValidateConfiguration()
        {
            return !string.IsNullOrEmpty(GetAppTokenForPlatform());
        }
        public override async Task Stop()
        {
            Adjust.setEnabled(false);
            _status = RuntimeSdkStatus.Stopped;
            await Task.Yield();
        }
    }

    #elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeAdjustLoader", menuName = "SDK/Adjust SDK", order = 100)]
    public class RuntimeAdjustLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "ADJUST_SDK_ENABLED";
        public override string SDKName => "Adjust SDK";
    }
    #endif
}