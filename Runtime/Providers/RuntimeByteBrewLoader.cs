

namespace Axios.SDK
{
    #if BYTEBREW_SDK_ENABLED
    using UnityEngine;
    using ByteBrewSDK;
    using System.Threading;
    using System.Threading.Tasks;



    [CreateAssetMenu(fileName = "RuntimeByteBrewLoader", menuName = "SDK/ByteBrew SDK", order = 100)]
    public class RuntimeByteBrewLoader : RuntimeSDKLoader
    {
        [Header("ByteBrew SDK Settings:")] 
        [SerializeField]
        private ByteBrewSettings _byteBrewSettings;
        public override string SDKName => "ByteBrew SDK";
        public override async Task Init(int timeoutSeconds,CancellationToken cancellationToken)
        {
            _status = RuntimeSdkStatus.Starting;
            
            new GameObject("ByteBrew").AddComponent<ByteBrew>(); 
        
            ByteBrew.InitializeByteBrew();
            
            var hasByteBrewStarted = Task.Run(async () =>
            {
                while (!ByteBrew.IsByteBrewInitialized()) await Task.Yield();
            }, cancellationToken);
            
            if (await Task.WhenAny(hasByteBrewStarted, Task.Delay(timeoutSeconds * 1000, cancellationToken)) == hasByteBrewStarted) 
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
            if (_byteBrewSettings == null) return false;
            #if UNITY_ANDROID
            if (_byteBrewSettings.androidEnabled && !string.IsNullOrEmpty(_byteBrewSettings.androidGameID) &&
                !string.IsNullOrEmpty(_byteBrewSettings.androidSDKKey)) return true;
            #elif UNITY_IOS
            if (_byteBrewSettings.iosEnabled && !string.IsNullOrEmpty(_byteBrewSettings.iosGameID) &&
                !string.IsNullOrEmpty(_byteBrewSettings.iosSDKKey)) return true;
            #endif
            return false;
        }
        public override async Task Stop()
        {
            _status = RuntimeSdkStatus.Stopped;
            await Task.Yield();
        }
    }

    #elif UNITY_EDITOR
    using UnityEngine;
    [CreateAssetMenu(fileName = "RuntimeByteBrewLoader", menuName = "SDK/ByteBrew SDK", order = 100)]
    public class RuntimeByteBrewLoader : DisabledSDKLoader
    {
        protected override string SDKDefineName => "BYTEBREW_SDK_ENABLED";
        public override string SDKName => "ByteBrew SDK";
    }
    #endif
}