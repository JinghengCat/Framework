using System;
using System.IO;
using UnityEngine;

namespace Core.Common
{
    public static class FileUtils
    {
        public static bool IsDirectoryExist(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public static bool IsFileExit(string filePath)
        {
            return File.Exists(filePath);
        }

        public static string[] GetDirectoriesInThisDirectory(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }
        
        public static string[] GetFilesInThisDirectory(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public static string GetDirectoryPathByFilePath(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        public static string GetFileRelativePath(string rootPath, string filePath)
        {
            string relativeFilePath = filePath.Replace(rootPath, string.Empty);
            relativeFilePath = relativeFilePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relativeFilePath;
        }

        public static string GetFileRelativePathByDirectoryName(string dirName, string filePath)
        {
            int index = filePath.IndexOf(dirName);
            int startIndex = index + dirName.Length + 1; // 把分隔符也去掉
            int length = filePath.Length - startIndex;
            return filePath.Substring(startIndex, length);
        }
        
        public static string GetPathRootByValue(string absoluteOrRelativePath)
        {
            int index = -1;
            for (int i = 0; i < absoluteOrRelativePath.Length; i++)
            {
                char c = absoluteOrRelativePath[i];
                if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
                {
                    index = i;
                }
            }

            if (index != -1)
            {
                return absoluteOrRelativePath.Substring(0, index);
            }
            else
            {
                throw new Exception("[FileUtils] GetPathRoot failed");
            }
        }

        public static string ReplaceDirectorySeparator(string s, char c)
        {
            string newStr = s.Replace(Path.DirectorySeparatorChar, c);
            newStr = newStr.Replace(Path.AltDirectorySeparatorChar, c);
            return newStr;
        }
    }
}
