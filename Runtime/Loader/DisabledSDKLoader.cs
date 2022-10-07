using System.Threading;
using System.Threading.Tasks;

namespace Axios.SDK
{
    #if UNITY_EDITOR
    using System.Collections;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class DisabledSDKLoader : RuntimeSDKLoader
    {
        [Header("Define not found in Player Settings for this Platform.")]
        public bool AddDefine;
        public override string SDKName => "Disabled";
        
        protected virtual string SDKDefineName { get; }
        private void OnValidate()
        {
            if (AddDefine) AddDefineToSelectedBuildTarget();
        }

        private void AddDefineToSelectedBuildTarget()
        {
            var selectedTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(selectedTargetGroup);

            if (string.IsNullOrEmpty(currentDefines) || string.IsNullOrWhiteSpace(currentDefines))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(selectedTargetGroup, SDKDefineName);
                return;
            }

            var splitDefineList = currentDefines.Split(';');
            if (splitDefineList.Contains(SDKDefineName)) return;

            currentDefines += $";{SDKDefineName}";

            PlayerSettings.SetScriptingDefineSymbolsForGroup(selectedTargetGroup, currentDefines);

            AddDefine = false;
        }
        public override Task Init(int timeoutSeconds, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
        public override Task Stop()
        {
            throw new System.NotImplementedException();
        }

        public override bool ValidateConfiguration()
        {
            return false;
        }

    }
#endif
}