using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
	[SerializeField] private List<SkillDataSO> _haveSkillList  = new List<SkillDataSO>();
	[SerializeField] private List<Image>       _skillImageList = new List<Image>();

	[SerializeField] private PlayerSkillHandler _skillHandler;

	[SerializeField] private Text _infoText;
	[SerializeField] private Text _statText;

	private void Start()
	{
		RefreshSkillUI();
		SetInfoText();
		SetStatText();
	}

	private void RefreshSkillUI()
	{
		var data = PlayerPersistentData.Instance?.SavedSkills;
		for (int i = 0; i < 4; i++)
		{
			if (data == null || data.Count <= i)
			{
				_skillImageList[i].gameObject.SetActive(false);
				continue;
			}

			SkillDataSO currentSO = data[i];
			_skillImageList[i].gameObject.SetActive(true);
			_skillImageList[i].sprite = currentSO.Get(1).SkillImage;
		}
	}

	private void SetInfoText()
	{
		var data = PlayerPersistentData.Instance?.SavedRuntimeData ?? new PlayerData().Clone();
		if (LanguageManager.Instance.Current == Language.Korean)
		{
			_infoText.text =
				$"최고 도달 층 수 : {1} 층\r\n최대 생존 시간 : {00: 00 : 38}\r\n처치한 적 : {1} 마리\r\n획득한 골드 : {data.CurrentGold} Gold\r\n강화 성공 횟수 : {0} 회\r\n가장 강력한 일격 : {10}";
		}
		else
		{
			_infoText.text =
				$"Highest Floor: {1}\r\nMax Survival Time: {"00:00:38"}\r\nEnemies Defeated: {1}\r\nGold Earned: {data.CurrentGold} Gold\r\nEnhancement Successes: {0}\r\nStrongest Hit: {10}";
		}
	}

	private void SetStatText()
	{
		var data = PlayerPersistentData.Instance?.SavedRuntimeData ?? new PlayerData().Clone();
		if (LanguageManager.Instance.Current == Language.Korean)
		{
			_statText.text =
				$"공격력 : {data.AtkDamage}\r\n공격 속도 : {data.AtkSpeed}\r\n이동 속도 : {data.MoveSpeed}\r\n치명타 확률 : {data.CritRate} %\r\n치명타 피해 : {data.CritDamage} %";
		}
		else
		{
			_statText.text =
				$"Attack Power: {data.AtkDamage}\r\nAttack Speed: {data.AtkSpeed}\r\nMove Speed: {data.MoveSpeed}\r\nCrit Rate: {data.CritRate} %\r\nCrit Damage: {data.CritDamage} %";
		}
	}
}