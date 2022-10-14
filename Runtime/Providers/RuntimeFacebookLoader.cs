namespace Axios.SDK
{
    #if FACEBOOK_SDK_ENABLED
    using System;
    using System.Collections;
    using Facebook.Unity;
    using Facebook.Unity.Settings;
    using UnityEngine;
    using System.Threading;
    using System.Threading.Tasks;


    [CreateAssetMenu(fileName = "RuntimeFacebookLoader", menuName = "SDK/Facebook SDK", order = 100)]
    public class RuntimeFacebookLoader : RuntimeSDKLoader
    {
        [Header("Facebook SDK Settings:")] public FacebookSettings FacebookSettings;
        public override string SDKName => "Facebook SDK";


        public override Task Stop()
        {
            throw new NotImplementedException();
        }

        public override bool ValidateConfiguration()
        {
            if (FacebookSettings == null) return false;
            if (string.IsNullOrEmpty(FacebookSettings.ClientToken)) return false;
            if (!FacebookSettings.IsValidAppId) return false;
            return false;
        }

        public override async Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            _status = RuntimeSdkStatus.Starting;

            var isInitialized = FB.IsInitialized;

            if (!isInitialized) FB.Init(() => { isInitialized = true; });
            
            var isFacebookReady = Task.Run(async () =>
            {
                while (!isInitialized) await Task.Yield();
            }, cancellationToken);

            if (await Task.WhenAny(isFacebookReady, Task.Delay(timeoutSeconds * 1000, cancellationToken)) == isFacebookReady) 
            {
                _status = RuntimeSdkStatus.Initialized;
                FB.ActivateApp();
                Debug.Log($"[SDK Loader] {SDKName} Initialization Complete");
            } 
            else
            {
                _status = RuntimeSdkStatus.Failed;
            }
        }
    }

    #elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeFacebookLoader", menuName = "SDK/Facebook SDK", order = 100)]
    public class RuntimeFacebookLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "FACEBOOK_SDK_ENABLED";
        public override string SDKName => "Facebook SDK";
    }
    #endif
}