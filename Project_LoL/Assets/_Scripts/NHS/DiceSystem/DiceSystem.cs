using UnityEngine;

public abstract class DiceSystem : MonoBehaviour
{
    public static DiceSystem instance { get; protected set; }

    public abstract int RollDice();

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}