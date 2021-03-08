using UnityEngine;
using UnityEngine.Tilemaps;

public enum KrbTileType
{
    None = -1,
    Wall = 0,
    Ground,
    Water
}

[CreateAssetMenu(fileName = "New KrbTile", menuName = "KRB_RL/Tile")]
public class KrbTile : Tile
{
    public KrbTileType TileType => _tile;
    public bool Walkable => _walkable;
    public bool Impassable => _impassable;

#pragma warning disable 649
    [SerializeField] KrbTileType _tile;
    [SerializeField] bool _walkable;
    [SerializeField] bool _impassable;
#pragma warning restore 649
}
