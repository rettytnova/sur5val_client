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

    public static NetworkManager networkManager
    {
        get {  return GetInstance._networkManager; }
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

            // 백그라운드에서도 실행 하도록 함
            Application.runInBackground = true;

            // Managers 오브젝트가 사라지지 않도록 해줌
            DontDestroyOnLoad(GOManagers);

            _Instance = GOManagers.GetComponent<Managers>();            
        }
    }
}
