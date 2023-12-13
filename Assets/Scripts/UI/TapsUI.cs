using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class TapsUI : MonoBehaviour
{
    TMPro.TextMeshProUGUI textElement;
    // Start is called before the first frame update
    void Start()
    {
        textElement = GetComponent<TMPro.TextMeshProUGUI>();
        GameManager.Instance.OnNewLivesAmount += OnNewLivesAmountHandler;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnNewScore -= OnNewLivesAmountHandler;
    }

    void OnNewLivesAmountHandler(int oldAmount, int newAmount)
    {
        textElement.text = $"Taps: {newAmount}";
    }
}
