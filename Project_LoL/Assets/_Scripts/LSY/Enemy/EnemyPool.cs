using System.Collections.Generic;
using UnityEngine;

// 스폰 방법 두 가지:
//   1. EnemyPool.Instance.Spawn(prefab, position)     → 프리팹 직접 전달
//   2. EnemyPool.Instance.Spawn("EnemyName", position) → 이름으로 스폰 (맵 담당 연결용)
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 5;
    }

    [Header("풀 설정")]
    public List<PoolConfig> poolConfigs;

    private Dictionary<string, Queue<GameObject>> _pools;

    private Dictionary<string, GameObject> _prefabMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitPools();
    }

    private void InitPools()
    {
        _pools     = new Dictionary<string, Queue<GameObject>>();
        _prefabMap = new Dictionary<string, GameObject>();

        foreach (var config in poolConfigs)
        {
            if (config.prefab == null) continue;

            string key = config.prefab.name;
            _pools[key]    = new Queue<GameObject>();
            _prefabMap[key] = config.prefab;

            for (int i = 0; i < config.initialSize; i++)
            {
                GameObject obj = CreateNewObject(config.prefab);
                _pools[key].Enqueue(obj);
            }
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position)
    {
        string key = prefab.name;

        if (!_pools.ContainsKey(key))
        {
            _pools[key]    = new Queue<GameObject>();
            _prefabMap[key] = prefab;
        }

        return SpawnByKey(key, position);
    }

    public GameObject Spawn(string enemyName, Vector3 position)
    {
        if (!_prefabMap.ContainsKey(enemyName))
        {
            Debug.LogWarning($"[EnemyPool] {enemyName} 을 찾을 수 없습니다. 인스펙터에서 등록해주세요.");
            return null;
        }

        return SpawnByKey(enemyName, position);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);

        string key = obj.name.Replace("(Clone)", "").Trim();

        if (_pools.ContainsKey(key))
            _pools[key].Enqueue(obj);
        else
            Destroy(obj);
    }

    private GameObject SpawnByKey(string key, Vector3 position)
    {
        GameObject obj;

        if (_pools[key].Count > 0)
            obj = _pools[key].Dequeue();
        else
            obj = CreateNewObject(_prefabMap[key]);

        obj.transform.position = position;
        obj.SetActive(true);

        if (obj.TryGetComponent(out EnemyFSM fsm))
            fsm.ResetEnemy();

        return obj;
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = prefab.name;
        obj.SetActive(false);
        return obj;
    }
}