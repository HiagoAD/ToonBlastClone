using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarMeter : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] GameObject starMeterPrefab;
    [SerializeField] RectTransform starMeterParent;

    readonly List<GameObject> starMeterInstances = new();

    void Start()
    {
        slider.minValue = 0;
        GameManager.Instance.OnNewLevel += OnNewLevelHandler;
        GameManager.Instance.OnNewScore += OnNewScoreHandler;
        if(GameManager.Instance.CurrentLevel != null) OnNewLevelHandler(GameManager.Instance.CurrentLevel);
    }

    void OnDestroy()
    {
        GameManager.Instance.OnNewLevel -= OnNewLevelHandler;
        GameManager.Instance.OnNewScore -= OnNewScoreHandler;

    }

    // This handles the possibility of levels with any amount of stars.
    // The extra instances are destroyed instead of reutilized because is assumed that levels with different star amounts will be the exception, not the norm, far apart, and will take long enought for the extra memory to be used somewhere else 
    private void OnNewLevelHandler(LevelDefinition level)
    {
        int maxStarsScore = level.scorePerStar[^1];
        slider.maxValue = maxStarsScore;
        int i = 0;
        for(GameObject starMeterInstance; i < level.scorePerStar.Length; i++)
        {
            if(i >= starMeterInstances.Count)
            {
                starMeterInstances.Add(Instantiate(starMeterPrefab, starMeterParent));
            }
            starMeterInstance = starMeterInstances[i];
            float positionPercentage = level.scorePerStar[i] / (float)maxStarsScore;
            starMeterInstance.transform.localPosition = new Vector3(starMeterParent.rect.width * (positionPercentage - 0.5f), 0, 0);
        }

        for(int extraInstances = starMeterInstances.Count - i; extraInstances > 0; extraInstances--)
        {
            var extraInstance = starMeterInstances[^1];
            Destroy(extraInstance);
            starMeterInstances.Remove(extraInstance);
        }            
    }

    private void OnNewScoreHandler(int oldAmount, int newAmount)
    {
        slider.value = (float)newAmount;
    }
}
