using UnityEngine;
using System.Collections.Generic;

public class DiceSystem : MonoBehaviour
{
    List<int> weights = new List<int> { 10, 10, 10, 10, 10, 10 };

    private int RollDice(bool useWeight)
    {
        if(!useWeight)
        {

        }
        rand = Random.Range(1, 6);
    }

}
