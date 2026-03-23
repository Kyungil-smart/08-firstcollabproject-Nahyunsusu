using UnityEngine;

public class DiceSystem_Random : DiceSystem
{
    public override int RollDice()
    {
        int result = Random.Range(1, 7);
        return result;
    }
}
