using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private GameObject _cutsceneUI;
    [SerializeField] private GameObject _skipUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_cutsceneUI != null) _cutsceneUI.SetActive(false);
    }

    public IEnumerator PlayCutscene(VideoClip clip)
    {
        _cutsceneUI.SetActive(true);

        _videoPlayer.clip = clip;
        _videoPlayer.Play();

        while (_videoPlayer.isPlaying)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || 
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _videoPlayer.Stop();
                break;
            }
            yield return null;
        }

        _cutsceneUI.SetActive(false);
    }
}