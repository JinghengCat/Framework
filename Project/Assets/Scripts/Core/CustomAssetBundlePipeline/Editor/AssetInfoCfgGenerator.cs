using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.Res;
using UnityEngine;
using UnityEditor;

namespace Core.CustomAssetBundlePipeline.Editor
{
    public static class AssetInfoCfgGenerationSetting
    {
        public static HashSet<string> ASSETINFO_SOURCE_RELATIVE_PATH = new HashSet<string>()
        {
            "AssetBundle",
        };

        public static readonly string ASSET_CSV_RELATIVE_FILE_PATH = Application.dataPath + "/../Cfg/Asset/AssetInfo.csv";
    }
    
    public class AssetInfo_Template
    {
        public string rawAssetPath;
        public string assetPath;
        public int assetType;
        public string bundlePath;
        public List<AssetInfo_Template> dependencies = new List<AssetInfo_Template>();

        public override string ToString()
        {
            string str = $"assetPath = {assetPath}\n";
            str += $"assetType = {assetType}\n";
            str += $"bundlePath = {bundlePath}\n";

            string dep = "";
            for (int i = 0; i < dependencies.Count; i++)
            {
                dep += "\t" + dependencies[i].assetPath + "\n";
            }

            str += $"dependencies = \n{dep}";
            
            return str;
        }
    }
    
    public static class AssetInfoCfgGenerator
    {
        private const string PREFAB_EXTENSION = ".prefab";
        private const string MATERIAL_EXTENSION = ".mat";
        private const string ASSETBUNDLE_EXTENSION = ".ab";
        private const string META_EXTENSION = ".meta";
        
        [MenuItem("Build/构建AssetInfo表")]
        public static void GenerateAssetCsv()
        {
            Dictionary<string, AssetInfo_Template> assetDict = new Dictionary<string, AssetInfo_Template>();
            List<AssetInfo_Template> assetList = new List<AssetInfo_Template>();
            // 第一步 收集asset的基本信息
            foreach (string relativePath in AssetInfoCfgGenerationSetting.ASSETINFO_SOURCE_RELATIVE_PATH)
            {
                string fullFolderPath = Application.dataPath + "/" + relativePath;
                RecursiveCreateBasicAssetInfo(Application.dataPath, fullFolderPath, assetDict, assetList);
            }
            
            Dictionary<string, AssetInfo_Template> bundleDict = new Dictionary<string, AssetInfo_Template>();
            List<AssetInfo_Template> bundleList = new List<AssetInfo_Template>();
            // 第二步 根据asset的基本信息 生成bundle信息
            foreach (AssetInfo_Template iterateAssetInfo in assetList)
            {
                // TODO 这里还能继续细分bundle 前期先不弄了
                // e.g 根据文件大小拆分bundle; 根据文件交集拆分bundle
                string relativePathRoot = FileUtils.GetPathRootByValue(iterateAssetInfo.rawAssetPath);
                string relativeBundlePath = relativePathRoot + ASSETBUNDLE_EXTENSION;
                iterateAssetInfo.bundlePath = relativeBundlePath;
                
                if (!bundleDict.ContainsKey(relativeBundlePath))
                {
                    AssetInfo_Template bundleInfo = CreateAssetInfoByFilePath(relativeBundlePath);
                    InsertAssetInfoTemplate(bundleInfo, bundleDict, bundleList);
                }
            }
            
            // 第三步 根据AssetDataBase导出的依赖信息 生成 assetInfo的依赖信息
            foreach (AssetInfo_Template iterateAssetInfo in assetList)
            {
                if (iterateAssetInfo.assetType == EAssetType.AssetBundle)
                {
                    continue;
                }

                string path = "Assets/" + iterateAssetInfo.assetPath;
                string[] depAssetPaths = AssetDatabase.GetDependencies(path, false);
                if (depAssetPaths == null || depAssetPaths.Length == 0)
                {
                    continue;
                }

                Dictionary<string, AssetInfo_Template> repeatChecker = new Dictionary<string, AssetInfo_Template>();
                foreach (var depAssetPath in depAssetPaths)
                {
                    string relativePath = FileUtils.GetFileRelativePathByDirectoryName("Assets", depAssetPath);
                    var assetPath = NormalizePath(relativePath);
                    if (assetDict.TryGetValue(assetPath, out var refAssetInfo))
                    {
                        if (repeatChecker.ContainsKey(refAssetInfo.assetPath))
                        {
                            throw new Exception("repeat add dependencies");
                        }
                        iterateAssetInfo.dependencies.Add(refAssetInfo);
                        repeatChecker.Add(refAssetInfo.assetPath, refAssetInfo);
                    }
                    else
                    {
                        Debug.LogError($"Did not find ref asset info {assetPath}");
                        throw new Exception();
                    }
                }
                
                Debug.Log(iterateAssetInfo.ToString());
            }
        }

        private static void RecursiveCreateBasicAssetInfo(string rootPath, string folderFullPath, Dictionary<string, AssetInfo_Template> dict, List<AssetInfo_Template> list)
        {
            if (!FileUtils.IsDirectoryExist(folderFullPath))
            {
                return;
            }

            string[] directoryPaths = FileUtils.GetDirectoriesInThisDirectory(folderFullPath);
            foreach (var directoryPath in directoryPaths)
            {
                RecursiveCreateBasicAssetInfo(rootPath, directoryPath, dict, list);
            }
            
            string[] filePaths = FileUtils.GetFilesInThisDirectory(folderFullPath);

            if (filePaths == null || filePaths.Length == 0)
            {
                return;
            }


            foreach (string filePath in filePaths)
            {
                if (filePath.EndsWith(META_EXTENSION))
                {
                    continue;
                }
                string fileRelativePath = FileUtils.GetFileRelativePath(rootPath, filePath);
                AssetInfo_Template assetInfo = CreateAssetInfoByFilePath(fileRelativePath);
                InsertAssetInfoTemplate(assetInfo, dict, list);                    
            }
        }
        
        private static string NormalizePath(string path)
        {
            string normalizedPath = path.Replace("\\", "/");
            normalizedPath = normalizedPath.ToLower();
            return normalizedPath;
        }

        private static int GetAssetTypeByAssetPath(string filePath)
        {
            if (filePath.EndsWith(PREFAB_EXTENSION))
            {
                return EAssetType.Prefab;
            }
            else if (filePath.EndsWith(MATERIAL_EXTENSION))
            {
                return EAssetType.Material;
            }
            else if (filePath.EndsWith(ASSETBUNDLE_EXTENSION))
            {
                return EAssetType.AssetBundle;
            }
            else
            {
                return EAssetType.Asset;
            }
        }

        private static AssetInfo_Template CreateAssetInfoByFilePath(string relativePath)
        {
            AssetInfo_Template assetInfo = new AssetInfo_Template();
            assetInfo.rawAssetPath = relativePath;
            assetInfo.assetPath = NormalizePath(relativePath);
            assetInfo.assetType = GetAssetTypeByAssetPath(relativePath);

            return assetInfo;
        }

        private static bool InsertAssetInfoTemplate(AssetInfo_Template assetInfo, Dictionary<string, AssetInfo_Template> dict, List<AssetInfo_Template> list)
        {
            if (!dict.ContainsKey(assetInfo.assetPath))
            {
                dict.Add(assetInfo.assetPath, assetInfo);
                list.Add(assetInfo);
                return true;
            }

            return false;
        }
    }
}
