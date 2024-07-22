using System.Collections.Generic;
using UnityEngine;

namespace Editor.Scripts.HotReload {
    [CreateAssetMenu(menuName = "Editor/HybridCLRConfig", fileName = "HybridCLRConfig")]
    public class HybridCLRConfig : ScriptableObject {
        public string DllPath;
        public string MetadataPath;

        public List<string> MetadataAssemblyList;
    }
}