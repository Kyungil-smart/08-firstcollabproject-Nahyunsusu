using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeDuration = 1f;

    [Header("씬 BGM")]
    [SerializeField] private AudioClip _lobbyBGM;
    [SerializeField] private AudioClip _tutorialBGM;
    [SerializeField] private AudioClip _stage1BGM;
    [SerializeField] private AudioClip _stage2BGM;
    [SerializeField] private AudioClip _stage3BGM;
    [SerializeField] private AudioClip _finalBossBGM;

    [Header("보스 BGM")]
    [SerializeField] private AudioClip _boss1BGM;
    [SerializeField] private AudioClip _boss2BGM;
    [SerializeField] private AudioClip _boss3BGM;

    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clip = GetSceneBGM(scene.name);
        if (clip != null)
            FadeTo(clip);
        else
            FadeOut();
    }

    public void PlayBossBGM(int stageIndex)
    {
        AudioClip clip = stageIndex switch
        {
            1 => _boss1BGM,
            2 => _boss2BGM,
            3 => _boss3BGM,
            _ => null
        };

        if (clip != null)
            FadeTo(clip);
    }

    public IEnumerator FadeOutCoroutine()
    {
        yield return StartCoroutine(FadeCoroutine(0f));
    }

    public IEnumerator FadeInCoroutine()
    {
        yield return StartCoroutine(FadeCoroutine(_audioSource.volume == 0f ? 1f : _audioSource.volume));
    }

    public void FadeTo(AudioClip clip)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeToCoroutine(clip));
    }

    public void FadeOut()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeCoroutine(0f));
    }

    private IEnumerator FadeToCoroutine(AudioClip clip)
    {
        yield return FadeCoroutine(0f);

        _audioSource.clip = clip;
        _audioSource.Play();

        yield return FadeCoroutine(1f);
    }

    private IEnumerator FadeCoroutine(float targetVolume)
    {
        float startVolume = _audioSource.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / _fadeDuration);
            yield return null;
        }

        _audioSource.volume = targetVolume;
    }

    private AudioClip GetSceneBGM(string sceneName) => sceneName switch
    {
        "Lobby"      => _lobbyBGM,
        "Tutorial"   => _tutorialBGM,
        "Stage1"     => _stage1BGM,
        "Stage2"     => _stage2BGM,
        "Stage3"     => _stage3BGM,
        "FinalBoss"  => _finalBossBGM,
        _            => null
    };
}