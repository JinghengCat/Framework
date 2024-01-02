using System.Collections.Generic;

namespace Test
{
    public class TestCfg2Instance
    {
        public readonly int id;
        public readonly string name;
        public readonly bool isPlayer;
        public readonly float hp;
        
        
        public TestCfg2Instance(int id, string name, bool isPlayer, float hp)
        {
            this.id = id;
            this.name = name;
            this.isPlayer = isPlayer;
            this.hp = hp;
            
        }
    }
    
    public static class TestCfg2
    {
        private static Dictionary<int, TestCfg2Instance> _dictionary = new Dictionary<int, TestCfg2Instance>()
        {
            {2, new TestCfg2Instance(2, "CJHHHH", false, 1000.666f)},
            
        };
        
        public static TestCfg2Instance Get(int key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}