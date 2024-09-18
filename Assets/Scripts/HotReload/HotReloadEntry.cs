using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HotReload;
using HybridCLR;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HotReload {
    public class HotReloadEntry : MonoBehaviour
    {
        public AssetReference StartSceneReference;

        private void Awake()
        {
            LoadDLLS().Forget();
        }

        public async UniTask LoadDLLS() {
            try
            {
                var dlls = await HotReloadAddressableAssetService.LoadByLabel<TextAsset>("DLLS");

                foreach (var dll in dlls)
                {
#if !UNITY_EDITOR
                    // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
                    Assembly hotUpdateAss = Assembly.Load(dll.bytes);
#endif
                }
            }
            catch (Exception e) {
                Debug.LogError($"Load DLLs error: {e.Message}");
            }

            try
            {
                var supplementaryMetadataDlls =
                    await HotReloadAddressableAssetService.LoadByLabel<TextAsset>("DLLSMetadata");

                foreach (var dll in supplementaryMetadataDlls)
                {
#if !UNITY_EDITOR
                    var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dll.bytes, HomologousImageMode.SuperSet);
                    Debug.Log($"LoadMetadataForAOTAssembly");
    #endif
                }
            }
            catch (Exception e) {
                Debug.LogError($"Load Metadata DLLs error: {e.Message}");
            }

            await StartSceneReference.LoadSceneAsync();
        }
    }
}