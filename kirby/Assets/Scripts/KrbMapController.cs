using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class KrbMapController : MonoBehaviour, IMapController
{
    public Vector2Int PlayerStart => _playerStart;
    [SerializeField] Tilemap _mapView;
    KrbMapData _mapData;
    RectGridMap _mapHelper;

    Vector2Int _playerStart;

    public Rect WorldBounds
    {
        get
        {
            // Tilemap uses x: row, y: col. I'm using the opposite.
            BoundsInt cellBounds = _mapView.cellBounds;
            Vector2 min = _mapView.CellToWorld(new Vector3Int(cellBounds.yMin, cellBounds.xMin, 0));
            Vector2 max = _mapView.CellToWorld(new Vector3Int(cellBounds.yMax, cellBounds.xMax, 0));
            return new Rect(min.y, min.x, (max.y - min.y), (max.x - min.x));
        }
    }

    public BoundsInt CellBounds => _mapView.cellBounds;

    public void Cleanup()
    {
        _mapView.ClearAllTiles();
    }

    public void Init(BaseMapData mapData)
    {
        _mapData = (KrbMapData)mapData;
        _mapHelper = new RectGridMap(_mapData.DistanceStrategy);

        _palette = new Dictionary<KrbTileType, KrbTile>();
        foreach (var entry in _mapData.Palette)
        {
            var castTile = (KrbTile)entry;
            _palette.Add(castTile.TileType, castTile);
        }
        BuildMap();
    }

    public int Distance(Vector2Int from, Vector2Int to)
    {
        return _mapHelper.Distance(from, to);
    }

    public Vector2Int CalculateMoveOffset(MoveDirection inputDir, Vector2Int playerCoords)
    {
        return _mapHelper.GetDirectionOffset(inputDir, playerCoords);
    }

    public Vector2Int CoordsFromWorld(Vector3 worldPos)
    {
        return (Vector2Int)_mapView.WorldToCell(worldPos);
    }

    public TileBase GetTileAt(Vector2Int coords)
    {
        return _mapView.GetTile(new Vector3Int(coords.x, coords.y, 0));
    }

    public KrbTile GetKrbTileAt(Vector2Int coords)
    {
        return (KrbTile)GetTileAt(coords);
    }

    Dictionary<KrbTileType, KrbTile> _palette;
    public KrbTile NoTile => null;


    // We'll start with neutral, then N and then clockwise
    Vector2Int[][] _neighbourOffsets = new Vector2Int[][]
    {
        new Vector2Int[]{ new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1,1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1)},
        new Vector2Int[]{ new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1),new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1),new Vector2Int(1, -1)}
    };

    public int Height => _mapView.size.x;
    public int Width => _mapView.size.y;

    public bool HasTile(Vector3Int pos)
    {
        return _mapView.HasTile(pos);
    }

    public int[] AllTileValues
    {
        get
        {
            var tileBase = _mapView.GetTilesBlock(CellBounds);
            return System.Array.ConvertAll(tileBase, x => (int)GetTypeFromTile(x));
        }
    }

    public KrbTileType GetTypeFromTile(TileBase tile)
    {
        if (tile == null)
        {
            return KrbTileType.None;
        }
        return ((KrbTile)tile).TileType;
    }

    public int GetIntValueFromTile(TileBase tile)
    {
        return (int)GetTypeFromTile(tile);
    }

    public KrbTile[] AllTiles
    {
        get
        {
            var tileBase = _mapView.GetTilesBlock(CellBounds);
            return System.Array.ConvertAll(tileBase, tile => (KrbTile)tile);
        }
    }

    public void BuildMap()
    {
        if (_mapData.GenerationData.GeneratorType == GeneratorType.Fixed)
        {
            BaseMapContext mapContext = new BaseMapContext();
            mapContext.GeneratorData = _mapData.GenerationData;

            FixedMapGeneratorData genData = ((FixedMapGeneratorData)_mapData.GenerationData);
            InitFromArray(genData.MapSize, genData.LevelData, genData.PlayerStart, genData.OriginIsTopLeft);
        }
        else if (_mapData.GenerationData.GeneratorType == GeneratorType.BSP)
        {
            BSPContext context = new BSPContext();
            context.GeneratorData = _mapData.GenerationData;
            context.PlayerStart = Vector2Int.zero;
            int[] level = null;
            IMapGenerator generator = new BSPMapGenerator();
            generator.GenerateMap(ref level, context);

            InitFromArray(context.GeneratorData.MapSize, level, context.PlayerStart, _mapData.GenerationData.OriginIsTopLeft);
        }
    }

    public void InitFromArray(Vector2Int dimensions, int[] typeArray, Vector2Int playerStart, bool arrayOriginTopLeft)
    {
        _mapView.ClearAllTiles();
        int width = dimensions.y;
        int height = dimensions.x;
        if (typeArray.Length != width * height)
        {
            Debug.LogError("Array dimensions not matching provided size");
            return;
        }

        _playerStart = playerStart;
        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                int rowCoord = (arrayOriginTopLeft) ? height - (row + 1) : row;
                _mapView.SetTile(new Vector3Int(rowCoord, col, 0), GetTileByType((KrbTileType)typeArray[width * row + col]));
            }
        }
        _mapView.CompressBounds();
    }

    public List<Vector2Int> GetWalkableNeighbours(Vector2Int coords)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        Vector2Int[] offsets = _neighbourOffsets[coords.y & 1];
        for (int i = 1; i < offsets.Length; ++i)
        {
            Vector2Int neighbourCoords = coords + offsets[i];
            var tile = TileAt(neighbourCoords);
            if (tile != null && tile.Walkable)
            {
                neighbours.Add(neighbourCoords);
            }
        }
        return neighbours;
    }

    public KrbTile GetTileByType(KrbTileType type)
    {
        if(_palette.TryGetValue(type, out var tile))
        {
            return tile;
        }
        return null;
    }

    public Vector3 WorldFromCoords(Vector2Int coords)
    {
        return _mapView.CellToWorld((Vector3Int)coords);
    }

    public Vector2Int CoordsFromWorld(Vector2 pos)
    {
        return (Vector2Int)_mapView.WorldToCell(pos);
    }

    public void ConstrainCoords(ref Vector2Int playerCoords)
    {
        playerCoords.x = Mathf.Clamp(playerCoords.x, 0, _mapView.size.x - 1);
        playerCoords.y = Mathf.Clamp(playerCoords.y, 0, _mapView.size.y - 1);
    }

    internal bool IsWalkableTile(Vector2Int playerTargetPos)
    {
        KrbTile gameTile= (KrbTile) _mapView.GetTile((Vector3Int)playerTargetPos);
        return (gameTile == null) ? false : gameTile.Walkable;
    }

    public Rect GetBounds()
    {
        // Tilemap uses x: row, y: col. I'm using the opposite.
        BoundsInt cellBounds = _mapView.cellBounds;
        Vector2 min = _mapView.CellToWorld(new Vector3Int(cellBounds.yMin, cellBounds.xMin, 0));
        Vector2 max = _mapView.CellToWorld(new Vector3Int(cellBounds.yMax, cellBounds.xMax, 0));
        return new Rect(min.y, min.x, (max.y - min.y), (max.x - min.x));
    }

    public bool InBounds(Vector2Int coords)
    {
        return coords.x >= 0 && coords.x < _mapView.size.x && coords.y >= 0 && coords.y < _mapView.size.y;
    }

    List<Vector2Int> GetNearbyCoords(Vector2Int reference, int radius)
    {
        List<Vector2Int> pending = new List<Vector2Int>();
        List<Vector2Int> pendingNextDepth = new List<Vector2Int>();
        HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();

        pending.Add(reference);
        int depth = 0;
        while (depth <= radius)
        {
            pendingNextDepth.Clear();
            foreach (var curReference in pending)
            {
                candidates.Add(curReference);
                bool isEven = curReference.y % 2 == 0;
                Vector2Int[] offsets = _neighbourOffsets[isEven ? 0 : 1];
                // TODO: OPTIMIZATION: Make offsets dependent on the lookup direction
                foreach (var offset in offsets)
                {
                    Vector2Int neighbourCoords = curReference + offset;
                    if (!InBounds(neighbourCoords)) continue;
                    if (pending.Contains(neighbourCoords) || pendingNextDepth.Contains(neighbourCoords) || candidates.Contains(neighbourCoords)) continue;
                    pendingNextDepth.Add(neighbourCoords);
                }
            }
            depth++;
        }
        return new List<Vector2Int>(candidates);
    }

    public KrbTile TileAt(Vector2Int coords)
    {
        return _mapView.GetTile((Vector3Int)coords) as KrbTile;
    }

    public bool ExistsTile(Vector3Int coords3D)
    {
        return _mapView.HasTile(coords3D);
    }

    public bool IsNavigationValidTile(Vector3Int coords)
    {
        return _mapView.HasTile(coords) && ((KrbTile)GetTileAt((Vector2Int)coords)).Walkable;
    }

    public void GetNeighbourDeltas(Vector2Int currentCoords, out Vector2Int[] offsets)
    {
        var source = _neighbourOffsets[currentCoords.y & 1];
        int deltasLen = source.Length - 1;
        offsets = new Vector2Int[deltasLen];
        Array.Copy(source, 1, offsets, 0, deltasLen);
    }
}
