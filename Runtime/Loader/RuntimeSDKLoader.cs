namespace Axios.SDK
{
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    public abstract class RuntimeSDKLoader : ScriptableObject
    {
        [Header("Runtime Debug:")] 
        [SerializeField] protected RuntimeSdkStatus _status;
        [SerializeField] private bool _runInEditor;
       
        public RuntimeSdkStatus Status => _status;
        public bool RunInEditor => _runInEditor;
        public abstract string SDKName { get; }
        public abstract Task Init(int timeoutSeconds, CancellationToken cancellationToken);
        public abstract Task Stop();
        public abstract bool ValidateConfiguration();
        private void OnDisable() => Reset();
        protected void Reset()
        {
            _status = RuntimeSdkStatus.Waiting;
        }
    }
}