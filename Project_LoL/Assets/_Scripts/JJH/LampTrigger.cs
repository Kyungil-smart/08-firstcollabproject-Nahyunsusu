using UnityEngine;

public class LampTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] _lampsOff;
    [SerializeField] private GameObject[] _lampsOn;

    private void Awake()
    {
        BossDieState.OnBossDied += TurnOn;
    }

    private void OnDestroy()
    {
        BossDieState.OnBossDied -= TurnOn;
    }

    private void TurnOn()
    {
        foreach (var lamp in _lampsOff)
            if (lamp != null) lamp.SetActive(false);

        foreach (var lamp in _lampsOn)
            if (lamp != null) lamp.SetActive(true);
    }
}