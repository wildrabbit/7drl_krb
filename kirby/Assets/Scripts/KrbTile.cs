using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum KrbTileType
{
    None = 0,
    Wall,
    Water,
    Ground // ??
}

[CreateAssetMenu(fileName = "New KrbTile", menuName = "KRB_RL/Tile")]
public class KrbTile : Tile
{
    public KrbTileType TileType => _tile;
    public bool Walkable => _walkable;
    public bool Impassable => _impassable;

    [SerializeField] KrbTileType _tile;
    [SerializeField] bool _walkable;
    [SerializeField] bool _impassable;
}

