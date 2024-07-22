using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Editor.Scripts.HotReload;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class HybridCLRBuild {
        public static void MoveFile(string sourcePath, string destinationPath) {
            if (File.Exists(destinationPath)) {
                File.Delete(destinationPath);
            }
            File.Move(sourcePath, destinationPath);
        }

        public static string RenameFile(string filePath, string fileNewName) {
            var directoryName = Path.GetDirectoryName(filePath);

            string delimeter = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                delimeter = "\\";
            }
            else {
                delimeter = "//";
            }
                
            File.Move(filePath, directoryName + "\\" + fileNewName);
            return directoryName + "\\" + fileNewName;
        }
        public static T FindFirstAssetByType<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            foreach (var t in guids) {
                string assetPath = AssetDatabase.GUIDToAssetPath( t );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    return asset;
                }
            }

            return null;
        }
    
        [MenuItem("Finiki Games/Build/HybridCLR/Build hybrid clr fresh")]
        public static void BuildHybridCLRFresh() {
            var installerController = new HybridCLR.Editor.Installer.InstallerController();
            if (!installerController.HasInstalledHybridCLR()) {
                installerController.InstallDefaultHybridCLR();
            }
            
            MainBuild();
            
            AddressableAssetSettings.BuildPlayerContent();
        }
        
        [MenuItem("Finiki Games/Build/HybridCLR/Build hybrid clr update")]
        public static void BuildHybridCLRUpdate() {
            MainBuild();
            
            var input = new AddressablesDataBuilderInput(AddressableAssetSettingsDefaultObject.Settings);
            var updateBuild = new AddressablesBuildMenuUpdateAPreviousBuild();
            updateBuild.OnPrebuild(input);
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult _);
        }

        private static void MainBuild() {
            HybridCLRExtensions.GenerateAllLite(true, BuildTarget.Android);

            string projectPath = Application.dataPath;
            projectPath = projectPath.Replace($"/Assets", "");

            var hybridCLRConfig = FindFirstAssetByType<HybridCLRConfig>();
            
            var assemblies = HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyFilesExcludePreserved;
            var assembliesUrl = HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(BuildTarget.Android);

            var settings = HybridCLR.Editor.SettingsUtil.HybridCLRSettings;

            foreach (var assemblyName in assemblies) {
                if (settings.hotUpdateAssemblies.Contains(assemblyName.Replace(".dll", ""))) continue;
                var fullAssemblyPath = projectPath + "\\" + assembliesUrl + "\\" + assemblyName;
                
                var newAssemblyPath = RenameFile(fullAssemblyPath, assemblyName + ".bytes");

                var inProjectAssemblyPath =
                    projectPath + "\\" + hybridCLRConfig.DllPath + "\\" + assemblyName + ".bytes";
                MoveFile(newAssemblyPath, inProjectAssemblyPath);
            }

            var metadataAssemblies = hybridCLRConfig.MetadataAssemblyList;
            var metadataUrl = HybridCLR.Editor.SettingsUtil.GetAssembliesPostIl2CppStripDir(BuildTarget.Android);

            foreach (var metadataAssemblyName in metadataAssemblies) {
                var fullAssemblyPath = projectPath + "\\" + metadataUrl + "\\" + metadataAssemblyName;
                
                var newAssemblyPath = RenameFile(fullAssemblyPath, metadataAssemblyName + ".bytes");

                var inProjectAssemblyPath =
                    projectPath + "\\" + hybridCLRConfig.MetadataPath + "\\" + metadataAssemblyName + ".bytes";
                MoveFile(newAssemblyPath, inProjectAssemblyPath);
            }
        }
    }