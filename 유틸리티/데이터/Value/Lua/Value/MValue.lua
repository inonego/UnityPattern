---@meta Value

---@generic T
---@class IModifier<T>
---@field Modify fun(self, value: any) : any

---@class (exact) ModifierEntry
---@field Modifier IModifier @적용할 수정자입니다.
---@field Order number @수정자 적용 순서입니다.

---@class (exact) MValue : Value
---@field Modifiers ModifierEntry[] @수정자 목록입니다.
---@field Modified any @수정자가 적용된 현재 값입니다.
---@field AddModifier fun(self, key: string, modifier: IModifier, order: number?, invokeEvent: boolean?)
---@field AddModifier fun(self, modifier: IModifier, order: number?, invokeEvent: boolean?)
---@field RemoveModifier fun(self, key: string, invokeEvent: boolean?) : boolean
---@field ClearModifiers fun(self, invokeEvent: boolean?)
---@field CloneFrom fun(self, source: MValue)