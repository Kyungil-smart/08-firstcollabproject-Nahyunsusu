using UnityEngine;

[System.Serializable]
public class NegotiationSystem
{
    [SerializeField] private float range;
    public bool SetTable()
    {
        return DiceSystem.instance.RollDice() > 3;
    }

    public int IncreasePrice(int price)
    {
        float multiplier = 1f + range;
        return Mathf.RoundToInt(price * multiplier);
    }

    public int DecreasePrice(int price)
    {
        float multiplier = 1f - range;
        return Mathf.Max(0, Mathf.RoundToInt(price * multiplier));
    }
}