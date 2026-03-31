using System.Collections;
using _Scripts.LYC.Skill;
using UnityEngine;

public class StreetDiceHitbox : SkillHitbox
{
	protected override IEnumerator DestroyAfterDuration()
	{
		BoxCollider2D col = GetComponent<BoxCollider2D>();
		YieldInstruction y = new WaitForSeconds(0.1f);

		for (int i = 0; i < 3; i++)
		{
			yield return y;
			if (col) col.enabled = false;
			if (_effect)
			{
				_effect.Stop();
				_effect.gameObject.SetActive(false);
			}

			yield return y;
			if (col) col.enabled = true;
			if (_effect)
			{
				_effect.gameObject.SetActive(true);
				_effect.Play();
			}
		}

		_effect.Stop();

		DestroyHitbox();
	}
}