using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public DataManager equip;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            equip.Init();

            equip.LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
