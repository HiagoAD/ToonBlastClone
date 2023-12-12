using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(LevelDefinition))]
public class LevelDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        LevelDefinition def = (serializedObject.targetObject as LevelDefinition);

        if (GUILayout.Button("Generate Random Level", GUILayout.Height(40)))
        {
            def.RandomLevel();
        }

        DrawDefaultInspector();


        if (def.gridSize.x < 5) def.gridSize.x = 5;
        else if (def.gridSize.x > 9) def.gridSize.x = 9;

        if (def.gridSize.y < 5) def.gridSize.y = 5;
        else if (def.gridSize.y > 9) def.gridSize.y = 9;

        if (def.textLayout != null) def.textLayout = def.textLayout.ToUpper();

        if (GUILayout.Button("Parse Text", GUILayout.Height(40)))
        {
            def.ParseText();
        }

        GUILayout.Space(EditorGUIUtility.singleLineHeight);


        if (def.Validate())
        {
            int pixelSize = 50;
            Vector2Int textureSize = (new Vector2Int(def.gridSize.x, (def.Layout.Count / def.gridSize.x) + 1) * (pixelSize + 1)) + Vector2Int.one;
            var texture = new Texture2D(textureSize.x, textureSize.y);

            for (int i, j, n = 0; n < def.Layout.Count; n++)
            {
                i = n / def.gridSize.x;
                j = n % def.gridSize.x;

                if(i == def.gridSize.y && j == 0)
                {
                    for(int j2 = 0; j2 < def.gridSize.x; j2++)
                    {
                        texture.SetPixels(j2 * (pixelSize + 1) + 1, i * (pixelSize + 1) + 1, pixelSize, pixelSize, Enumerable.Repeat(Color.clear, pixelSize * pixelSize).ToArray());
                    }
                }

                if(i < def.gridSize.y) texture.SetPixels(j* (pixelSize + 1) + 1, i * (pixelSize + 1) + 1, pixelSize, pixelSize, Enumerable.Repeat(def.Layout[n].ToColor(), pixelSize * pixelSize).ToArray());
                else texture.SetPixels(j* (pixelSize + 1) + 1, (i + 1) * (pixelSize + 1) + 1, pixelSize, pixelSize, Enumerable.Repeat(def.Layout[n].ToColor(), pixelSize * pixelSize).ToArray());
            }

            texture.Apply();

            GUILayout.Label(texture);
        }
        else
        {
            GUILayout.Label("Invalid Level");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
