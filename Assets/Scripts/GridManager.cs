using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 5;
    public int height = 5;
    public GameObject cellPrefab;
    public Transform gridParent;

    public Sprite joySprite;
    public Sprite fearSprite;
    public Sprite angerSprite;
    public Sprite sadnessSprite;
    public Sprite emptySprite;
    public Sprite hiddenSprite;
    public Sprite goalSprite;
    private Dictionary<EmotionType, Sprite> emotionSprites;

   
    public GridCell[,] grid;

    private void Awake()
    {
        Instance = this;

        // Initialize dictionary
        emotionSprites = new Dictionary<EmotionType, Sprite>
        {
            { EmotionType.Joy, joySprite },
            { EmotionType.Fear, fearSprite },
            { EmotionType.Anger, angerSprite },
            { EmotionType.Sadness, sadnessSprite },
             { EmotionType.Empty, emptySprite },
             { EmotionType.Goal, goalSprite}
        };
    }

    void Start()
    {
        grid = new GridCell[width, height];
        GenerateGrid();

        StartCoroutine(DelayedInitPlayer());
    }

    System.Collections.IEnumerator DelayedInitPlayer()
    {
        yield return null; // чекај 1 frame
        GameManager.Instance.player.InitializePosition();
    }

    void GenerateGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject cellGO = Instantiate(cellPrefab, gridParent);
                GridCell cell = cellGO.GetComponent<GridCell>();
                cell.x = x;
                cell.y = y;
                EmotionType emotion;

                if (x == 0 && y == 0)
                {
                    emotion = EmotionType.Empty;
                }
                else if (x == width - 1 && y == height - 1)
                {
                    emotion = EmotionType.Goal;
                }
                else
                {
                    emotion = GetRandomEmotion();
                }

                Sprite emotionSprite = emotionSprites[emotion];
                cell.SetEmotion(emotion, emotionSprite);


                grid[x, y] = cell;
                cell.Hide(hiddenSprite);
                if ((x == 4 && y == 4))
                {
                    emotion = EmotionType.Goal;
                    RevealCell(x,y);
                }

            }
        }
        GameManager.Instance.player.TryMove(0,0);

    }
    public void RevealCell(int x, int y)
    {
        if (!IsValidPosition(x, y)) return;

        EmotionType emotion = grid[x, y].emotion;
        Sprite sprite = emotionSprites[emotion];
        grid[x, y].Reveal(sprite);
    }
    EmotionType GetRandomEmotion()
    {
        float rand = Random.value;

        if (rand < 0.5f)
        {
            return EmotionType.Empty;
        }
        else
        {

            int index = Random.Range(0, 4);
            return (EmotionType)index;
        }
    }
    
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
    public Vector3 GetCellWorldPosition(int x, int y)
    {
        if (!IsValidPosition(x, y)) return Vector3.zero;
        return grid[x, y].transform.position; // WORLD position, not local
    }


}