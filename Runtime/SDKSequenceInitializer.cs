namespace Axios.SDK
{
    using System.Collections;
    using UnityEngine;
    using System.Threading.Tasks;
    public class SDKSequenceInitializer : MonoBehaviour
    {
        [SerializeField] private RuntimeSDKExecutionOrder _SDKExecutionOrder;
        [SerializeField] private bool _initOnAwake;
        [SerializeField] private bool _destroyAfterInit;

        private void Awake()
        {
            if (_initOnAwake) Init();
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            if (_SDKExecutionOrder == null)
            {
                Debug.LogWarning("[SDK Loader] No SDK Sequence added to the Initializer");
            }

            _ = _SDKExecutionOrder.Init();
            if (_destroyAfterInit) StartCoroutine(WaitAndDestroyObject());
        }

        private IEnumerator WaitAndDestroyObject()
        {
            if (_SDKExecutionOrder == null) yield break;
            yield return new WaitUntil(() => _SDKExecutionOrder.IsInitialized);
            Destroy(gameObject);
        }
        
        
    }
    
}