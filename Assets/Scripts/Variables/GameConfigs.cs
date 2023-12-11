using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Configs", menuName = "Game/Game Configs")]
public class GameConfigs : ScriptableObject
{
    public LevelDefinition[] levels;
    public float animationSpeed;
    public float tileSize;
    public Vector2 padding;

}
