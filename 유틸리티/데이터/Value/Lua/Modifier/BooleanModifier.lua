---@meta Value

---@enum BooleanOperation
local BooleanOperation = {
    SET = 0,
    AND = 1,
    OR = 2,
    XOR = 3
}

---@class BooleanModifier : IModifier
--- # 필드
---@field NOT BooleanModifier @static NOT 연산을 수행하는 수정자입니다.
--- # 프로퍼티
---@field Operation BooleanOperation @논리 연산 타입입니다.
---@field Value boolean @연산에 사용될 값입니다.
--- # 메서드
---@field Modify fun(self, value: boolean): boolean @논리 연산을 적용합니다.
---@field CloneFrom fun(self, source: BooleanModifier) @다른 수정자로부터 값을 복제합니다.
