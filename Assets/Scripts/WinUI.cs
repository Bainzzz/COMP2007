using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private AudioSource ambientAudio; 
    [SerializeField] private AudioSource monsterAudio; 

    void Start()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (winPanel != null && winPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            RetryGame();
        }
    }

    public void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f; 
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

    public void RetryGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}