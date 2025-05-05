using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private AudioSource ambientAudio; 
    [SerializeField] private AudioSource monsterAudio;

    #region Hides panel at start
    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    #endregion

    void Update()
    {
        // Check for Enter key when Game Over panel is active
        if (gameOverPanel != null && gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            RetryGame();
        }
    }

    #region Shows the Game Over screen and pauses
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; //pause the game
                                    //pause audio
            if (ambientAudio != null)
            {
                ambientAudio.Pause();
            }
            if (monsterAudio != null)
            {
                monsterAudio.Pause();
            }
        }
    }
    #endregion

    //restarts the current scene
    public void RetryGame()
    {
        Time.timeScale = 1f; //resume time
        //resume audio
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
