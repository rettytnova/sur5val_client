using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _Instance;
    public static Managers GetInstance
    {
        get { Init(); return _Instance; }
    }

    #region ServerManager
    NetworkManager _networkManager = new NetworkManager();
    #endregion

    #region ClientManager
    ResourceManagers _resourceManager = new ResourceManagers();
    #endregion

    public static NetworkManager networkManager
    {
        get { return GetInstance._networkManager; }
    }

    public static ResourceManagers resourceManager
    {
        get { return GetInstance._resourceManager; }
    }

    void Start()
    {
        Init();
    }
    
    void Update()
    {
        
    }

    static void Init()
    {
        if(_Instance == null)
        {            
            GameObject GOManagers = GameObject.Find("@Managers");
            if(GOManagers == null)
            {
                GOManagers = new GameObject { name = "@Managers" };
                GOManagers.AddComponent<Managers>();
            }

            // ��׶��忡���� ���� �ϵ��� ��
            Application.runInBackground = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            // Managers ������Ʈ�� ������� �ʵ��� ����
            DontDestroyOnLoad(GOManagers);

            _Instance = GOManagers.GetComponent<Managers>();

            _Instance._resourceManager.Init();
        }
    }
}
