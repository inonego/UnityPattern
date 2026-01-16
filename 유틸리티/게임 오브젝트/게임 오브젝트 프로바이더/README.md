# 게임 오브젝트 프로바이더 (GameObject Provider)

- 게임 오브젝트의 생성(Acquire)과 반환(Release) 방식을 추상화하여 관리하는 시스템입니다.
- 프리팹 기반의 단순 생성부터 Addressables 기반의 비동기 생성까지 동일한 인터페이스로 처리할 수 있습니다.

---

### 🏗️ 핵심 클래스 (Core Classes)

- **[IGameObjectProvider]**: 객체 생성 및 반환을 담당하는 최상위 인터페이스입니다.
- **[PrefabGameObjectProvider]**: 일반 프리팹(`GameObject`)을 사용하여 `Instantiate`/`Destroy` 방식으로 동작합니다.
- **[AddressableGameObjectProvider]**: Addressables 시스템의 `AssetReference`를 사용하여 자원을 로드하고 해제합니다.

---

### ⚙️ 주요 기능 및 속성 (Key Features & Properties)

#### 메서드 (Methods)
- **`Acquire()`**: 객체를 동기적으로 생성하여 가져옵니다. 부모(`Parent`) 설정이 자동으로 수행됩니다.
- **`AcquireAsync()`**: `Awaitable<GameObject>`를 반환하는 비동기 생성 메서드입니다.
- **`Release(GameObject)`**: 생성된 객체를 각 프로바이더 방식에 맞게 안전하게 반환/파괴합니다.

#### 속성 (Properties)
- **`Parent`**: 객체가 생성될 부모 Transform입니다. (`null`이면 루트)
- **`WorldPositionStays`**: 부모 설정 시 월드 위치를 유지할지 여부를 결정합니다.

---

### 📝 사용 예시 (Examples)

> 💡 **중요**: 인스펙터에서 설정을 노출하려면 반드시 `[SerializeReference]`를 사용해야 합니다.

```csharp
public class BulletSpawner : MonoBehaviour
{
    [SerializeReference]
    private IGameObjectProvider provider = new PrefabGameObjectProvider();

    public async void Spawn()
    {
        // 동기 방식
        var obj = provider.Acquire();
        
        // 비동기 방식
        var asyncObj = await provider.AcquireAsync();
        
        provider.Release(obj);
    }
}
```

---

### ⚠️ 주의사항 (Cautions)

- **사전 설정**: `Acquire` 호출 전 반드시 프리팹이나 AssetReference가 할당되어 있어야 합니다.
- **Addressables 관리**: `AddressableGameObjectProvider` 사용 시, 메모리 누수를 방지하기 위해 반드시 `Release()`를 호출하여 자원을 해제해야 합니다.
- **비동기 인스턴스**: `PrefabGameObjectProvider`의 `AcquireAsync`는 내부적으로 `InstantiateAsync`를 사용합니다.
