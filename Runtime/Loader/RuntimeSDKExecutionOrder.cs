namespace Axios.SDK
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Threading.Tasks;
    using System.Threading;

    [CreateAssetMenu(fileName = "RuntimeSDKExecutionSequence", menuName = "SDK/Execution Sequence", order = 1)]
    public class RuntimeSDKExecutionOrder : ScriptableObject
    {
        [Header("Runtime Debug:")] 
        [SerializeField] private bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        [Header("Configuration:")] 
        [SerializeField] private List<RuntimeSDKLoader> _executionSequence;
        [SerializeField] private int _loadTimeOutSeconds;
        [SerializeField] private bool _verbose;
        private CancellationTokenSource _cancellationTokenSource;
        public async Task Init()
        {
            if (_verbose) Debug.Log($"[SDK Loader] Initializing sequence {name} .");

            if (_isInitialized)
            {
                if (_verbose) Debug.LogWarning($"[SDK Loader] Sequence {name} was already Initialized. If you want to start it again ensure to Stop it first");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            foreach (var sdk in _executionSequence)
            {
                if (sdk == null)
                {
                    Debug.LogError("[SDK Loader] Null or Empty SDK added to Execution Sequence. Please verify that you have the correct defines for this platform.");
                    continue;
                }

                if (sdk is DisabledSDKLoader)
                {
                    if (_verbose) Debug.LogWarning($"[SDK Loader] [{sdk.SDKName}] is not enabled and won't run on this platform.");
                    continue;
                }
                
                if (Application.isEditor && !sdk.RunInEditor)
                {
                    if (_verbose) Debug.Log($"[SDK Loader] [{sdk.SDKName}] is disabled in Unity Editor.");
                    continue;
                }
                await sdk.Init(_loadTimeOutSeconds, _cancellationTokenSource.Token);
                await Task.Yield();
            }

            _isInitialized = true;
        }
        
        public async Task Stop()
        {
            if (_isInitialized && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel(false);
            }
            foreach (var sdkLoader in _executionSequence)
            {
                _ = sdkLoader.Stop();
                await Task.Yield();
            }

            if (_verbose) Debug.Log($"[SDK Loader] Sequence {name} was stopped.");
            _isInitialized = false;
        }
        private void OnDisable() => Reset();
        public void Reset()
        {
            _isInitialized = false;
            _cancellationTokenSource?.Dispose();
        }
    }
}