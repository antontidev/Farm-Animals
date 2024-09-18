using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace HotReload {
    public static class HotReloadAddressableAssetService {
        public static async UniTask<IList<T>> LoadByLabel<T>(string keys) where T : class
        {
            var objects = await Addressables.LoadAssetsAsync<object>(keys, null);
            return objects.OfType<T>().ToList();
            
            /*var result = Addressables.LoadResourceLocationsAsync(keys,
                Addressables.MergeMode.Union, typeof(T));

            await result;
            var locations = result.Result;
            var loadOps = new List<AsyncOperationHandle>(locations.Count);

            foreach (IResourceLocation location in locations) {
                AsyncOperationHandle handle = Addressables.LoadAssetAsync<object>(location);

                loadOps.Add(handle);
            }

            AsyncOperationHandle<IList<AsyncOperationHandle>> group
                = Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);
            await group.Task;

            var instances = loadOps.Select(x => (T) x.Result).ToList();
            return instances;*/
        }
    }
}