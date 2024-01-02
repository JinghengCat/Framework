using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CfgGenerator
{
    public enum EDefineType
    {
        UNKNOWN,
        INT,
        FLOAT,
        BOOL,
        STRING,
    }

    public static class Utils
    {
        private static Dictionary<string, EDefineType> _str2Type = new Dictionary<string, EDefineType>()
        {
            {"int", EDefineType.INT},
            {"integer", EDefineType.INT},

            {"float", EDefineType.FLOAT},
            
            {"bool", EDefineType.BOOL},
            {"boolean", EDefineType.BOOL},
            
            {"str", EDefineType.STRING},
            {"string", EDefineType.STRING},
        };

        public static EDefineType GetEDefineTypeByString(string str)
        {
            if (_str2Type.TryGetValue(str.ToLower(), out EDefineType type))
            {
                return type;
            }

            return EDefineType.UNKNOWN;
        }

        public static object Read(EDefineType type, AReader reader, int rowNum, int columnNum)
        {
            object value = null;
            switch (type)
            {
                case EDefineType.INT:
                {
                    value = reader.ReadInt(rowNum, columnNum);
                    break;
                }
                case EDefineType.FLOAT:
                {
                    value = reader.ReadFloat(rowNum, columnNum);
                    break;
                }
                case EDefineType.BOOL:
                {
                    value = reader.ReadBoolean(rowNum, columnNum);
                    break;
                }
                case EDefineType.STRING:
                {
                    value = reader.ReadString(rowNum, columnNum);
                    break;
                }
                default:
                {
                    throw new Exception("Unknown field value");
                }
            }

            return value;
        }

        public static bool IsExcel(string path)
        {
            return path.EndsWith(".xls") || path.EndsWith(".xlsx");
        }

        public static bool IsCsv(string path)
        {
            return path.EndsWith(".csv");
        }
        
        public static string GetRelativeFolderPath(string rootPath, string filePath)
        {
            string normalizeFilePath = filePath.Replace('\\', '/');
            string directoryPath = Path.GetDirectoryName(normalizeFilePath);
            string relativePath = directoryPath.TrimStart(rootPath.ToCharArray());
            relativePath = relativePath.TrimStart('/');
            
            return relativePath;
        }

        
        
        public static void TryCreateFile(string filePath, string content)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            // 如果文件夹不存在，先创建文件夹
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory '{directoryPath}': {ex.Message}");
                    return; // 如果文件夹创建失败，不再继续创建文件
                }
            }

            // 如果文件存在，先删除
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error deleting file '{filePath}': {ex.Message}");
                }
            }

            // 创建新文件
            try
            {
                FileStream fileStream = File.Create(filePath);
                TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8);
                textWriter.Write(content);
                
                textWriter.Close();
                textWriter.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file '{filePath}': {ex.Message}");
            }
        }

        public static string ReadTemplate(string templateFilePath)
        {
            return File.ReadAllText(templateFilePath);
        }
    }
}