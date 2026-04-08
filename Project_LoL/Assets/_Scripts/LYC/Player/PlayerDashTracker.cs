using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDashTracker : MonoBehaviour
{
	public PlayerController _controller;
	public Slider           _dashSlider;

	private void Awake()
	{
		_controller = GetComponentInParent<PlayerController>();
		_dashSlider = GetComponentInChildren<Slider>();
	}

	void Start()
	{
		_dashSlider.gameObject.SetActive(false);
		_dashSlider.maxValue = 1;
	}

	public void RefreshDash()
	{
		StartCoroutine(DashRoutine());
	}

	private IEnumerator DashRoutine()
	{
		_dashSlider.value = 0;
		_dashSlider.gameObject.SetActive(true);
		float time = 0;
		while (time < _controller.Data.DashCooldown)
		{
			time              += Time.deltaTime;
			_dashSlider.value =  time / _controller.Data.DashCooldown;
			yield return null;
		}

		_dashSlider.value = 1;
		_dashSlider.gameObject.SetActive(false);
	}
}