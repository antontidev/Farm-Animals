// Improved FastPlatformSwitcher for Unity 2018 (and maybe other Unity versions)
// For Asset Database v1
// by Andy Miira (Andrei Müller), October 2019
//
// Based on:
// Unity – fast build platform switcher script! (by Aymeric - Da Viking Code)
// https://davikingcode.com/blog/unity-fast-build-platform-switcher-script/
//
// A simple fast platform switcher for Unity (by Waldo Bronchart)
// https://gist.github.com/waldobronchart/b3cb789c028c199e2855
//
//
// [INSTRUCTIONS]
// 0) Add this script inside an "Editor" folder in your project.
// You must create this folder if it doesn't already exist.
//
// 1) Open Unity (preferably with administrator privileges),
// then click on "Tools -> Fast Build Switcher -> Use Cached Directory" at the menu bar.
//
// After the switcher window is loaded, select your desired Target Platform on the dropdown,
// then click on the "Switch Platform" button.
// Please do not switch to a Target Platform that you don't have installed, to avoid errors!
//
//
// [IMPORTANT]
// To switch the project's platform, ONLY use THIS platform switcher,
// instead of Unity's standard platform switcher (in Build Settings) or any other switcher.
//
// When you are switching to a Target Platform for the first time, Unity must create that platform's Library.
// You must wait for Unity's standard Library creation to finish,
// that could take a long time depending on your project's size.
// But at the next switches, if the Target Platform's Library already exists,
// the switch process will be MUCH faster.
//
// Some entries you may want to add in your .gitignore file:
// # Ignore all folders whose name starts with Library
// /[Ll]ibrary*/
// # Ignore any symlink called Library
// /[Ll]ibrary*
//
//
// Check out more Unity scripts and utilities at:
// https://gist.github.com/andreiagmu

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace MirrorMirai
{
    public class FastPlatformSwitcher : ScriptableWizard
    {
        public BuildTarget targetPlatform;

        [MenuItem("Tools/Fast Platform Switcher/Use Cached Directory")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<FastPlatformSwitcher>("Fast Platform Switcher (Use Cached Directory)", "Switch Platform");
        }

        void OnWizardCreate()
        {
            if (targetPlatform == 0) {
                Debug.LogWarning("You didn't select a valid Target Platform!");
                return;
            }

            var currentPlatform = EditorUserBuildSettings.activeBuildTarget;

            //Debug.Log("current platform: " + currentPlatform);
            //Debug.Log("next platform: " + buildTarget);

            if (currentPlatform == targetPlatform) {
                Debug.LogWarning("You selected the current platform as the Target Platform!");
                return;
            }

            // Don't switch when compiling
            if (EditorApplication.isCompiling) {
                Debug.LogWarning("Could not switch platform because Unity is compiling!");
                return;
            }

            // Don't switch while playing
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                Debug.LogWarning("Could not switch platform because Unity is in Play Mode!");
                return;
            }

            Debug.Log("Switching platform from " + currentPlatform + " to " + targetPlatform);

            //save current Library folder state
            if (Directory.Exists("Library-" + currentPlatform))
                DirectoryClear("Library-" + currentPlatform);

            DirectoryCopy("Library", "Library-" + currentPlatform, true);

            //restore new target Library folder state
            if (Directory.Exists("Library-" + targetPlatform)) {
                DirectoryClear("Library");
                //Directory.Delete("Library", true);

                //Directory.Move("Library-" + buildTarget, "Library");
                MoveDirectory("Library-" + targetPlatform, "Library");
            }

            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(targetPlatform);
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, targetPlatform);

            Debug.Log("Platform switched to " + targetPlatform);
        }

        void DirectoryClear(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles()) {
                //if (IsFileLocked(fi.FullName)) {
                    //Debug.Log("File is locked! " + fi.FullName);
                //}

                if (IsFileBlacklisted(fi.Name))
                    continue;

                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories()) {
                DirectoryClear(di.FullName);
                di.Delete(true);
            }
        }

        void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files) {
                if (IsFileBlacklisted(file.Name))
                    continue;

                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
                foreach (DirectoryInfo subdir in dirs) {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
        }

        /// <summary>
        ///  Moves a file or a directory and its contents to an existing location (by doing a Recursive Files Move)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                .GroupBy(s=> Path.GetDirectoryName(s));

            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);

                foreach (var file in folder) {
                    if (IsFileBlacklisted(Path.GetFileName(file)))
                        continue;

                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }
            Directory.Delete(source, true);
        }

        /// <summary>
        /// Checks for files that shouldn't be moved or copied (mainly files that are being used by some Unity process)
        /// </summary>
        /// <returns></returns>
        private static bool IsFileBlacklisted(string filename)
        {
            if (filename == "ShaderCache.db" || filename.StartsWith("shadercompiler-UnityShaderCompiler.exe"))
                return true;

            return false;
        }

        // From a previous test I made, to detect locked files.
        private static bool IsFileLocked(string file)
        {
            // Check that problem is not in destination file
            if (File.Exists(file)) {
                FileStream stream = null;

                try {
                    stream = new FileInfo(file).Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    //stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception e) {
                    //Debug.Log("Error in checking whether file is locked: " + file);
                    //Debug.Log(e);
                    if (e is IOException) {
                        return true;
                    }
                }
                finally {
                    if (stream != null)
                        stream.Close();
                }
            }

            return false;
        }
    }
}
