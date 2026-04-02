using UnityEngine;

public class BossRewardChest : MonoBehaviour
{
    private void Awake()
    {
        BossDieState.OnBossDied += Activate;
    }

    private void OnDestroy()
    {
        BossDieState.OnBossDied -= Activate;
    }

    private void Activate()
    {
        gameObject.SetActive(true);
    }
}