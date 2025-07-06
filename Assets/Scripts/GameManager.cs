using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerController player;
    public AIAdvisor advisor;
    public UIManager uiManager;
    private void Awake() => Instance = this;
    public GameOverUI gameOverUI;

    public int trustLevel = 50;

    
    public void OnFearTriggered()
    {
        player.gridX = 0;
        player.gridY = 0;
        player.InitializePosition();
        trustLevel = Mathf.Min(trustLevel+ 10,100);
    }
    public void OnJoyTriggered()
    {
        player.movesRemaining += 2;
        trustLevel = Mathf.Max(trustLevel - 20, 0);
    }

    public void OnSadnessTriggered()
    {
        player.movesRemaining = Mathf.Max(0, player.movesRemaining - 2);
        trustLevel = Mathf.Min(trustLevel + 10, 100);
    }

    public void OnAngerTriggered()
    {
        player.SetExternalFreeze(true);  
        StartCoroutine(RandomMoveSequence(3));
        trustLevel = Mathf.Min(trustLevel + 10, 100);
    }


    System.Collections.IEnumerator RandomMoveSequence(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            int dx = Random.Range(-1, 2);
            int dy = Random.Range(-1, 2);
            player.gridX = Mathf.Clamp(player.gridX + dx, 0, GridManager.Instance.width - 1);
            player.gridY = Mathf.Clamp(player.gridY + dy, 0, GridManager.Instance.height - 1);
            player.InitializePosition();
            yield return new WaitForSeconds(1f);
        }

        player.SetExternalFreeze(false);  
        OnPlayerMoved();
    }

    public void OnPlayerMoved()
    {
        
            bool trustworthy = trustLevel >= 50;
            string advice = advisor.GetAdvice();
            uiManager.DisplayAdvice(advice);
        
        uiManager.UpdateUI(player.movesRemaining, trustLevel);
        if (player.movesRemaining <= 0)
        {
            uiManager.ShowGameOverPanel();
            return;
        }
    }
    public void ShowVictory()
    {
        gameOverUI.ShowWin();
    }
    public void AdjustTrust(bool trusted)
    {
        trustLevel += trusted ? 5 : -10;
        trustLevel = Mathf.Clamp(trustLevel, 0, 100);
    }
}