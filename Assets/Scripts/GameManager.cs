using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] TileGrid gridPrefab;
    [SerializeField] GameConfigs configs;

    public float AnimationSpeed { get { return configs.animationSpeed; } }
    public float TileSize { get { return configs.tileSize; } }
    public Vector2 Padding { get { return configs.padding; } }

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
        TileGrid instance = Instantiate(gridPrefab);
        instance.transform.position = Vector3.zero;

        var level = LoadLevel(0);
        instance.Init(level);
    }

    LevelDefinition LoadLevel(int index)
    {
        return configs.levels[index];
    }
}