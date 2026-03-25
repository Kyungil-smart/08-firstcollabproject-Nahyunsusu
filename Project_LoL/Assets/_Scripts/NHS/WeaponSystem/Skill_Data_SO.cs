using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Combat/Skill Data")]
public class Skill_Data_SO : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float  cooldown;
    public int    maxAmmo;
}
