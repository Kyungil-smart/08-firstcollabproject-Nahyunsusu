using System.Collections.Generic;
using UnityEngine;

// 스폰: EnemyPool.Instance.Spawn(prefab, position, room)
// room이 null이면 A* 그리드 없이 직선 이동
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

        if (poolConfigs == null || poolConfigs.Count == 0) return;

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

    public GameObject Spawn(GameObject prefab, Vector3 position, RoomNode room = null)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[EnemyPool] Spawn 실패: prefab이 null입니다.");
            return null;
        }

        string key = prefab.name;

        if (!_pools.ContainsKey(key))
        {
            _pools[key]    = new Queue<GameObject>();
            _prefabMap[key] = prefab;
        }

        return SpawnByKey(key, position, room);
    }

    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[EnemyPool] Return 실패: obj가 null입니다.");
            return;
        }

        // 이미 비활성화된 오브젝트는 중복 반환으로 간주
        if (!obj.activeSelf)
        {
            Debug.LogWarning("[EnemyPool] 이미 반환된 오브젝트입니다.");
            return;
        }

        // 색상 복구 후 비활성화 (피격 색상이 남아있는 문제 방지)
        if (obj.TryGetComponent(out EnemyEffectManager effect))
            effect.RestoreColors();

        obj.SetActive(false);

        // (Clone) 제거 후 키 매칭
        string key = obj.name.Replace("(Clone)", "").Trim();

        if (_pools.ContainsKey(key))
            _pools[key].Enqueue(obj);
        else
            Destroy(obj);
    }

    private GameObject SpawnByKey(string key, Vector3 position, RoomNode room)
    {
        GameObject obj;

        if (_pools[key].Count > 0)
        {
            obj = _pools[key].Dequeue();
        }
        else if (_prefabMap.ContainsKey(key))
        {
            obj = CreateNewObject(_prefabMap[key]);
        }
        else
        {
            Debug.LogWarning($"[EnemyPool] {key} prefab 정보가 없습니다.");
            return null;
        }

        if (obj.TryGetComponent(out EnemyFSM fsm))
        {
            fsm.ResetEnemy();
            fsm.SetRoom(room);
        }

        obj.transform.position = position;
        obj.SetActive(true);

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