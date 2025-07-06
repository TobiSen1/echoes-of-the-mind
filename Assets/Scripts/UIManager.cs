using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    

    public TextMeshProUGUI movesText;
    public TextMeshProUGUI trustText;
    public TextMeshProUGUI adviceText;
    public Slider moveProgressBar;
    public GameObject gameOverPanel;

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
    public void UpdateUI(int moves, int trust)
    {
        movesText.text = "Mental Energy: " + moves;
        trustText.text = "Trust Level: " + trust;
        moveProgressBar.value = moves;
    }

    public void DisplayAdvice(string advice)
    {
        adviceText.text = "Voice in your head: " + advice;
    }
}