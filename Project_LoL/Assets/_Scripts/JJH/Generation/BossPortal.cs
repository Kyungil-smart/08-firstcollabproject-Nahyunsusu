using UnityEngine;

public class BossPortal : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;

    private void Awake()
    {
        if (_visuals != null) _visuals.SetActive(false);
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