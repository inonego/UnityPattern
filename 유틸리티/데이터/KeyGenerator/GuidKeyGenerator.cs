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

        public IKeyGenerator<string> @new() => new GuidGenerator();

        public void CloneFrom(IKeyGenerator<string> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"GuidGenerator.CloneFrom()의 인자가 null입니다.");
            }

            if (source is not GuidGenerator other)
            {
                throw new ArgumentException($"GuidGenerator.CloneFrom()의 인자가 GuidGenerator가 아닙니다.");
            }

            // NONE
        }
    }
};