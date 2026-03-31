using Unity.VisualScripting;
using UnityEngine;

public class StatDice : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public void RollAndUpgrade()
    {
        if (player == null) return;

        if (DiceSystem.instance == null)
        {
            return;
        }

        int result = DiceSystem.instance.RollDice();

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

        Debug.Log($"주사위 결과: {result}");
    }
}
