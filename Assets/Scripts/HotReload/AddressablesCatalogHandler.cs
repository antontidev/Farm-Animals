#define Debug
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

namespace RotaryHeart.Lib
{
    /// <summary>
    /// Class used to handle all remote and local catalog load, download and update
    /// </summary>
    public static class AddressablesCatalogHandler
    {
        /// <summary>
        /// Can be used to display any update information to the UI
        /// </summary>
        public struct UpdateDownloadStatus
        {
            public long totalBytes;
            public long downloadedBytes;
            public float packagePercentage;
            public string currentFile;
        }

        public static List<string> consoleOutput = new List<string>();
        static List<IResourceLocator> M_loadedLocators = new List<IResourceLocator>();
        static AsyncOperationHandle<IResourceLocator> M_initializeHandle;
        static string M_updateCatalogPath;
        
        /// <summary>
        /// Will hold any current download status information
        /// </summary>
        public static UpdateDownloadStatus CurrentDownloadStatus { get; private set; }

        /// <summary>
        /// Loads the local catalog with the provided hash name
        /// </summary>
        /// <returns>true if the catalog was successfully loaded; otherwise, false</returns>
        public static async UniTask<bool> LoadLocalCatalog(string savedHash)
        {
            consoleOutput.Clear();
            
            if (!M_initializeHandle.IsValid())
            {
                M_initializeHandle = Addressables.InitializeAsync();
                await M_initializeHandle.Task;
            }

            if (string.IsNullOrEmpty(savedHash))
            {
                return false;
            }
            
            //Path used for calculating the correct hash path by Unity
            string localSavedPath = GetCatalogPath(savedHash) + ".json";
            Log("Attempting to load local catalog: " + localSavedPath);
            
            //Get the proper hash path
            string localCorrectPath = GetCatalogPath(GetHasFromCatalog(localSavedPath)) + ".json";

            if (!System.IO.File.Exists(localCorrectPath))
            {
                Log("Local catalog file not found");
                return false;
            }

            AsyncOperationHandle<IResourceLocator> catalogHandle = Addressables.LoadContentCatalogAsync(localSavedPath);
            M_loadedLocators.Add(await catalogHandle.Task);
            Addressables.Release(catalogHandle);

            Log("Catalog loaded");
            return true;
        }

        /// <summary>
        /// Deletes all the cached data from the Addressables, including downloaded bundles and local catalogs
        /// </summary>
        public static void DeleteAllCachedData()
        {
            //Need to manually delete since Addressables throws an error when using CleanBundleCache
            // Addressables.CleanBundleCache();

            if (System.IO.Directory.Exists(Caching.currentCacheForWriting.path))
            {
                foreach (var cacheDir in System.IO.Directory.EnumerateDirectories(
                             Caching.currentCacheForWriting.path, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    System.IO.Directory.Delete(cacheDir, true);
                }
            }

            if (System.IO.Directory.Exists(Application.persistentDataPath + "/com.unity.addressables/"))
            {
                foreach (var cacheFiles in System.IO.Directory.EnumerateFiles(
                             Application.persistentDataPath + "/com.unity.addressables/", "*",
                             System.IO.SearchOption.TopDirectoryOnly))
                {
                    System.IO.File.Delete(cacheFiles);
                }
            }

            Caching.ClearCache();
        }
        
        /// <summary>
        /// Used to read the catalog and identify if a new update is available
        /// </summary>
        /// <param name="remoteCatalogPath">Remote path to load the catalog from</param>
        /// <returns>The size in bytes of the download (0 if none available) and the locator as a KeyValuePair</returns>
        public static async UniTask<KeyValuePair<long, IResourceLocator>> CheckCatalogUpdate(string remoteCatalogPath)
        {
            consoleOutput.Clear();
            
            Log("Loading remote catalog " + remoteCatalogPath);

            AsyncOperationHandle<IResourceLocator> loadContentHandle = Addressables.LoadContentCatalogAsync(remoteCatalogPath);
            IResourceLocator locator = await loadContentHandle.Task;

            Log("Catalog loaded, checking for update");

            AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(locator.Keys);
            long bytes = await sizeHandle.Task;
            
            Addressables.Release(sizeHandle);
            Addressables.Release(loadContentHandle);
            
            //Bytes will return 0 if no update needs to be downloaded
            if (bytes > 0)
            {
                Log("Update available: " + bytes);

                M_updateCatalogPath = remoteCatalogPath;
                return new KeyValuePair<long, IResourceLocator>(bytes, locator);
            }
            else
            {
                Log("No update available");

                string newHash = GetHasFromCatalog(remoteCatalogPath);
                DeleteLocalCatalog(newHash);
                
                return default;
            }
        }
        
        /// <summary>
        /// Downloads the catalog located at <paramref name="catalogPath"/>, if <paramref name="savedHash"/> is passed
        /// the local catalog files will be deleted
        /// </summary>
        /// <param name="catalogPath">The remote catalog path to download</param>
        /// <param name="savedHash">The hash name of the already downloaded remote catalog</param>
        /// <returns>The local hash of the downloaded catalog if succeeded; otherwise, null</returns>
        public static async UniTask<string> DownloadCatalog(string catalogPath, string savedHash = null)
        {
            if (string.IsNullOrEmpty(catalogPath))
            {
                return null;
            }
            
            //We need to make sure that the catalog is loaded (could have been unloaded before calling download)
            AsyncOperationHandle<IResourceLocator> loadContentHandle = Addressables.LoadContentCatalogAsync(catalogPath);
            await loadContentHandle.Task;

            AsyncOperationHandle<long> downloadSizeHandle = Addressables.GetDownloadSizeAsync(loadContentHandle.Result.Keys);
            await downloadSizeHandle.Task;

            Log($"Starting catalog download {catalogPath}");
            
            //This is the new hash that will be used for loading the local catalog
            string newHash = GetHasFromCatalog(M_updateCatalogPath);
            long totalDownloadedBytes = 0;

            //Make sure to delete the previously saved hash, this is required since we are using a different hash than the remote hash so Unity will not overwrite it
            if (!string.IsNullOrEmpty(savedHash))
            {
                string localSavedPath = GetCatalogPath(savedHash) + ".json";
                string localCorrectPath = GetCatalogPath(GetHasFromCatalog(localSavedPath));

                DeleteLocalCatalog(localCorrectPath);
            }
            
            UpdateDownloadStatus updateDownloadStatus = new UpdateDownloadStatus()
            {
                totalBytes = downloadSizeHandle.Result
            };

            int count = loadContentHandle.Result.Keys.Count();
            float index = 0f;
            
            //Need to ensure all keys are downloaded
            foreach (object key in loadContentHandle.Result.Keys)
            {
                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key);
                
                updateDownloadStatus.currentFile = key.ToString();
                long totalSize = downloadHandle.GetDownloadStatus().TotalBytes;
                
                //Wait until download is complete, we are not using await to be able to retrieve the download information
                while (!downloadHandle.IsDone)
                {
                    DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
                    updateDownloadStatus.downloadedBytes = totalDownloadedBytes + downloadStatus.DownloadedBytes;
                    CurrentDownloadStatus = updateDownloadStatus;
                    
                    await UniTask.Yield();
                }

                Addressables.Release(downloadHandle);

                totalDownloadedBytes += totalSize;
                    
                updateDownloadStatus.downloadedBytes = totalDownloadedBytes;
                updateDownloadStatus.packagePercentage = index / count;
                
                CurrentDownloadStatus = updateDownloadStatus;
                index++;
                
                await UniTask.Yield();
            }
            
            Log("Download completed");

            updateDownloadStatus.packagePercentage = 1;
            updateDownloadStatus.downloadedBytes = totalDownloadedBytes;
            
            CurrentDownloadStatus = updateDownloadStatus;
            
            await UniTask.Yield();

            M_loadedLocators.Add(loadContentHandle.Result);
            
            //We need to reload it using the new hash so that the local file name is changed with Unity calculated hash
            string newCatalog = GetCatalogPath(newHash + ".json");
            Log("Reloading using: " + newCatalog);
            AsyncOperationHandle<IResourceLocator> catalogHandle = Addressables.LoadContentCatalogAsync(newCatalog);
            Addressables.Release(catalogHandle);
            Addressables.Release(downloadSizeHandle);
            Addressables.Release(loadContentHandle);
            
            //Delete the cached remote catalog to prevent Unity from auto downloading when checking for new updates
            DeleteLocalCatalog(newHash);
            
            return newHash;
        }

        /// <summary>
        /// Used when canceling an update
        /// </summary>
        public static void CancelDownloadingUpdate(string catalogPath, IResourceLocator locator)
        {
            Log("Canceling download");

            //This avoids Unity from loading the assets on the current session
            Addressables.RemoveResourceLocator(locator);
            //Delete the downloaded remote catalog to prevent auto downloading update on next boot
            string hashName = GetHasFromCatalog(catalogPath);
            DeleteLocalCatalog(hashName);
        }

        /// <summary>
        /// Completely unloads all the system, including Addressables. NOTE This uses reflection, should only be called when updating
        /// </summary>
        public static void UnloadSystem()
        {
            //This avoids Unity from loading the assets on the current session
            foreach (IResourceLocator locator in M_loadedLocators)
            {
                Addressables.RemoveResourceLocator(locator);
            }

            M_loadedLocators.Clear();
            
            if (M_initializeHandle.IsValid())
            {
                Addressables.Release(M_initializeHandle);
            }

            //We need to force Addressables to be unloaded since Unity doesn't provide a function for reinitializing it
            //This is the same code as Addressables.m_Addressables static property editor only reinitialization
            var field = typeof(Addressables).GetField("m_AddressablesInstance", BindingFlags.Static |  BindingFlags.NonPublic);
            var oldInstance = field.GetValue(null);
            oldInstance.GetType().GetMethod("ReleaseSceneManagerOperation", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(oldInstance, null);
            field.SetValue(null, System.Activator.CreateInstance(field.FieldType, new LRUCacheAllocationStrategy(1000, 1000, 100, 10)));
        }

        /// <summary>
        /// Used to return the hashcode from a catalog path
        /// </summary>
        /// <param name="catalogPath">Catalog path to use</param>
        static string GetHasFromCatalog(string catalogPath)
        {
            return catalogPath.Replace(".json", ".hash").GetHashCode().ToString();
        }

        /// <summary>
        /// Used to delete a local catalog
        /// </summary>
        /// <param name="hashName">Hash used for the file name</param>
        static void DeleteLocalCatalog(string hashName)
        {
            string path = GetCatalogPath(hashName);
            Log("Attempting to delete: " + path);
            System.IO.File.Delete(path + ".json");
            System.IO.File.Delete(path + ".hash");
        }
        
        /// <summary>
        /// Returns the proper local catalog path from a hash code
        /// </summary>
        /// <param name="hash">Hash code to use</param>
        static string GetCatalogPath(string hash)
        {
            return System.IO.Path.Combine(Application.persistentDataPath, "com.unity.addressables", hash);
        }
        
        static void Log(string data)
        {
#if Debug
            consoleOutput.Add(data);
#endif
        }
        
    }
}
