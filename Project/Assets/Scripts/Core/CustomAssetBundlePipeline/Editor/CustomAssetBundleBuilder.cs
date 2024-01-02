using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.CustomAssetBundlePipeline.Editor
{
    public class CustomAssetBundleBuilder
    {
        public class BuildSetting
        {
            public string outputPath;
            public AssetBundleBuild[] buildSettings;
            public BuildAssetBundleOptions buildOption = BuildAssetBundleOptions.ChunkBasedCompression;
            public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        }
    
        public static string inputPath = string.Format("Assets/AssetBundle");

        public static string INPUT_FULL_PATH = string.Format("{0}/AssetBundle", Application.dataPath);
        public static string OUTPUT_FULL_PATH = string.Format("{0}/../Build", Application.dataPath);

        private static BuildSetting InitBuildSetting()
        {
            var setting = new BuildSetting();

            var dirs = Directory.GetDirectories(INPUT_FULL_PATH);
            setting.buildSettings = new AssetBundleBuild[dirs.Length];
            setting.outputPath = OUTPUT_FULL_PATH;

            for (int i = 0; i < dirs.Length; i++)
            {
                var resDirPath= dirs[i].Replace("\\", "/");
                var resDirPathArr = resDirPath.Split('/');
                var resDirName = resDirPathArr[resDirPathArr.Length - 1];
            
                //创建文件夹
                if (Directory.Exists(OUTPUT_FULL_PATH))
                {
                    Directory.Delete(OUTPUT_FULL_PATH, true);
                }

                Directory.CreateDirectory(OUTPUT_FULL_PATH);

                //获取资源文件夹下所有文件路径
                var resFilesPath = Directory.GetFiles(resDirPath);
                List<string> filePathes = new List<string>();
                for (int index = 0; index < resFilesPath.Length; index++)
                {
                    var filePath = resFilesPath[index].Replace("\\", "/");
                    if (filePath.EndsWith(".meta"))
                    {
                        continue;
                    }
                    filePathes.Add(filePath);
                }

                //将所有文件写入设置
                AssetBundleBuild buildSettingData = new AssetBundleBuild();

                int count = filePathes.Count;
                string[] assetNames = new string[count];
                string[] addressableNames = new string[count];

                for (int index = 0; index < count; index++)
                {
                    var filePath = filePathes[index];
                    var fileName = Path.GetFileName(filePath);
                    var relativeFilePath = string.Format("{0}/{1}/{2}", inputPath, resDirName, fileName);
                    assetNames[index] = relativeFilePath;
                    addressableNames[index] = fileName;
                }

                buildSettingData.assetBundleName = resDirName.ToLower() + ".ab";

                buildSettingData.assetNames = assetNames;
                buildSettingData.addressableNames = addressableNames;

                setting.buildSettings[i] = buildSettingData;
            }

            return setting;
        }
        [MenuItem("Build/BuildAssetBundle")]
        public static void BuildAssetBundle()
        {
            var setting = InitBuildSetting();
            // LogSettings(setting);
            BuildPipeline.BuildAssetBundles(setting.outputPath, setting.buildSettings, setting.buildOption, setting.buildTarget);
            AssetDatabase.Refresh();
        }

        public static void LogSettings(BuildSetting setting)
        {
            Debug.Log("------------------------------------------------------------");
            Debug.LogFormat("OutPutPath = {0}, BuildOption = {1}, BuildTarget = {2}", setting.outputPath, setting.buildOption, setting.buildTarget);
            var buildSettings = setting.buildSettings;
            for (int i = 0; i < buildSettings.Length; i++)
            {
                Debug.LogFormat("Index = {0}, BundleName = {1}, Assets: ", i, buildSettings[i].assetBundleName);
                var assetNames = buildSettings[i].assetNames;
                var addressableNames = buildSettings[i].addressableNames;
                for (int j = 0; j < assetNames.Length; j++)
                {
                    Debug.LogFormat("AssetName = {0}, AddressableName = {1}", assetNames[j], addressableNames[j]);
                }
            }
            Debug.Log("------------------------------------------------------------");
        }

    }
}