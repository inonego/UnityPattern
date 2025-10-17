using System;

namespace inonego
{
    public enum ReleativeFaction
    {
        Ally, Enemy, Neutral
    }

    [Serializable]
    public abstract class TeamFactionChecker
    {
        public abstract ReleativeFaction Check(Team self, Team other);
    }
}