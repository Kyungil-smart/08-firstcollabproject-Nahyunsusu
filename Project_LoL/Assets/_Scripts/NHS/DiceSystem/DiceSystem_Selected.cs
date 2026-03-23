using UnityEngine;

public class DiceSystem_Selected : DiceSystem
{
    [SerializeField] private int _selectedNum;
    public int selectedNum
    {
        get { return _selectedNum; }
        set { _selectedNum = value; }
    }

    public override int RollDice()
    {
        return _selectedNum;
    }
}
