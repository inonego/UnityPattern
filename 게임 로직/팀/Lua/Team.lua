---@meta Team

---@enum TeamRelativeFaction
local TeamRelativeFaction = {
    Me = 0,
    Ally = 1,
    Enemy = 2,
    Neutral = -1,
}

---@enum TeamRelativeFactionGroup
local TeamRelativeFactionGroup = {
    Me = 0,
    Ally = 1,
    All = 2,
    AllyNotMe = 3,
    AllNotMe = 4,
    Neutral = 5,
    Enemy = 6,
}