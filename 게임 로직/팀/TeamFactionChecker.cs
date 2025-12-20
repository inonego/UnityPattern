using System;

namespace inonego
{
    //============================================================
    /// <summary>
    /// 팀 상대 진영 관계를 나타냅니다.
    /// </summary>
    //============================================================
    public enum TeamRelativeFaction : int
    {
        Me, Ally, Enemy, Neutral = -1
    }

    //============================================================
    /// <summary>
    /// 팀 상대 진영 관계(그룹)를 나타냅니다.
    /// </summary>
    //============================================================
    public enum TeamRelativeFactionGroup
    {
        Me,         // 자신
        Ally,       // 아군
        All,        // 모두
        AllyNotMe,  // 아군 (자신 제외)
        AllNotMe,   // 모두 (자신 제외)
        Neutral,    // 중립
        Enemy,      // 적군
    }

    [Serializable]
    public abstract class TeamFactionChecker
    {
        public abstract TeamRelativeFaction Check(Entity self, Entity other);
        public abstract TeamRelativeFaction Check(int self, int other);

        public abstract bool CheckIsInGroup(Entity self, Entity other, TeamRelativeFactionGroup group);
    }
}