using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossSkill1 : MonoBehaviour
{
    public MonsterSkillDataSO skillData;

    public void Execute(RoomNode room, bool isLowHp)
    {
        if (room == null || skillData == null) return;
        int count = isLowHp ? Random.Range(6, 9) : Random.Range(4, 7);

        StartCoroutine(SkillProcess(room, count));
    }

    private IEnumerator SkillProcess(RoomNode room, int count)
    {
        RectInt bounds = room.GetBounds();
        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(bounds.xMin + 1f, bounds.xMax - 1f);
            float randomY = Random.Range(bounds.yMin + 1f, bounds.yMax - 1f);
            spawnPositions.Add(new Vector2(randomX, randomY));
        }

        List<GameObject> indicators = new List<GameObject>();
        foreach (var pos in spawnPositions)
        {
            if (skillData.skillPrefab != null)
            {
                GameObject obj = Instantiate(skillData.skillPrefab, pos, Quaternion.identity);
                obj.transform.localScale = new Vector3(skillData.damageRangeX, skillData.damageRangeY, 1);
                indicators.Add(obj);
            }
        }

        yield return new WaitForSeconds(skillData.warningDuration);

        foreach (var obj in indicators) Destroy(obj);

        foreach (var pos in spawnPositions)
        {
            if (skillData.mainVfxPrefab != null)
            {
                GameObject exp = Instantiate(skillData.mainVfxPrefab, pos, Quaternion.identity);
                exp.transform.localScale = Vector3.one * skillData.impactScale;

                Collider2D hit = Physics2D.OverlapBox(pos, Vector2.one * skillData.impactScale, 0, skillData.targetLayer);
                if (hit != null && hit.TryGetComponent(out Damageable player))
                {
                    player.TakeDamage(skillData.baseDamage);
                }

                Destroy(exp, skillData.impactTime);
            }
        }
    }
}