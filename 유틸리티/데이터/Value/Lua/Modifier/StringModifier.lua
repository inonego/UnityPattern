---@meta Value

---@enum StringOperation
local StringOperation = {
    SET = 0
}

---@class StringModifier : IModifier
--- # 프로퍼티
---@field Operation StringOperation @문자열 연산 타입입니다.
---@field Value string @연산에 사용될 문자열 값입니다.
--- # 메서드
---@field Modify fun(self, value: string): string @문자열 연산을 적용합니다.
---@field CloneFrom fun(self, source: StringModifier) @다른 수정자로부터 값을 복제합니다.
