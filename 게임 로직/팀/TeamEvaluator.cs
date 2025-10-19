using System;

namespace inonego
{
    public enum RelativeFaction
    {
        Self, Ally, Enemy, Neutral
    }

    [Serializable]
    public abstract class TeamFactionChecker
    {
        public abstract RelativeFaction Check(Entity self, Entity other);
    }
}