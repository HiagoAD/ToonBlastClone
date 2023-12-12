using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;
using UnityEditor.Overlays;
using UnityEditor;

public class TileGrid : MonoBehaviour
{
    public delegate Vector3 FromTilePositionToLocalPositionConverter(Vector2Int position);

    [SerializeField] Tile[] tilesPrefabs;
    [SerializeField] SpriteMask mask;

    float tileSize => GameManager.Instance.TileSize;
    Vector2 padding => GameManager.Instance.Padding;


    Vector2Int visibleGridSize;
    Tile[] grid;
    Dictionary<Tile.Type, Tile> prefabCache = new Dictionary<Tile.Type, Tile>();
    Dictionary<Tile.Type, Stack<Tile>> availablePool = new();
    System.Random rnd = new();

    HashSet<Tile> movingTiles = new();

    public void Init(LevelDefinition def)
    {
        var layout = def.Layout;
        visibleGridSize = def.gridSize;

        mask.transform.localScale = new Vector3(visibleGridSize.x * (tileSize + padding.x), visibleGridSize.y * (tileSize + padding.y), 0);
        Camera.main.orthographicSize = visibleGridSize.x + 1;

        grid = new Tile[Math.Max(layout.Count, visibleGridSize.x * visibleGridSize.y * 2)]; // The grid will always have, at least, twice the visible size instantiated. If the layout is bigger, will use the layout's size.

        for (int i = 0; i < tilesPrefabs.Length; i++)
        {
            prefabCache[tilesPrefabs[i].type] = tilesPrefabs[i];
            availablePool[tilesPrefabs[i].type] = new();
        }

        for (int i, j, n = 0; n < grid.Length; n++)
        {
            i = n / def.gridSize.x;
            j = n % def.gridSize.x;

            Tile instance = n < layout.Count ? Spawn(layout[n]) : Spawn(); // If there is a layout, use it, if not, grab a random one

            instance.Init();
            instance.GoTo(j, visibleGridSize.y + i, true);
            instance.GoTo(j, i);

            if (i >= visibleGridSize.y) instance.gameObject.SetActive(false);

            grid[n] = instance;
        }
    }

    public void SetMovingTile(Tile tile)
    {
        movingTiles.Add(tile);
    }

    public void RemoveMovingTile(Tile tile)
    {
        movingTiles.Remove(tile);
    }

    private void OnTileClicked(Tile tile)
    {
        if (movingTiles.Count > 0) return;

        ReSpawn(KillNeibourTiles(tile));

        string msg = "";
        int amount = 0;
        foreach (var entry in availablePool)
        {
            msg += $"{entry.Key} - {entry.Value.Count} /// ";
            amount += entry.Value.Count;
        }
        msg += $"Total: {grid.Length + amount}";
        Debug.Log(msg);
    }


    /// <summary>
    /// Starting from a tile, will search for all the conected tiles of the same type using BFS, and kills them in the process.
    /// If no neighbour tile of the same type is found, the original tile is not killed
    /// </summary>
    /// <param name="tile">The tile that the BFS will start from</param>
    /// <returns>A dictionary of ints where the key is the column index, and the value, the lowest deleted tile's index</returns>
    private Dictionary<int, int> KillNeibourTiles(Tile tile)
    {
        var typeToDelete = tile.type;
        List<Tile> toDelete = new() { tile };
        Dictionary<int, int> deleted = new();

        bool firstRun = true;
        Tile current;
        int? up, down, left, right;
        while (toDelete.Count > 0)
        {
            current = toDelete[0];

            if (!current.Alive)
            {
                toDelete.Remove(current);
                continue;
            }

            up = current.TilePosition.y + 1 < visibleGridSize.y ? ((current.TilePosition.y + 1) * visibleGridSize.x) + current.TilePosition.x : null;
            down = current.TilePosition.y - 1 >= 0 ? ((current.TilePosition.y - 1) * visibleGridSize.x) + current.TilePosition.x : null;
            right = current.TilePosition.x + 1 < visibleGridSize.x ? (current.TilePosition.y * visibleGridSize.x) + current.TilePosition.x + 1 : null;
            left = current.TilePosition.x - 1 >= 0 ? (current.TilePosition.y * visibleGridSize.x) + current.TilePosition.x - 1 : null;


            if (up.HasValue && grid[up.Value].Alive && grid[up.Value].type == typeToDelete)
                toDelete.Add(grid[up.Value]);

            if (down.HasValue && grid[down.Value].Alive && grid[down.Value].type == typeToDelete)
                toDelete.Add(grid[down.Value]);

            if (right.HasValue && grid[right.Value].Alive && grid[right.Value].type == typeToDelete)
                toDelete.Add(grid[right.Value]);

            if (left.HasValue && grid[left.Value].Alive && grid[left.Value].type == typeToDelete)
                toDelete.Add(grid[left.Value]);


            toDelete.Remove(current);

            if (!(firstRun && toDelete.Count == 0))
            {
                if(deleted.TryGetValue(current.TilePosition.x, out int value))
                {
                    deleted[current.TilePosition.x] = Math.Min(value, current.TilePosition.y);
                }
                else
                {
                    deleted[current.TilePosition.x] = current.TilePosition.y;
                }
                current.Kill();
                current.transform.position = new Vector3(-10, 0, 0);
            }

            firstRun = false;
        }

        return deleted;
    }

/// <summary>
/// Receives the return from KillNeibourTiles to realocate alive tiles to free spaces, and spawn new tiles to fill the blanks
/// </summary>
/// <param name="deleted">A dictionary of ints where the key is the column index, and the value, the lowest deleted tile's index</param>
    private void ReSpawn(Dictionary<int, int> deleted)
    {
        foreach(var touple in deleted)
        {
            int j = touple.Key;

            int toSpawn = 0;
            Tile highest = touple.Value > 0 ? grid[((touple.Value - 1) * visibleGridSize.x) + j] : null;
            Tile current;
            for (int i = touple.Value; i < grid.Length / visibleGridSize.x; i++)
            {
                current = grid[(i * visibleGridSize.x) + j];
                if (current.Alive)
                {
                    int y = highest != null ? highest.TilePosition.y + 1 : 0;

                    bool visible = y < visibleGridSize.y;
                    current.GoTo(j, y, !visible);
                    current.gameObject.SetActive(visible);

                    grid[(y * visibleGridSize.x) + j] = current;

                    highest = current;
                }
                else
                {
                    toSpawn++;
                    availablePool[current.type].Push(current);
                }
            }

            for (; toSpawn > 0; toSpawn--)
            {
                Tile instance = Spawn();
                instance.Init();
                instance.GoTo(j, highest.TilePosition.y + 1, true);


                grid[(instance.TilePosition.y * visibleGridSize.x) + instance.TilePosition.x] = instance;
                instance.gameObject.SetActive(instance.TilePosition.y < visibleGridSize.y);

                highest = instance;
            }
        }
    }

    /// <summary>
    /// Instantiate a tile and initilize it
    /// </summary>
    /// <param name="type">The type of tile to be spawned, random if null</param>
    /// <returns>The instantiated tile</returns>
    private Tile Spawn(Tile.Type? tileType = null)
    {
        Tile.Type type = tileType ?? tilesPrefabs[rnd.Next(tilesPrefabs.Length)].type;

        if (availablePool[type].Count > 0)
        {
            return availablePool[type].Pop();
        }
        else
        {
            var instance = Instantiate(prefabCache[type], transform);
            instance.SetCallbacks(FromTilePositionToLocalPosition, OnTileClicked, SetMovingTile, RemoveMovingTile);
            return instance;
        }
    }

    /// <summary>
    /// Converts a tile position to a local position
    /// </summary>
    private Vector3 FromTilePositionToLocalPosition(Vector2Int position)
    {
        Vector2 tileWithPadding = padding + (Vector2.one * tileSize);
        Vector2 offset = (Vector2.one - visibleGridSize) / 2;
        offset.Scale(tileWithPadding);
        Vector3 localPosition = (Vector3)offset + new Vector3(tileWithPadding.x * position.x, tileWithPadding.y * position.y, 0);
        return localPosition;
    }
}
