using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [SerializeField] private DataManager _equip;
    public DataManager equip => _equip;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            _equip.Init();

            _equip.LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
