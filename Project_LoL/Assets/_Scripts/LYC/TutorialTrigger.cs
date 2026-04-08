using TMPro;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour, IInteract
{
	public TutorialManager tutorialManager;
	public TutorialDodger  tutorialDodger;
	public TextMeshProUGUI text;

	public void OnInteracted()
	{
		text.text = "";

		tutorialManager.StopTutorial();
		tutorialDodger.gameObject.SetActive(false);
	}
}