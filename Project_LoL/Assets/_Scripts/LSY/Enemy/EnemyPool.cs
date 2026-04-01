using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 5;
        public int spawnAmount = 3; 
    }

    public List<PoolConfig> poolConfigs;
    private Dictionary<string, Queue<GameObject>> _pools;
    private Dictionary<string, GameObject> _prefabMap;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitPools();
    }

    private void InitPools()
    {
        _pools = new Dictionary<string, Queue<GameObject>>();
        _prefabMap = new Dictionary<string, GameObject>();

        if (poolConfigs == null) return;

        foreach (var config in poolConfigs)
        {
            if (config.prefab == null) continue;
            string key = config.prefab.name;
            _pools[key] = new Queue<GameObject>();
            _prefabMap[key] = config.prefab;

            for (int i = 0; i < config.initialSize; i++)
            {
                GameObject obj = CreateNewObject(config.prefab);
                _pools[key].Enqueue(obj);
            }
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, RoomNode room)
    {
        string key = prefab.name;
        if (!_pools.ContainsKey(key))
        {
            _pools[key] = new Queue<GameObject>();
            _prefabMap[key] = prefab;
        }

        GameObject obj = (_pools[key].Count > 0) ? _pools[key].Dequeue() : CreateNewObject(prefab);

        // ★ 일반 몬스터 전용 로직
        if (obj.TryGetComponent(out EnemyFSM fsm))
        {
            fsm.ResetEnemy();
            fsm.SetRoom(room);
        }

        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
        string key = obj.name.Replace("(Clone)", "").Trim();
        if (_pools.ContainsKey(key)) _pools[key].Enqueue(obj);
        else Destroy(obj);
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = prefab.name;
        obj.SetActive(false);
        return obj;
    }
}