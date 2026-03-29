using System.Collections.Generic;
using UnityEngine;

public class MapObjectPool : MonoBehaviour
{
    [SerializeField] private int _defaultPrewarmCount = 0;

    private Dictionary<int, Queue<GameObject>> _poolMap
        = new Dictionary<int, Queue<GameObject>>();

    private Transform _poolRoot;

    private void Awake()
    {
        GameObject root = new GameObject("PooledObjects");
        root.transform.SetParent(transform);
        root.SetActive(false);

        _poolRoot = root.transform;
    }

    public void Prewarm(GameObject prefab, int count)
    {
        // 프리팹 체크
        Debug.Assert(prefab != null, "Prewarm prefab null");
        
        if (prefab == null || count <= 0)
            return;

        int key = prefab.GetInstanceID();
        Queue<GameObject> queue = GetOrCreateQueue(key);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = CreateNewInstance(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(_poolRoot, false);
            queue.Enqueue(obj);
        }
    }

    public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        // 풀링 대상 프리팹 null 체크
        Debug.Assert(prefab != null, "Spawn prefab null");
        
        if (prefab == null)
            return null;

        int key = prefab.GetInstanceID();
        Queue<GameObject> queue = GetOrCreateQueue(key);

        GameObject obj = null;

        while (queue.Count > 0 && obj == null)
            obj = queue.Dequeue();

        if (obj == null)
            obj = CreateNewInstance(prefab);

        Transform objTransform = obj.transform;
        objTransform.SetParent(parent, false);
        objTransform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        return obj;
    }

    public void Despawn(GameObject instance)
    {
        if (instance == null)
            return;

        PooledObject pooledObject = instance.GetComponent<PooledObject>();
        if (pooledObject == null || pooledObject.SourcePrefab == null)
        {
            Destroy(instance);
            return;
        }

        int key = pooledObject.SourcePrefab.GetInstanceID();
        Queue<GameObject> queue = GetOrCreateQueue(key);

        instance.SetActive(false);
        instance.transform.SetParent(_poolRoot, false);
        queue.Enqueue(instance);
    }

    public void ReleaseChildren(Transform root)
    {
        if (root == null)
            return;

        for (int i = root.childCount - 1; i >= 0; i--)
            Despawn(root.GetChild(i).gameObject);
    }

    private Queue<GameObject> GetOrCreateQueue(int key)
    {
        if (_poolMap.TryGetValue(key, out Queue<GameObject> queue))
            return queue;

        queue = new Queue<GameObject>();
        _poolMap[key] = queue;
        return queue;
    }

    private GameObject CreateNewInstance(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        PooledObject pooledObject = obj.GetComponent<PooledObject>();

        if (pooledObject == null)
            pooledObject = obj.AddComponent<PooledObject>();

        pooledObject.SetSourcePrefab(prefab);

        if (_defaultPrewarmCount >= 0)
            obj.transform.SetParent(_poolRoot, false);

        return obj;
    }
}