using System.Collections.Generic;

namespace ProjectCfg.Test
{
    public class TestCfg1
    {
        public readonly int id;
        public readonly string name;
        public readonly bool isPlayer;
        public readonly float hp;
        
        
        public TestCfg1(int id, string name, bool isPlayer, float hp)
        {
            this.id = id;
            this.name = name;
            this.isPlayer = isPlayer;
            this.hp = hp;
            
        }
    }
    
    public static class TestCfg1Cfg
    {
        private static Dictionary<int, TestCfg1> _dictionary = new Dictionary<int, TestCfg1>()
        {
            {1, new TestCfg1(1, "C", true, 1000.666f)},
            {14, new TestCfg1(14, "C", true, 1000.666f)},
            
        };
        
        public static TestCfg1 Get(int key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}