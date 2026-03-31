using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPool : MonoBehaviour
{
    public static SkillPool Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> _poolDict = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!_poolDict.ContainsKey(key))
        {
            _poolDict[key] = new Queue<GameObject>();
        }

        GameObject obj;
        if (_poolDict[key].Count > 0)
        {
            obj = _poolDict[key].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
            obj.name = key;
        }

        return obj;
    }

    public void Despawn(GameObject obj, float delay = 0f)
    {
        if (delay > 0)
            StartCoroutine(DespawnRoutine(obj, delay));
        else
            ReturnToPool(obj);
    }

    private IEnumerator DespawnRoutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(obj);
    }

    private void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        
        obj.SetActive(false);
        string key = obj.name;

        if (!_poolDict.ContainsKey(key))
        {
            _poolDict[key] = new Queue<GameObject>();
        }

        if (!_poolDict[key].Contains(obj))
        {
            _poolDict[key].Enqueue(obj);
        }
    }
}