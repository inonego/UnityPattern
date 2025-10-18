using System;

namespace inonego
{
    public enum ReleativeFaction
    {
        Self, Ally, Enemy, Neutral
    }

    [Serializable]
    public abstract class TeamFactionChecker
    {
        public abstract ReleativeFaction Check(Entity self, Entity other);
    }
}