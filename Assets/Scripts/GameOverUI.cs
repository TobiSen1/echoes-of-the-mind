using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public void ShowGameOver()
    {
        panel.SetActive(true);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exiting Game...");
    }
    public void ShowWin()
    {
        titleText.text = "YOU WIN!";
        panel.SetActive(true);
    }
}