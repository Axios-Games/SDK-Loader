namespace Axios.SDK
{
    #if GAMEANALYTICS_SDK_ENABLED
    using UnityEngine;
    using GameAnalyticsSDK;
    using GameAnalyticsSDK.Setup;
    using System.Threading;
    using System.Threading.Tasks;
    
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [CreateAssetMenu(fileName = "RuntimeGameAnalyticsLoader", menuName = "SDK/GameAnalytics SDK", order = 100)]
    public class RuntimeGameAnalyticsLoader : RuntimeSDKLoader
    {
        [Header("GameAnalytics SDK Settings:")]
        public Settings GameAnalyticsSetting;
        public override string SDKName => "GameAnalytics SDK";


        public override bool ValidateConfiguration()
        {
            if (GameAnalyticsSetting == null || GameAnalyticsSetting.Platforms == null || GameAnalyticsSetting.Platforms.Count == 0) 
                return false;

            var activePlatforms = GameAnalyticsSetting.Platforms;
            for (int i = 0; i < activePlatforms.Count; i++)
            {
                var hasValidKeys = !string.IsNullOrEmpty(GameAnalyticsSetting.GetGameKey(i)) && !string.IsNullOrEmpty(GameAnalyticsSetting.GetSecretKey(i));
                
                if (activePlatforms[i] == RuntimePlatform.Android && EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.LinuxPlayer && EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64 && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.IPhonePlayer && EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.WebGLPlayer && EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.tvOS && EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.OSXPlayer && EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.WSAPlayerARM && EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer && hasValidKeys) return true;
                if (activePlatforms[i] == RuntimePlatform.WindowsPlayer && 
                    (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows || 
                     EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 ) && hasValidKeys) return true;
            }

            return false;
            
        }

        public override async Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            _status = RuntimeSdkStatus.Starting;
            if (!ValidateConfiguration())
            {
                Debug.LogError($"[SDK Loader] {SDKName} has invalid configuration and won't be initialized.");
                _status = RuntimeSdkStatus.Failed;
                return;
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                _status = RuntimeSdkStatus.Stopped;
                return;
            }
            
            GameAnalytics.Initialize();
            await Task.Yield();
            _status = RuntimeSdkStatus.Initialized;
            Debug.Log($"[SDK Loader] {SDKName} Initialization Complete");
        }
        public override  async Task Stop()
        {
            GameAnalytics.SetEnabledEventSubmission(false);
            GameAnalytics.EndSession();
            _status = RuntimeSdkStatus.Stopped;
            await Task.Yield();
        }
    }

#elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeGameAnalyticsLoader", menuName = "SDK/GameAnalytics SDK", order = 100)]
    public class RuntimeGameAnalyticsLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "GAMEANALYTICS_SDK_ENABLED";
        public override string SDKName => "GameAnalytics SDK";
    }
#endif
            }