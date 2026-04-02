using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;

    public GameObject SpawnBoss()
    {
        if (bossPrefab == null)
        {
            return null;
        }

        GameObject bossInstance = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        return bossInstance;
    }
}