using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileExtensions
{
    public static Color ToColor(this Tile.Type type)
    {
        switch (type)
        {
            case Tile.Type.RED:
                return Color.red;
            case Tile.Type.GREEN:
                return Color.green;
            case Tile.Type.BLUE:
                return Color.blue;
            case Tile.Type.YELLOW:
                return Color.yellow;
            default:
                throw new NotImplementedException();
        }
    }
}
