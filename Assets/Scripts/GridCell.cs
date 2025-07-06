using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum EmotionType
{
    Joy,
    Fear,
    Anger,
    Sadness,
    Empty,
    Goal
}
public class GridCell : MonoBehaviour
{
    public Image image;
    public int x, y;
    public EmotionType emotion;
    private bool isRevealed = false;

    public TextMeshProUGUI goalLabel; // ✅ Add this

    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();

        if (goalLabel != null)
            goalLabel.gameObject.SetActive(false); // Hide it by default
    }

    public void Reveal(Sprite sprite)
    {
        if (isRevealed) return;

        image.sprite = sprite;
        image.color = Color.white;
        isRevealed = true;
    }

    public void Hide(Sprite hiddenSprite)
    {
        image.sprite = hiddenSprite;
        image.color = Color.gray;
        isRevealed = false;
    }

    public void TriggerEmotionEffect()
    {
        if (emotion == EmotionType.Empty) return;

        switch (emotion)
        {
            case EmotionType.Fear:
                GameManager.Instance.OnFearTriggered();
                break;
            case EmotionType.Joy:
                GameManager.Instance.OnJoyTriggered();
                break;
            case EmotionType.Anger:
                GameManager.Instance.OnAngerTriggered();
                break;
            case EmotionType.Sadness:
                GameManager.Instance.OnSadnessTriggered();
                break;
        }

        emotion = EmotionType.Empty;
    }
    public void SetEmotion(EmotionType emotion, Sprite sprite)
    {
        this.emotion = emotion;
        image.sprite = sprite;
        image.color = Color.white;

        if (emotion == EmotionType.Goal && goalLabel != null)
        {
            goalLabel.gameObject.SetActive(true);
            goalLabel.text = "GOAL";
        }
    }
}