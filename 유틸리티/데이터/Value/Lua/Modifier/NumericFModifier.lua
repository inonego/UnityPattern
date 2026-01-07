---@meta Value

---@enum NumericFOperation
local NumericFOperation = {
    SET = 0,
    ADD = 1,
    SUB = 2,
    MUL = 3,
    DIV = 4
}

---@class NumericFModifier : IModifier
--- # 프로퍼티
---@field Operation NumericFOperation @수치 연산 타입입니다.
---@field Value number @연산에 사용될 실수형 값입니다.
--- # 메서드
---@field Modify fun(self, value: number): number @수치 연산을 적용합니다.
---@field CloneFrom fun(self, source: NumericFModifier) @다른 수정자로부터 값을 복제합니다.