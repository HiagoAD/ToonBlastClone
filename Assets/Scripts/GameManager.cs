using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public delegate void NewLevelEventHandler(LevelDefinition level);
    public delegate void NewScoreEventHandler(int oldScore, int newScore);
    public delegate void NewLivesAmountEventHandler(int oldAmount, int newAmount);

    public event NewLevelEventHandler OnNewLevel;
    public event NewScoreEventHandler OnNewScore;
    public event NewLivesAmountEventHandler OnNewLivesAmount;


    [SerializeField] TileGrid gridPrefab;
    [SerializeField] GameConfigs configs;

    public float AnimationSpeed { get { return configs.animationSpeed; } }
    public float TileSize { get { return configs.tileSize; } }
    public Vector2 Padding { get { return configs.padding; } }

    public LevelDefinition CurrentLevel { get; private set; }
    int lives;
    int score;
    TileGrid tileGridInstance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        tileGridInstance = Instantiate(gridPrefab);
        tileGridInstance.transform.position = Vector3.zero;

        LoadLevel(0);
    }

    public void OnTilesRemoved(int tilesAmount)
    {
        int oldLives = lives;
        lives--;
        OnNewLivesAmount?.Invoke(oldLives, lives);

        int oldScore = score;
        score += tilesAmount;
        OnNewScore?.Invoke(oldScore, score);
    }

    void LoadLevel(int index)
    {
        CurrentLevel = configs.levels[index];
        lives = CurrentLevel.lives;
        score = 0;

        OnNewLevel?.Invoke(CurrentLevel);
        
        tileGridInstance.Init(CurrentLevel);
    }
}