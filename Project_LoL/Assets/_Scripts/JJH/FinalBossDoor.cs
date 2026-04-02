using UnityEngine;

public class FinalBossDoor : MonoBehaviour
{
    [SerializeField] private GameObject _collider;
    [SerializeField] private GameObject[] _visuals;

    private void Awake()
    {
        SetDoorState(false);
        // FinalBossFSM.OnBattleStarted += Close;
    }

    private void OnDestroy()
    {
        // FinalBossFSM.OnBattleStarted -= Close;
    }

    public void Close()
    {
        SetDoorState(true);
    }

    public void Open()
    {
        SetDoorState(false);
    }

    private void SetDoorState(bool active)
    {
        if (_collider != null) _collider.SetActive(active);
        foreach (var visual in _visuals)
            if (visual != null) visual.SetActive(active);
    }
}