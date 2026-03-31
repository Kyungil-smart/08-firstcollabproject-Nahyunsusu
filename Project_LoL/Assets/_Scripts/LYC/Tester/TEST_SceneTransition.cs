using UnityEngine;
using UnityEngine.SceneManagement;

public class TEST_SceneTransition : MonoBehaviour
{
	[ContextMenu("Change")]
	public void TryChangeScene()
	{
		string name = SceneManager.GetActiveScene().name;
		SceneTransitionManager.Instance.LoadSceneWithTransition(name);
	}
}