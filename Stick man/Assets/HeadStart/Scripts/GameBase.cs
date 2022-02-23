using Assets.HeadStart.Core;
using Assets.HeadStart.Core.Player;
using Assets.HeadStart.Core.SceneService;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour, IGame
{
#pragma warning disable 0414 // private field assigned but not used.
    public static readonly string _version = "2.0.8";
#pragma warning restore 0414 //
    public Player Player;
    [HideInInspector]
    private int _uniqueId;
    private bool _isGamePaused;

    public virtual void PreStartGame()
    {
    }
    public virtual void StartGame()
    {
    }

    public virtual void GameOver()
    {
    }

    public virtual bool IsGamePaused()
    {
        return _isGamePaused;
    }

    public virtual void PauseGame()
    {
        _isGamePaused = true;
    }

    public virtual void ResumeGame()
    {
        _isGamePaused = false;
    }

    public int GetUniqueId()
    {
        _uniqueId++;
        return _uniqueId;
    }


    public void GoToMainMenu()
    {
        LoadScene(SCENE.MainMenu);
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        __.ClearSceneDependencies();
        SceneManager.LoadScene(scene.buildIndex);
    }

    internal void LoadScene(SCENE scene)
    {
        SceneReference sceneRef = __.SceneService.GetScene(scene);
        __.ClearSceneDependencies();
        sceneRef.LoadScene();
    }

}
