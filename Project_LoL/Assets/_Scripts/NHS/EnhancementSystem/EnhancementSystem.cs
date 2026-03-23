using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnhancementSystem : MonoBehaviour
{
    List<DiceSystem> DiceList = new List<DiceSystem>();

    private void Start()
    {
        DiceList.Add(new DiceSystem_Random());
    }

    public void EnhanceWeapon(Weapon targetWeapon)
    {
        if (targetWeapon == null) return;

        int sum = 0;

        foreach(var dice in DiceList)
        {
            sum += dice.RollDice();
        }

        targetWeapon.ApplyEnhancement(sum);
    }
}
