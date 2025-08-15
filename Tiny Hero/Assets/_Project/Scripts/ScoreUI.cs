using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI: MonoBehaviour 
{
    [SerializeField] TMPro.TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        //Make sure all logic has runs before updating the score
        StartCoroutine(UpdateScoreNextFrame());
    }

    IEnumerator UpdateScoreNextFrame()
    {
        yield return null;

        scoreText.text = GameManager.Instance.Score.ToString();
    }
}
