using System.Threading;

namespace Axios.SDK
{
    #if DIVE_SDK_ENABLED
    using UnityEngine;
    using Dive;
    using System.Threading.Tasks;

    [CreateAssetMenu(fileName = "RuntimeDiveLoader", menuName = "SDK/Dive SDK", order = 100)]
    public class RuntimeDiveLoader : RuntimeSDKLoader
    {
        [Header("Dive SDK Settings:")] 
        [SerializeField] private string _appToken;
        [SerializeField] private string _hashKey;
        [SerializeField] private string _analyticsUrl;
        [SerializeField] private string _apiUrl;
        [SerializeField] private bool _showInfoLogs;
        [SerializeField] private Environment  _debugBuildEnvironment;
        [SerializeField] private Environment  _releaseBuildEnvironment;
        public override string SDKName => "Dive SDK";
        
        public override async Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            if (!ValidateConfiguration())
            {
                Debug.LogError($"[SDK Loader] {SDKName} has invalid configuration and won't be initialized.");
                return;
            }
            var currentEnv = Debug.isDebugBuild ? _debugBuildEnvironment : _releaseBuildEnvironment;
            var diveConfig = new DiveConfig(_appToken, _hashKey,  _analyticsUrl,  _apiUrl, currentEnv, true, _showInfoLogs);
            
            DiveSDK.Init(diveConfig);

            var hasDiveStarted = Task.Run(async () =>
            {
                while (!DiveSDK.SdkStarted) await Task.Yield();
            }, cancellationToken);
            
            if (await Task.WhenAny(hasDiveStarted, Task.Delay(timeoutSeconds * 1000, cancellationToken)) == hasDiveStarted) 
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
            return !string.IsNullOrEmpty( _appToken) && !string.IsNullOrEmpty( _hashKey) && !string.IsNullOrEmpty( _analyticsUrl) && !string.IsNullOrEmpty( _apiUrl);
        }

        public override Task Stop()
        {
            _status = RuntimeSdkStatus.Stopped;
            DiveSDK.Internal.StopSdk();
            return null;
        }
    }
    #elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeDiveLoader", menuName = "SDK/Dive SDK", order = 100)]
    public class RuntimeDiveLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "DIVE_SDK_ENABLED";
    }
    #endif
}