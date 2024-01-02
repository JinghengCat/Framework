using System.Collections.Generic;

namespace ProjectCfg.NAMESPACE
{
    public class CFG_CLASS_NAME
    {
        /*FIELD_DEFINE*/
        
        public CFG_CLASS_NAME(/*CTOR_PARAM*/)
        {
            /*CTOR*/
        }
    }
    
    public static class CFG_NAME
    {
        private static Dictionary<PRIMARY_KEY_TYPE, CFG_CLASS_NAME> _dictionary = new Dictionary<PRIMARY_KEY_TYPE, CFG_CLASS_NAME>()
        {
            /*DICT_FIELD_VALUE*/
        };
        
        public static CFG_CLASS_NAME Get(PRIMARY_KEY_TYPE key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}