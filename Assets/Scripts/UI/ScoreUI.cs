using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class ScoreUI : MonoBehaviour
{
    TMPro.TextMeshProUGUI textElement;
    // Start is called before the first frame update
    void Start()
    {
        textElement = GetComponent<TMPro.TextMeshProUGUI>();
        GameManager.Instance.OnNewScore += OnNewScoreHandler;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnNewScore -= OnNewScoreHandler;
    }

    void OnNewScoreHandler(int oldAmount, int newAmount)
    {
        textElement.text = $"Score: {newAmount}";
    }
}
