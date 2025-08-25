using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI: MonoBehaviour 
{
    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    private void OnEnable()
    {
        GameManager.Instance.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        StartCoroutine(UpdateScoreNextFrame());
    }

    IEnumerator UpdateScoreNextFrame()
    {
        yield return null;

        scoreText.text = GameManager.Instance.Score.ToString();
    }
}
