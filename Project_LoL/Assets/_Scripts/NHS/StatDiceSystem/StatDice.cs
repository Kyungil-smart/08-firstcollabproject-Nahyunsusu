using Unity.VisualScripting;
using UnityEngine;

public class StatDice : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private DiceSystem _diceSystem;
    private void Awake()
    {
        _diceSystem = new DiceSystem_Random();
    }

    public void RollAndUpgrade()
    {
        if (player == null) return;

        int result = _diceSystem.RollDice();

        switch (result)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
    }
}
