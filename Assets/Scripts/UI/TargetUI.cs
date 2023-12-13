using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class TargetUI : MonoBehaviour
{
    TMPro.TextMeshProUGUI textElement;
    // Start is called before the first frame update
    void Start()
    {
        textElement = GetComponent<TMPro.TextMeshProUGUI>();
        GameManager.Instance.OnNewLevel += OnNewLevelHandler;
        if(GameManager.Instance.CurrentLevel != null) OnNewLevelHandler(GameManager.Instance.CurrentLevel);
    }

    void OnDestroy()
    {
        GameManager.Instance.OnNewLevel -= OnNewLevelHandler;
    }

    void OnNewLevelHandler(LevelDefinition level)
    {
        textElement.text = $"Target: {level.goal}";
    }
}
