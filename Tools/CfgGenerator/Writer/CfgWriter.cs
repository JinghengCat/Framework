using System;
using System.Collections.Generic;
using System.IO;

namespace CfgGenerator
{
    public abstract class CfgWriter
    {
        protected CfgData _cfgData;

        public virtual void SetCfgData(CfgData cfgData)
        {
            _cfgData = cfgData;
        }

        public abstract void Write(string outputPath);
    }

    public class CSharpCfgWriter : CfgWriter
    {
        private const string NAMESPACE = "NAMESPACE";
        private const string CFG_CLASS_NAME = "CFG_CLASS_NAME";
        private const string CFG_NAME = "CFG_NAME";
        private const string PRIMARY_KEY_TYPE = "PRIMARY_KEY_TYPE";

        private const string FIELD_DEFINE = "/*FIELD_DEFINE*/";
        private const string CTOR_PARAM = "/*CTOR_PARAM*/";
        private const string CTOR = "/*CTOR*/";
        private const string DICT_FIELD_VALUE = "/*DICT_FIELD_VALUE*/";

        private const string CTOR_TEMPLATE = "this.{0} = {1};";
        private const string FIELD_DEFINE_TEMPLATE = "public readonly {0} {1};";
        private const string DICT_FIELD_TEMPLATE = "{0}, new {1}({2})";

        private const string TAB = "    ";

        private string _template;

        public void SetTemplate(string template)
        {
            _template = template;
        }

        private string GetFieldDefineString()
        {
            string str = "";
            string END = "\n" + TAB + TAB;
            for (int i = 0; i < _cfgData.FieldCount; i++)
            {
                EDefineType type = _cfgData.fieldTypes[i];
                string fieldTypeStr = DynamicType_ToString(type);
                string fieldNameStr = _cfgData.fieldNames[i];
                str += string.Format(FIELD_DEFINE_TEMPLATE, fieldTypeStr, fieldNameStr) + END;
            }

            return str;
        }

        private string GetCtorParamString()
        {
            string str = "";
            for (int i = 0; i < _cfgData.FieldCount; i++)
            {
                EDefineType type = _cfgData.fieldTypes[i];
                string fieldTypeStr = DynamicType_ToString(type);
                string fieldNameStr = _cfgData.fieldNames[i];

                str += fieldTypeStr + " " + fieldNameStr;
                if (i != _cfgData.FieldCount - 1)
                {
                    str += ", ";
                }
            }

            return str;
        }

        private string GetCtorString()
        {
            string str = "";
            string END = "\n" + TAB + TAB + TAB;
            for (int i = 0; i < _cfgData.FieldCount; i++)
            {
                string fieldNameStr = _cfgData.fieldNames[i];
                str += string.Format(CTOR_TEMPLATE, fieldNameStr, fieldNameStr) + END;
            }

            return str;
        }

        private string GetDictValueString()
        {
            string str = "";
            string END = "\n" + TAB + TAB + TAB;

            List<EDefineType> typeList = _cfgData.fieldTypes;

            string cfgClassName = GetCfgClassName();
            List<List<object>> fieldDataList = _cfgData.fieldData;
            int fieldCount = _cfgData.FieldCount;

            for (int row = 0; row < fieldDataList.Count; row++)
            {
                List<object> fieldData = fieldDataList[row];

                string primaryKeyStr = null;
                string paramStr = "";

                for (int dataIndex = 0; dataIndex < fieldCount; dataIndex++)
                {
                    object value = fieldData[dataIndex];
                    EDefineType type = typeList[dataIndex];

                    string valueStr = DynamicValue_ToString(type, value);
                    if (dataIndex == 0)
                    {
                        primaryKeyStr = valueStr;
                    }

                    paramStr += valueStr;
                    if (dataIndex != fieldCount - 1)
                    {
                        paramStr += ", ";
                    }
                }

                str += "{" + string.Format(DICT_FIELD_TEMPLATE, primaryKeyStr, cfgClassName, paramStr) + "}," + END;
            }

            return str;
        }

        private string GetCfgClassName()
        {
            return _cfgData.cfgName;
        }

        private string GetCfgName()
        {
            return _cfgData.cfgName + "Cfg";
        }
        
        public override void Write(string outputPath)
        {
            if (_cfgData.FieldCount == 0)
            {
                return;
            }

            string content = string.Copy(_template);

            string nameSpace = _cfgData.cfgNameSpace.Replace("/", ".");
            content = content.Replace(NAMESPACE, nameSpace);

            string cfgClassName = GetCfgClassName();
            content = content.Replace(CFG_CLASS_NAME, cfgClassName);

            string cfgName = GetCfgName();
            content = content.Replace(CFG_NAME, cfgName);

            string primaryKeyStr = DynamicType_ToString(_cfgData.fieldTypes[0]);
            content = content.Replace(PRIMARY_KEY_TYPE, primaryKeyStr);

            content = content.Replace(FIELD_DEFINE, GetFieldDefineString());
            content = content.Replace(CTOR_PARAM, GetCtorParamString());
            content = content.Replace(CTOR, GetCtorString());
            content = content.Replace(DICT_FIELD_VALUE, GetDictValueString());
            
            Utils.TryCreateFile(outputPath, content);
            // Console.WriteLine(content);
        }

        public string Int_ToString(int value)
        {
            return value.ToString();
        }

        public string Float_ToString(float value)
        {
            return value.ToString() + "f";
        }

        public string Bool_ToString(bool value)
        {
            if (value)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public string String_ToString(string value)
        {
            return "\"" + value + "\"";
        }

        public string IntType_ToString()
        {
            return "int";
        }

        public string FloatType_ToString()
        {
            return "float";
        }

        public string BoolType_ToString()
        {
            return "bool";
        }

        public string StringType_ToString()
        {
            return "string";
        }

        public string DynamicValue_ToString(EDefineType type, object value)
        {
            switch (type)
            {
                case EDefineType.INT:
                {
                    return Int_ToString((int) value);
                }
                case EDefineType.FLOAT:
                {
                    return Float_ToString((float) value);
                }
                case EDefineType.BOOL:
                {
                    return Bool_ToString((bool) value);
                }
                case EDefineType.STRING:
                {
                    return String_ToString((string) value);
                }
            }

            throw new Exception("Unknown type");
        }

        public string DynamicType_ToString(EDefineType type)
        {
            switch (type)
            {
                case EDefineType.INT:
                {
                    return IntType_ToString();
                }
                case EDefineType.FLOAT:
                {
                    return FloatType_ToString();
                }
                case EDefineType.BOOL:
                {
                    return BoolType_ToString();
                }
                case EDefineType.STRING:
                {
                    return StringType_ToString();
                }
            }

            throw new Exception("Unknown type");
        }
    }
}