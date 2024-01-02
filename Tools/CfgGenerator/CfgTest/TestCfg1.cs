using System.Collections.Generic;

namespace Test
{
    public class TestCfg1Instance
    {
        public readonly int id;
        public readonly string name;
        public readonly bool isPlayer;
        public readonly float hp;
        
        
        public TestCfg1Instance(int id, string name, bool isPlayer, float hp)
        {
            this.id = id;
            this.name = name;
            this.isPlayer = isPlayer;
            this.hp = hp;
            
        }
    }
    
    public static class TestCfg1
    {
        private static Dictionary<int, TestCfg1Instance> _dictionary = new Dictionary<int, TestCfg1Instance>()
        {
            {1, new TestCfg1Instance(1, "C", true, 1000.666f)},
            {14, new TestCfg1Instance(14, "C", true, 1000.666f)},
            
        };
        
        public static TestCfg1Instance Get(int key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}