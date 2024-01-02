using System.Collections.Generic;

namespace ProjectCfg.Test
{
    public class TestCfg2
    {
        public readonly int id;
        public readonly string name;
        public readonly bool isPlayer;
        public readonly float hp;
        
        
        public TestCfg2(int id, string name, bool isPlayer, float hp)
        {
            this.id = id;
            this.name = name;
            this.isPlayer = isPlayer;
            this.hp = hp;
            
        }
    }
    
    public static class TestCfg2Cfg
    {
        private static Dictionary<int, TestCfg2> _dictionary = new Dictionary<int, TestCfg2>()
        {
            {2, new TestCfg2(2, "CJHHHH", false, 1000.666f)},
            
        };
        
        public static TestCfg2 Get(int key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}