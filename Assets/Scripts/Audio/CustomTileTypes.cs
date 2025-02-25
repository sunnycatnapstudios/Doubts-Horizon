using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTileTypes : TileBase {
    public TileType tileType; // Enum for tile types

    public enum TileType {
        Dirt,
        Stone,
        Water,
        Wood
    }
}
