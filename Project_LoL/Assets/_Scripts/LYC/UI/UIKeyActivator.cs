using UnityEngine;

public class UIKeyActivator : MonoBehaviour
{
	public void Set(bool b)
	{
		transform.GetChild(0).gameObject.SetActive(!b);
		transform.GetChild(1).gameObject.SetActive(b);
	}
}