using System;
using System.Collections.Generic;
using System.IO;

namespace CfgGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string toolFolderPath = args[0];
            string inputFolderPath = args[1];
            string csOutputFolderPath = args[2];
            
            Console.WriteLine("Tool folder path = " + toolFolderPath);
            Console.WriteLine("Input folder path = " + inputFolderPath);
            Console.WriteLine("CSharp cfg output folder path = " + csOutputFolderPath);
            try
            {
                var converter = new Sheet2CfgDataConverter();
                converter.OnInit(inputFolderPath);
                Dictionary<string, CfgData> cfgDataDictionary = converter.StartConvert();
                converter.OnRelease();
            
                string template = Utils.ReadTemplate(toolFolderPath + "/" + Setting.CS_TEMPLATE_RELATIVE_PATH);
                var writer = new CSharpCfgWriter();
                writer.SetTemplate(template);
            
                foreach (var cfgData in cfgDataDictionary.Values)
                {
                    writer.SetCfgData(cfgData);
                    writer.Write(csOutputFolderPath + "/" + cfgData.cfgName + ".cs");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Failed! 按任意键关闭");
                throw;
            }

            
            Console.WriteLine("已生成配置文件 按任意键关闭");
        }
    }
}