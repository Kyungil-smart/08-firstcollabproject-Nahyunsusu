using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public string sceneName;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			SceneTransitionManager.Instance.LoadSceneWithTransition(sceneName);
		}
	}
}