using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ResourceManagers
{
    Dictionary<Define.en_ResourceName, string> resourcePath = new Dictionary<Define.en_ResourceName, string>();

    public void Init()
    {
        resourcePath.Add(Define.en_ResourceName.RESOURCE_UI_GLOBAL_MESSAGE, "UIGlobalMessage");        
        resourcePath.Add(Define.en_ResourceName.RESOURCE_GENERIC_DEATH, "GenericDeath");        
    }

    public T Load<T>(string Path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            // /Knight
            // /의 위치를 찾아서 그다음부터의 값을 가져옴
            string Name = Path;
            int Index = Name.LastIndexOf('/');
            if (Index >= 0)
            {
                Name = Name.Substring(Index + 1);
            }
        }

        return Resources.Load<T>(Path);
    }

    public T[] LoadAll<T>(string Path) where T : Object
    {
        return Resources.LoadAll<T>(Path);
    }

    public GameObject Instantiate(en_ResourceName ResourcePath, Transform Parent = null)
    {
        // 기존에 이미 만들었던 대상이면 바로 사용할수 있게 해준다.
        GameObject Prefab = Load<GameObject>($"Prefabs/{resourcePath[ResourcePath]}");
        if (Prefab == null)
        {
            Debug.Log($"프리팹 생성에 실패 ( 경로 : {resourcePath[ResourcePath]})");
            return null;
        }

        // Object를 붙인이유는 안 붙이면 재귀적으로 Instantiate 함수가 한번더 호출 되기 때문
        GameObject Go = Object.Instantiate(Prefab, Parent);
        Go.name = Prefab.name;

        return Go;
    }
}
