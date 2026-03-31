using UnityEngine;

public class DiceSystem_Random : DiceSystem
{
    public override int RollDice()
    {
        return Random.Range(1, 7);
    }
}