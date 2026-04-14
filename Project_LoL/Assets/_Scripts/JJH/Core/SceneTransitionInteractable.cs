using UnityEngine;
using UnityEngine.InputSystem;

public class SceneTransitionInteractable : MonoBehaviour
{
    public enum Destination
    {
        Next,
        Tutorial,
        Lobby,
        Stage1,
        Stage2,
        Stage3,
        FinalBoss,
        Ending
    }

    [SerializeField] private Destination _destination = Destination.Next;
    [SerializeField] private bool _requireTutorialComplete = false;

    private bool _playerInRange;

    private void Start()
    {
        if (_requireTutorialComplete)
        {
            gameObject.SetActive(false);

            TutorialManager tm = FindAnyObjectByType<TutorialManager>();
            if (tm != null)
                tm.onTutorialCompleted.AddListener(OnTutorialCompleted);
        }
    }

    private void OnTutorialCompleted()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
            Transit();
    }

    public void Transit()
    {
        switch (_destination)
        {
            case Destination.Next:      SceneLoader.LoadNext(); break;
            case Destination.Tutorial:  SceneLoader.LoadTutorial(); break;
            case Destination.Lobby:     SceneLoader.LoadLobby(); break;
            case Destination.Stage1:    SceneLoader.LoadStage(1); break;
            case Destination.Stage2:    SceneLoader.LoadStage(2); break;
            case Destination.Stage3:    SceneLoader.LoadStage(3); break;
            case Destination.FinalBoss: SceneLoader.LoadFinalBoss(); break;
            case Destination.Ending:    SceneLoader.LoadEnding(); break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
    }
}