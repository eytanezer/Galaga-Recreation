using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    
    
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Cheats.OnResetGame += resetGame;
        Cheats.OnQuit += QuitGame;
       
    }
    
    private void OnDisable()
    {
        Cheats.OnResetGame -= resetGame;
        Cheats.OnQuit -= QuitGame;
    }
    

    
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor
        // so we use this instead
                UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Close the game!
        Application.Quit();
        #endif
    }

  


    private void resetGame()
    {
        // Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
