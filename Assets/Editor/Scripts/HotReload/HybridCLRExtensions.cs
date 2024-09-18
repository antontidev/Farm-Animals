using System.Collections.Generic;
using HybridCLR.Editor.Commands;
using UnityEditor;

namespace Editor.Scripts.HotReload {
    public static class HybridCLRExtensions {
        [MenuItem("HybridCLR/Generate/All but lite", priority = 200)]
        public static void GenerateAllLite(bool develop, BuildTarget buildTarget)
        {
            if (develop) {
                CompileDllCommand.CompileDll(buildTarget, true);
            }
            else {
                CompileDllCommand.CompileDll(buildTarget);
            }

            Il2CppDefGeneratorCommand.GenerateIl2CppDef();

            // 这几个生成依赖HotUpdateDlls
            LinkGeneratorCommand.GenerateLinkXml(buildTarget);

            // 生成裁剪后的aot dll
            StripAOTDllCommand.GenerateStripedAOTDlls(buildTarget);
            
            var hybridCLRConfig = HybridCLRBuild.FindFirstAssetByType<HybridCLRConfig>();

            hybridCLRConfig.MetadataAssemblyList = new List<string>();
            var metadataList = AOTGenericReferences.PatchedAOTAssemblyList;
            foreach (var dll in metadataList) {
                hybridCLRConfig.MetadataAssemblyList.Add(dll);
            }
        }
    }
}