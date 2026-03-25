using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnhancementSystem : MonoBehaviour
{
    private bool _isEven; // 짝수이면 True
    private bool _isActionIsEven = false;

    public void EnhanceWeapon(Weapon targetWeapon)
    {
        if (targetWeapon == null) return;

        if (!_isActionIsEven) return;

        int sum = RollEnhanceDice();
        bool isSumEven = (sum % 2 == 0);

        if(_isEven == isSumEven)
        {
            targetWeapon.ApplyEnhancement(sum);
        }
        else
        {
        }

        _isActionIsEven = false;
    }

    private int RollEnhanceDice()
    {
        DiceSystem Dice = new DiceSystem_Random();

        int sum = 0;

        for (int i = 0; i < 3; i++)
        {
            sum += Dice.RollDice();
        }


        return sum;
    }

    public void SelectPrediction(bool isEven)
    {
        _isEven = isEven;
        _isActionIsEven = true;

        if (_isEven)
            Debug.Log("시스템: [짝수] 예측 선택!");
        else
            Debug.Log("시스템: [홀수] 예측 선택!");
    }
}
