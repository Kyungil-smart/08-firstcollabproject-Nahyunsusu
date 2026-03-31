using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene 로딩 시 Transition 효과를 관리하는 싱글톤 매니저입니다.
///
/// [동작 흐름]
///   TransitionIn 트리거 → 씬 로딩 → TransitionOut 트리거
///
/// [로컬 씬 전환]
///   SceneTransitionManager.Instance.LoadSceneWithTransition("SceneName");
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
	public static SceneTransitionManager Instance { get; private set; }

	static readonly int HashTriggerIn = Animator.StringToHash("TransitionIn");
	static readonly int HashTriggerOut = Animator.StringToHash("TransitionOut");

	Animator _animator;

	// 애니메이션 이벤트 완료 신호
	bool _inCompleted;
	bool _outCompleted;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		_animator = GetComponent<Animator>();
	}

	/// <summary>TransitionIn 클립 마지막 프레임 애니메이션 이벤트에서 호출합니다.</summary>
	public void OnTransitionInComplete()
	{
		_inCompleted = true;
	}

	/// <summary>TransitionOut 클립 마지막 프레임 애니메이션 이벤트에서 호출합니다.</summary>
	public void OnTransitionOutComplete()
	{
		_outCompleted = true;
	}

	/// <summary>
	/// TransitionIn 트리거를 실행하고 애니메이션 완료까지 대기하는 코루틴입니다.
	/// </summary>
	public IEnumerator PlayTransitionInCoroutine()
	{
		_inCompleted = false;
		_animator.SetTrigger(HashTriggerIn);
		yield return new WaitUntil(() => _inCompleted);
	}

	/// <summary>
	/// TransitionOut 트리거를 실행하고 애니메이션 완료까지 대기하는 코루틴입니다.
	/// </summary>
	public IEnumerator PlayTransitionOutCoroutine()
	{
		_outCompleted = false;
		_animator.SetTrigger(HashTriggerOut);
		yield return new WaitUntil(() => _outCompleted);
	}

	/// <summary>
	/// 트랜지션 효과와 함께 씬을 전환합니다. (로컬 씬 전환용 통합 메소드)<br/>
	/// TransitionIn → 씬 로딩 → 씬 로드 완료 → TransitionOut
	/// </summary>
	/// <param name="sceneName">전환할 씬 이름</param>
	public void LoadSceneWithTransition(string sceneName)
	{
		StartCoroutine(LoadSceneWithTransitionCoroutine(sceneName));
	}

	IEnumerator LoadSceneWithTransitionCoroutine(string sceneName)
	{
		yield return PlayTransitionInCoroutine();
		yield return SceneManager.LoadSceneAsync(sceneName);
		yield return PlayTransitionOutCoroutine();
	}
}