using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public int gridX = 0;
    public int gridY = 0;
    public int movesRemaining;
    public float moveSpeed = 5f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    private bool isTemporarilyFrozen = false;  // Optional: use if you want timed freeze
    private bool isExternallyFrozen = false;   // Used for Anger

    void Start()
    {
        InitializePosition();
    }

    public void InitializePosition()
    {
        RectTransform playerRect = GetComponent<RectTransform>();
        RectTransform cellRect = GridManager.Instance.grid[gridX, gridY].GetComponent<RectTransform>();

        playerRect.anchoredPosition = cellRect.anchoredPosition;
        targetPosition = transform.position;
        isMoving = false;

        GridManager.Instance.RevealCell(gridX, gridY);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }

        // Block movement if any freeze is active
        if (movesRemaining <= 0 || isMoving || isTemporarilyFrozen || isExternallyFrozen) return;

        if (Input.GetKeyDown(KeyCode.W)) TryMove(0, -1);
        else if (Input.GetKeyDown(KeyCode.S)) TryMove(0, 1);
        else if (Input.GetKeyDown(KeyCode.A)) TryMove(-1, 0);
        else if (Input.GetKeyDown(KeyCode.D)) TryMove(1, 0);
    }

    public void TryMove(int dx, int dy)
    {
        int newX = gridX + dx;
        int newY = gridY + dy;

        if (GridManager.Instance.IsValidPosition(newX, newY))
        {
            gridX = newX;
            gridY = newY;
            targetPosition = GridManager.Instance.GetCellWorldPosition(gridX, gridY);
            isMoving = true;
            movesRemaining--;

            GridManager.Instance.RevealCell(gridX, gridY);
            GridManager.Instance.grid[gridX, gridY].TriggerEmotionEffect();
            GameManager.Instance.OnPlayerMoved();
        }
        if (gridX == GridManager.Instance.width - 1 && gridY == GridManager.Instance.height - 1)
        {
            GameManager.Instance.ShowVictory();
            return;
        }
    }

    // === Freeze Control ===

    public void SetExternalFreeze(bool value)
    {
        isExternallyFrozen = value;
    }

    public void FreezeTemporarily(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private System.Collections.IEnumerator FreezeCoroutine(float duration)
    {
        isTemporarilyFrozen = true;
        yield return new WaitForSeconds(duration);
        isTemporarilyFrozen = false;
    }
}
