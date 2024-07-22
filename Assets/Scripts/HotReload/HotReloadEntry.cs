using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HotReload;
using HybridCLR;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HotReload {
    public class HotReloadEntry : MonoBehaviour {
        public async UniTask LoadDLLS() {
            var dlls = await HotReloadAddressableAssetService.LoadByLabel<TextAsset>(new[] {"DLL"});

            foreach (var dll in dlls) {
#if !UNITY_EDITOR
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
                var dllBytes = await Addressables.LoadAssetAsync<TextAsset>(dll);

                Assembly hotUpdateAss = Assembly.Load(dllBytes.bytes);
#endif
            }

            var supplementaryMetadataDlls =
                await HotReloadAddressableAssetService.LoadByLabel<TextAsset>(new[] {"DLLMetadata"});

            foreach (var dll in supplementaryMetadataDlls) {
#if !UNITY_EDITOR
                var dllBytes = await Addressables.LoadAssetAsync<TextAsset>(dll);
                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes.bytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly");
#endif
            }
        }
    }
}