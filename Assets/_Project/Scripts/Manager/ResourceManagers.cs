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
            // /�� ��ġ�� ã�Ƽ� �״��������� ���� ������
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
        // ������ �̹� ������� ����̸� �ٷ� ����Ҽ� �ְ� ���ش�.
        GameObject Prefab = Load<GameObject>($"Prefabs/{resourcePath[ResourcePath]}");
        if (Prefab == null)
        {
            Debug.Log($"������ ������ ���� ( ��� : {resourcePath[ResourcePath]})");
            return null;
        }

        // Object�� ���������� �� ���̸� ��������� Instantiate �Լ��� �ѹ��� ȣ�� �Ǳ� ����
        GameObject Go = Object.Instantiate(Prefab, Parent);
        Go.name = Prefab.name;

        return Go;
    }
}
