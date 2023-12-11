using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "level_x", menuName = "Levels/Level Asset")]
public class LevelDefinition : ScriptableObject
{
    public Vector2Int gridSize;
    [Multiline(20)] public string textLayout;

    private List<Tile.Type> _layout;
    public List<Tile.Type> layout
    {
        get
        {
            if (_layout == null) ParseText();
            return _layout;
        }
    }

    public void RandomLevel()
    {
        var rnd = new System.Random();
        gridSize = new Vector2Int(rnd.Next(5) + 5, rnd.Next(5) + 5);
        int extraHeight = rnd.Next(gridSize.y + 2);

        textLayout = "";
        string possibilities = "RGBY";
        for (int j = 0; j < gridSize.y + extraHeight; j++)
        {
            if (j == extraHeight)
            {
                textLayout += '\n';
                for (int i = 0; i < gridSize.x; i++)
                {
                    textLayout += '-';
                }
            }

            if (j > 0) textLayout += '\n';
            for (int i = 0; i < gridSize.x; i++)
            {
                textLayout += possibilities[rnd.Next(possibilities.Length)];
            }
        }
    }

    public void ParseText()
    {
        if (textLayout == null || textLayout == "") return;

        _layout = new List<Tile.Type>();
        var _temp = new List<Tile.Type>();
        char c;
        for (int n = 0; n < textLayout.Length; n++)
        {
            c = textLayout[n];
            switch (c)
            {
                case 'R':
                   _temp.Add(Tile.Type.RED);
                    break;
                case 'G':
                   _temp.Add(Tile.Type.GREEN);
                    break;
                case 'B':
                   _temp.Add(Tile.Type.BLUE);
                    break;
                case 'Y':
                   _temp.Add(Tile.Type.YELLOW);
                    break;
                case '-':
                    break;
                case '\n':
                    _layout.InsertRange(0, _temp);
                    _temp.Clear();
                    break;
                default:
                    Debug.LogError($"PARSING ERROR - INVALID CHARACTER {c} AT {n}");
                    return;
            }
        }
        _layout.InsertRange(0, _temp);
        DumpMap();

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    void DumpMap()
    {
        var txt = "";
        foreach(var item in _layout)
        {
            txt += item;
        }
        Debug.Log(txt);
    }

    public bool Validate()
    {
        return layout.Count % gridSize.x == 0 && layout.Count / gridSize.x >= gridSize.y;
    }
}
