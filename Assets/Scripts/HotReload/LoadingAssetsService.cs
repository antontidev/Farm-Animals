using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HotReload;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadingAssetsService : MonoBehaviour {
    public HotReloadEntry HotReload;

    public AssetReference FirstSceneAssetReference;

    public Slider _slider;

    private void Start() {
        Load().Forget();
    }

    public async UniTask Load() {
        await CheckForCatalogUpdate();
        
        await HotReload.LoadDLLS();
        
        await Addressables.LoadSceneAsync(FirstSceneAssetReference);
        
        IResourceLocator resourceLocator = await Addressables.LoadContentCatalogAsync($"https://antontidev.github.io/addressables-hosting/Android/catalog_{Application.version}.json");
        
        long bytes = await Addressables.GetDownloadSizeAsync(resourceLocator.Keys);
        
        List<AsyncOperationHandle> downloadHandles = new();
        List<UniTask> downloadTasks = new();

        List<AsyncOperationHandle> downloadSizeHandles = new();

        if (bytes < 0) return;
        foreach (object key in resourceLocator.Keys) {
            var downloadSize = await Addressables.GetDownloadSizeAsync(key);
            if (downloadSize < 0) continue; 
            
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key);
            downloadTasks.Add(downloadHandle.ToUniTask());
            downloadHandles.Add(downloadHandle);
        }

        TrackResult(downloadSizeHandles);
        
        await UniTask.WhenAll(downloadTasks);
    }

    private async UniTask CheckForCatalogUpdate() {
        await Addressables.InitializeAsync();
        var catalogUpdates = await Addressables.CheckForCatalogUpdates();
        if (catalogUpdates.Count > 0) {
            await Addressables.UpdateCatalogs(catalogUpdates);

            Debug.Log($"Catalog updates {catalogUpdates.Count}");
        }
        
        await Addressables.LoadContentCatalogAsync($"https://antontidev.github.io/addressables-hosting/Android/catalog_{Application.version}.json");
    }

    private async UniTask TrackResult(List<AsyncOperationHandle> asyncOperationHandles) {
        
    }
}