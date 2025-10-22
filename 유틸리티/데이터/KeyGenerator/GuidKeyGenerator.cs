using System;

namespace inonego
{
    [Serializable]
    public class GuidGenerator : IKeyGenerator<string>
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
};