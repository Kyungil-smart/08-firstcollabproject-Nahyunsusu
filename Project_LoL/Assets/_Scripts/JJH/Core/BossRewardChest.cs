using UnityEngine;

public class BossRewardChest : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;

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
        if (_visuals != null) _visuals.SetActive(true);
    }
}