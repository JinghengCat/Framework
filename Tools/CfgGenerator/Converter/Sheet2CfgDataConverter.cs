using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HPSF;

namespace CfgGenerator
{
    public class CfgData
    {
        public string cfgFullName;
        public string cfgNameSpace;
        public string cfgName;
        public List<string> fieldNames = new List<string>();
        public List<EDefineType> fieldTypes = new List<EDefineType>();
        public List<string> fieldDescription = new List<string>();
        public List<List<object>> fieldData = new List<List<object>>();

        public readonly Dictionary<object, List<object>> fieldDataDictionary = new Dictionary<object, List<object>>();
        
        public int FieldCount
        {
            get => fieldNames.Count;
        }

        public CfgData(string cfgNameSpace, string cfgName)
        {
            this.cfgNameSpace = cfgNameSpace;
            this.cfgName = cfgName;
            this.cfgFullName = cfgNameSpace + "/" + cfgName;
        }

        public void AddFieldData(List<object> singleFieldData)
        {
            object primaryKey = singleFieldData[0];
            if (fieldDataDictionary.ContainsKey(primaryKey))
            {
                throw new Exception($"Repeated primary key: {primaryKey}");
            }
            
            fieldDataDictionary.Add(primaryKey, singleFieldData);
            fieldData.Add(singleFieldData);
        }
        
        public void Merge(CfgData otherCfgData)
        {
            if (otherCfgData.cfgNameSpace != this.cfgNameSpace || otherCfgData.cfgName != this.cfgName)
            {
                throw new Exception("Merge cfg failed, different cfg");
            }

            for (int i = 0; i < this.FieldCount; i++)
            {
                if (this.fieldNames[i] != otherCfgData.fieldNames[i])
                {
                    throw new Exception($"Merge cfg failed, different field name: {this.fieldNames[i]} != {otherCfgData.fieldNames[i]}");
                }

                if (this.fieldTypes[i] != otherCfgData.fieldTypes[i])
                {
                    throw new Exception($"Merge cfg failed, different field type: {this.fieldTypes[i]} != {otherCfgData.fieldTypes[i]}");
                }
            }

            for (int i = 0; i < otherCfgData.fieldData.Count; i++)
            {
                this.AddFieldData(otherCfgData.fieldData[i]);
            }
        }
        
        public override string ToString()
        {
            string str = "";
            str += cfgNameSpace + "/" + cfgName + "\n";
            int fieldCount = FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                str += fieldNames[i] + "\t";
            }

            str += "\n";

            for (int i = 0; i < fieldCount; i++)
            {
                str += fieldTypes[i] + "\t";
            }

            str += "\n";
            
            for (int i = 0; i < fieldData.Count; i++)
            {
                for (int j = 0; j < fieldCount; j++)
                {
                    str += fieldData[i][j] + "\t";
                }
                str += "\n";
            }

            return str;
        }
    }
    
    public class Sheet2CfgDataConverter
    {
        private string _cfgFolderPath;
        
        public void OnInit(string cfgFolderPath)
        {
            _cfgFolderPath = cfgFolderPath;
        }

        public void OnRelease()
        {
            
        }

        private void FillFieldNames(AReader reader, CfgData cfgData)
        {
            for (int i = 0; i < reader.ColumnCount; i++)
            {
                string fieldName = reader.ReadString(0, i);
                cfgData.fieldNames.Add(fieldName);
            }
        }

        private void FillFieldTypes(AReader reader, CfgData cfgData)
        {
            for (int i = 0; i < reader.ColumnCount; i++)
            {
                string fieldType = reader.ReadString(1, i);
                cfgData.fieldTypes.Add(Utils.GetEDefineTypeByString(fieldType));
            }
        }

        private void FillFieldDescriptions(AReader reader, CfgData cfgData)
        {
            for (int i = 0; i < reader.ColumnCount; i++)
            {
                string fieldDescription = reader.ReadString(2, i);
                cfgData.fieldDescription.Add(fieldDescription);
            }
        }

        private void FillFieldData(AReader reader, CfgData cfgData)
        {
            for (int rowNum = 3; rowNum < reader.RowCount; rowNum++)
            {
                List<object> singleFieldData = new List<object>();
                for (int columnNum = 0; columnNum < reader.ColumnCount; columnNum++)
                {
                    EDefineType type = cfgData.fieldTypes[columnNum];
                    object value = Utils.Read(type, reader, rowNum, columnNum);
                    
                    singleFieldData.Add(value);
                }

                cfgData.AddFieldData(singleFieldData);
            }
        }
        
        private void FillCfgData(AReader reader, CfgData cfgData)
        {
            int rowCount = reader.RowCount;
            int columnCount = reader.ColumnCount;
            if (rowCount < 3 || columnCount < 1)
            {
                return;
            }
            
            // 第一行为field names
            FillFieldNames(reader, cfgData);
            // 第二行为field types
            FillFieldTypes(reader, cfgData);
            // 第三行为注释行
            FillFieldDescriptions(reader, cfgData);
            // 后面的行数为数据
            FillFieldData(reader, cfgData);
        }
        
        private List<CfgData> ConvertExcel(string filePath, Dictionary<string, CfgData> cfgDataDictionary)
        {
            List<CfgData> cfgDataList = new List<CfgData>();
            ExcelReader reader = new ExcelReader();
            reader.Open(filePath);
            try
            {
                if (reader.SheetCount == 0)
                {
                    reader.Close();
                    return cfgDataList;
                }

                for (int i = 0; i < reader.SheetCount; i++)
                {
                    reader.OpenSheet(i);

                    string cfgNameSpace = Utils.GetRelativeFolderPath(_cfgFolderPath, filePath);
                    string cfgName = reader.SheetName;
                    CfgData cfgData = new CfgData(cfgNameSpace, cfgName);
                    FillCfgData(reader, cfgData);
                    cfgDataList.Add(cfgData);
                }
            }
            catch (Exception e)
            {
                reader.Close();
                cfgDataList.Clear();
                throw e;
            }
            finally
            {
                reader.Close();
            }

            return cfgDataList;
        }

        private CfgData ConvertCsv(string filePath, Dictionary<string, CfgData> cfgDataDictionary)
        {
            return null;
        }

        private void AddOrMergeCfgData(Dictionary<string, CfgData> cfgDataDictionary, CfgData cfgData)
        {
            if (cfgDataDictionary.TryGetValue(cfgData.cfgFullName, out var cacheCfgData))
            {
                cacheCfgData.Merge(cfgData);
            }
            else
            {
                cfgDataDictionary.Add(cfgData.cfgFullName, cfgData);
            }
        }
        
        public Dictionary<string, CfgData> StartConvert()
        {
            Dictionary<string, CfgData> cfgDataDictionary = new Dictionary<string, CfgData>();
            try
            {
                string[] directoriesFullPath = Directory.GetDirectories(_cfgFolderPath);
                for (int i = 0; i < directoriesFullPath.Length; i++)
                {
                    string[] fileFullPath = Directory.GetFiles(directoriesFullPath[i]);
                    for (int j = 0; j < fileFullPath.Length; j++)
                    {
                        string filePath = fileFullPath[j];
                        if (Utils.IsExcel(filePath))
                        {
                            List<CfgData> cfgDataList = ConvertExcel(filePath, cfgDataDictionary);
                            if (cfgDataList != null)
                            {
                                foreach (var cfgData in cfgDataList)
                                {
                                    AddOrMergeCfgData(cfgDataDictionary, cfgData);
                                }
                            }
                        }
                        else if (Utils.IsCsv(filePath))
                        {
                            CfgData cfgData = ConvertCsv(filePath, cfgDataDictionary);
                            AddOrMergeCfgData(cfgDataDictionary, cfgData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            // foreach (var cfgData in cfgDataDictionary.Values)
            // {
            //     Console.WriteLine("-------------------------------------------------------------------");
            //     Console.WriteLine(cfgData.ToString());
            //     Console.WriteLine("-------------------------------------------------------------------");
            // }

            return cfgDataDictionary;
        }
    }
}