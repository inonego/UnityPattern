# 게임 오브젝트 프로바이더

## 개요

게임 오브젝트 프로바이더는 게임 오브젝트의 생성(Acquire)과 반환(Release)을 추상화하여 관리하는 시스템입니다.

- 다양한 방식으로 게임 오브젝트를 생성할 수 있는 인터페이스 제공
- 프리팹 기반 및 Addressables 기반 구현 제공
- 동기/비동기 방식 모두 지원

## 주요 클래스

### IGameObjectProvider
게임 오브젝트 프로바이더 인터페이스

### PrefabGameObjectProvider
프리팹을 이용하여 게임 오브젝트를 생성하는 프로바이더

### AddressableGameObjectProvider
Addressables를 사용하여 게임 오브젝트를 생성하는 프로바이더

## 주요 기능

### Acquire()
- 게임 오브젝트를 동기적으로 생성하여 가져옵니다
- 생성된 오브젝트는 설정된 Parent에 자동으로 배치됩니다

### AcquireAsync()
- 게임 오브젝트를 비동기적으로 생성하여 가져옵니다
- `Awaitable<GameObject>`를 반환합니다

### Release(GameObject)
- 생성된 게임 오브젝트를 반환합니다
- `PrefabGameObjectProvider`: `GameObject.Destroy()` 사용
- `AddressableGameObjectProvider`: `Addressables.ReleaseInstance()` 사용

## 속성

### Parent
- 게임 오브젝트를 생성할 부모 Transform
- null인 경우 루트에 생성됩니다

### WorldPositionStays
- 게임 오브젝트를 부모에 설정할 때 월드 위치를 유지할지 여부
- `SetParent(Transform, bool)`의 두 번째 매개변수로 전달됩니다

## 구현 클래스별 특징

### PrefabGameObjectProvider
- **프리팹**: `GameObject` 타입의 프리팹 참조
- **생성 방식**: `GameObject.Instantiate()` 사용
- **반환 방식**: `GameObject.Destroy()` 사용
- **생성자**: 프리팹, 부모, WorldPositionStays를 매개변수로 받는 생성자 제공

### AddressableGameObjectProvider
- **AssetReference**: `AssetReferenceGameObject` 타입의 Addressables 참조
- **생성 방식**: `AssetReference.InstantiateAsync()` 사용
- **반환 방식**: `Addressables.ReleaseInstance()` 사용
- **특징**: Addressables 시스템을 통한 리소스 관리

## 사용 예시

> `MonoBehaviour`나 `ScriptableObject`의 **필드로 사용할 때는**
> 반드시 `SerializeReference`가 붙은 `IGameObjectProvider` 타입으로 선언해야  
> 커스텀 에디터를 통해 프리팹/Addressables 설정 UI가 제대로 표시됩니다.

```csharp
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    // 전용 에디터를 사용하기 위해 SerializeReference + IGameObjectProvider 조합으로 선언
    [SerializeReference]
    private IGameObjectProvider provider = new PrefabGameObjectProvider();

    public void Spawn()
    {
        var gameObject = provider.Acquire();
        // ... 사용 ...
        provider.Release(gameObject);
    }
}

// 비동기 방식 사용
var asyncGameObject = await provider.AcquireAsync();
```

## 주의사항

- `PrefabGameObjectProvider`의 `Acquire()` 호출 전에 `Prefab`이 설정되어 있어야 합니다
- `AddressableGameObjectProvider`의 `Acquire()` 호출 전에 `AssetReference`가 설정되어 있어야 합니다
- `AddressableGameObjectProvider`는 Addressables 시스템을 사용하므로, 반드시 `Release()`를 호출하여 리소스를 해제해야 합니다
- `PrefabGameObjectProvider`의 `AcquireAsync()`는 `GameObject.InstantiateAsync()`를 사용하며, 배열의 첫 번째 요소를 반환합니다

