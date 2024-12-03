using UnityEngine;

public class GameScene : MonoBehaviour
{
    static GameScene _instance = null;
    public static GameScene GetInstance
    {
        get { return _instance; }
    }

    public UIGameScene gameSceneUI = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    void Init()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }
}
