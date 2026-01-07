---@meta Value

---@class (exact) RangeValue : Value
---@field Range Value
---@field Min any @최소값입니다.
---@field Max any @최대값입니다.
---@field CloneFrom fun(self, source: RangeValue)