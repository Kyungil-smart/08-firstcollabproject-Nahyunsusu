using UnityEngine;

public class SkillTooltipData : TooltipComponent
{
    private SkillDataSO _skillData;

    public void Setup(SkillDataSO data)
    {
        _skillData = data;
    }

    public override string GetTooltipText()
    {
        if (_skillData == null) 
            return "정보 없음";

        var data = _skillData.Get(1);

        if (LanguageManager.Instance.currentLanguage == Language.Korean)
        {
            return $"<b>{data.SkillName}</b>";
        }
        else
        {
            return $"<b>{data.SkillName}</b>";
        }
    }
}