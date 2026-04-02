using UnityEngine;

public static class SceneLoader
{
    private static class SceneName
    {
        public const string MainMenu = "MainMenu";
        public const string Lobby = "Lobby";
        public const string Stage1 = "Stage1";
        public const string Stage2 = "Stage2";
        public const string Stage3 = "Stage3";
        public const string FinalBoss = "FinalBoss";
        public const string Ending = "Ending";
    }

    public static void LoadMainMenu()
    {
        SceneTransitionManager.Instance.LoadSceneWithTransition(SceneName.MainMenu);
    }

    public static void LoadLobby()
    {
        SceneTransitionManager.Instance.LoadSceneWithTransition(SceneName.Lobby);
    }

    public static void LoadStage(int stage)
    {
        string name = stage switch
        {
            1 => SceneName.Stage1,
            2 => SceneName.Stage2,
            3 => SceneName.Stage3,
            _ => SceneName.Stage1
        };
        SceneTransitionManager.Instance.LoadSceneWithTransition(name);
    }

    public static void LoadFinalBoss()
    {
        SceneTransitionManager.Instance.LoadSceneWithTransition(SceneName.FinalBoss);
    }

    public static void LoadEnding()
    {
        SceneTransitionManager.Instance.LoadSceneWithTransition(SceneName.Ending);
    }
}