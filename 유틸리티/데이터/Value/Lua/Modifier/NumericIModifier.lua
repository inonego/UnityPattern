---@meta Value

---@enum NumericIOperation
local NumericIOperation = {
    SET = 0,
    ADD = 1,
    SUB = 2,
    MUL = 3,
    DIV = 4
}

---@class NumericIModifier : IModifier
--- # 프로퍼티
---@field Operation NumericIOperation @수치 연산 타입입니다.
---@field Value integer @연산에 사용될 정수형 값입니다.
--- # 메서드
---@field Modify fun(self, value: integer): integer @수치 연산을 적용합니다.
---@field CloneFrom fun(self, source: NumericIModifier) @다른 수정자로부터 값을 복제합니다.
