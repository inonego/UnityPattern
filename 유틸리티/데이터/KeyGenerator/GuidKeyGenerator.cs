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

        public IKeyGenerator<string> Clone()
        {
            var result = @new();
            result.CloneFrom(this);
            return result;
        }

        public void CloneFrom(IKeyGenerator<string> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"GuidGenerator.CloneFrom()의 인자가 null입니다.");
            }

            // NONE
        }
    }
};