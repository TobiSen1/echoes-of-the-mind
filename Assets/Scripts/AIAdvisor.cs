using UnityEngine;
using System.Collections.Generic;

public class AIAdvisor : MonoBehaviour
{
    string[] positiveAdvice = new string[]
    {
        "Keep going.",
        "Listen to your intuition.",
        "You're doing well, don't give up.",
        "Your thoughts are on the right track.",
        "Maybe the way out is near."
    };

    string[] negativeAdvice = new string[]
    {
        "This place hides something dark...",
        "You can't always trust your own thoughts.",
        "Maybe you made a mistake.",
        "You're getting lost, aren't you?",
        "Your mind is not your ally."
    };

    string[] neutralComments = new string[]
    {
        "Something tells me this is a good idea.",
        "Your intuition can help.",
        "The decision is still yours.",
        "Sometimes you have to take risks.",
        "You can't be sure of anything."
    };

    public string GetAdvice()
    {
        int trustLevel = GameManager.Instance.trustLevel;
        bool trustworthy = DetermineTrustworthyResponse(trustLevel);

        Vector2Int currentPos = new Vector2Int(GameManager.Instance.player.gridX, GameManager.Instance.player.gridY);
        Vector2Int goalPos = new Vector2Int(GridManager.Instance.width - 1, GridManager.Instance.height - 1);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> validDirections = new List<Vector2Int>();

        // First collect all valid directions
        foreach (var dir in directions)
        {
            Vector2Int newPos = currentPos + dir;
            if (GridManager.Instance.IsValidPosition(newPos.x, newPos.y))
            {
                validDirections.Add(dir);
            }
        }

        

        Vector2Int bestMove = MinimaxMove(currentPos, goalPos, 4, true, int.MinValue, int.MaxValue);

        // If not trustworthy, select a bad move from valid directions
        if (!trustworthy)
        {
            List<Vector2Int> badOptions = new List<Vector2Int>(validDirections);
            if (badOptions.Contains(bestMove))
            {
                badOptions.Remove(bestMove);
            }

            if (badOptions.Count > 0)
            {
                bestMove = badOptions[Random.Range(0, badOptions.Count)];
            }
            // If all remaining options are equally good (only best move left), we'll use that
        }

        // Ensure the selected move is valid (should always be true at this point)
        if (!validDirections.Contains(bestMove) && validDirections.Count > 0)
        {
            bestMove = validDirections[Random.Range(0, validDirections.Count)];
        }

        return MoveToText(bestMove, currentPos, trustworthy);

    }


    bool DetermineTrustworthyResponse(int trustLevel)
    {
        float trustChance = Mathf.Lerp(0.1f, 0.9f, trustLevel / 100f);
        return Random.value < trustChance;
    }

    string MoveToText(Vector2Int move, Vector2Int currentPos, bool trustworthy)
    {
        Vector2Int targetPos = currentPos + move;

       

        string direction = move == Vector2Int.up ? "Go Down." :
                         move == Vector2Int.down ? "Go Up." :
                         move == Vector2Int.left ? "Go Left." :
                         move == Vector2Int.right ? "Go Right." :
                         "Stop.";

        string comment = trustworthy ?
            positiveAdvice[Random.Range(0, positiveAdvice.Length)] :
            negativeAdvice[Random.Range(0, negativeAdvice.Length)];

        return $"{direction}\n{comment}";
    }

    Vector2Int MinimaxMove(Vector2Int pos, Vector2Int goal, int depth, bool maximizing, int alpha, int beta)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int bestMove = Vector2Int.zero;
        int bestScore = maximizing ? int.MinValue : int.MaxValue;

        foreach (var dir in directions)
        {
            Vector2Int newPos = pos + dir;
            if (!GridManager.Instance.IsValidPosition(newPos.x, newPos.y)) continue;

            int score = Minimax(newPos, goal, depth - 1, !maximizing, alpha, beta);
            if (maximizing)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = dir;
                }
                alpha = Mathf.Max(alpha, bestScore);
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = dir;
                }
                beta = Mathf.Min(beta, bestScore);
            }

            if (beta <= alpha) break;
        }

        return bestMove;
    }

    int Minimax(Vector2Int pos, Vector2Int goal, int depth, bool maximizing, int alpha, int beta)
    {
        if (depth == 0 || pos == goal)
        {
            return EvaluatePosition(pos, goal);
        }

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        int bestScore = maximizing ? int.MinValue : int.MaxValue;

        foreach (var dir in directions)
        {
            Vector2Int newPos = pos + dir;
            if (!GridManager.Instance.IsValidPosition(newPos.x, newPos.y)) continue;

            int score = Minimax(newPos, goal, depth - 1, !maximizing, alpha, beta);

            if (maximizing)
            {
                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, bestScore);
            }
            else
            {
                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, bestScore);
            }

            if (beta <= alpha) break;
        }

        return bestScore;
    }

    int EvaluatePosition(Vector2Int pos, Vector2Int goal)
    {
        EmotionType emotion = GridManager.Instance.grid[pos.x, pos.y].emotion;
        int score = -ManhattanDistance(pos, goal);

        switch (emotion)
        {
            case EmotionType.Joy:
                score += 2;
                break;
            case EmotionType.Sadness:
                score -= 2;
                break;
            case EmotionType.Anger:
                score -= 2;
                break;
            case EmotionType.Fear:
                Vector2Int start = new Vector2Int(0, 0);
                score = -ManhattanDistance(start, goal);
                break;
        }

        return score;
    }

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}