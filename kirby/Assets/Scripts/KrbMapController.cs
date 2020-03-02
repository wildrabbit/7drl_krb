using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class KrbMapController : MonoBehaviour, IMapController
{
    public Vector2Int PlayerStart => _playerStart;
    [SerializeField] Tilemap _mapView;


    public BoundsInt CellBounds => _mapView.cellBounds;

    public int Height => _mapView.size.x;
    public int Width => _mapView.size.y;

    KrbMapData _mapData;
    RectGridMap _mapHelper;

    Dictionary<KrbTileType, KrbTile> _palette;


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

    public void Cleanup()
    {
        _mapView.ClearAllTiles();
    }

    public void Init(BaseMapData mapData)
    {
        _mapData = (KrbMapData)mapData;
        _mapHelper = new RectGridMap(_mapData.DistanceStrategy); // Instantiate selectively from data

        _palette = new Dictionary<KrbTileType, KrbTile>();
        foreach (var entry in _mapData.Palette)
        {
            var castTile = (KrbTile)entry;
            _palette.Add(castTile.TileType, castTile);
        }
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
            BaseMapContext mapContext = new BaseMapContext
            {
                GeneratorData = _mapData.GenerationData
            };

            FixedMapGeneratorData genData = ((FixedMapGeneratorData)_mapData.GenerationData);
            if(!genData.BuildLevelData())
            {
                return;
            }
            InitFromArray(genData.MapSize, genData.LevelData, genData.PlayerStart, genData.OriginIsTopLeft);
        }
        else if (_mapData.GenerationData.GeneratorType == GeneratorType.BSP)
        {
            BSPContext context = new BSPContext
            {
                GeneratorData = _mapData.GenerationData,
                PlayerStart = Vector2Int.zero
            };
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
        if(arrayOriginTopLeft)
        {
            _playerStart.x = height - (_playerStart.x + 1);
        }
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
        Vector2Int[] offsets = _mapHelper.GetOffsets(coords);
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
        return _mapView.GetCellCenterWorld((Vector3Int)coords);
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
        HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();
        Queue<Vector2Int> pending = new Queue<Vector2Int>();
        pending.Enqueue(reference);
        while(pending.Count > 0)
        {
            var curReference = pending.Dequeue();
            candidates.Add(curReference);
            Vector2Int[] offsets = _mapHelper.GetOffsets(curReference);
            // TODO: OPTIMIZATION: Make offsets dependent on the lookup direction
            foreach (var offset in offsets)
            {
                Vector2Int neighbourCoords = curReference + offset;
                if (Distance(reference, neighbourCoords) > radius) continue;
                if (!InBounds(neighbourCoords)) continue;
                if (pending.Contains(neighbourCoords) || candidates.Contains(neighbourCoords)) continue;

                pending.Enqueue(neighbourCoords);
            }
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
        var source = _mapHelper.GetOffsets(currentCoords);
        int deltasLen = source.Length - 1;
        offsets = new Vector2Int[deltasLen];
        Array.Copy(source, 1, offsets, 0, deltasLen);
    }

    public void SetRandomCoords(Vector2Int refCoords, int scatterLimitRadius, ref Vector2Int[] coordList, bool firstIsRef, Predicate<Vector2Int> exclusionCheck = null)
    {
        int startIdx = firstIsRef ? 1 : 0;
        var nearbyCoords = GetNearbyCoords(refCoords, scatterLimitRadius);
        nearbyCoords.RemoveAll(exclusionCheck);

        for(int i = startIdx; i < coordList.Length; ++i)
        {
            if(nearbyCoords.Count == 0)
            {
                break;
            }
            int idx = UnityEngine.Random.Range(0, nearbyCoords.Count - 1);
            coordList[i] = nearbyCoords[idx];
            nearbyCoords.RemoveAt(idx);
        }
    }

    public Vector2Int RandomNeighbour(Vector2Int refCoords, Predicate<Vector2Int> restrictions = null)
    {
        var neighbours = GetNearbyCoords(refCoords, 1);
        if(restrictions != null)
        {
            neighbours.RemoveAll(restrictions);
        }

        if(neighbours.Count > 0)
        {
            return neighbours[UnityEngine.Random.Range(0, neighbours.Count - 1)];
        }
        return refCoords;
    }

    public List<MonsterSpawnData> MonsterSpawns
    {
        get
        {
            if (_mapData.GenerationData is FixedMapGeneratorData fixedMapData)
            {
                var spawns = new List<MonsterSpawnData>(fixedMapData.MonsterSpawns);
                if(fixedMapData.OriginIsTopLeft)
                {
                    foreach(var spawn in spawns)
                    {
                        spawn.Coords.x = Height - (spawn.Coords.x + 1);
                    }
                }
                return fixedMapData.MonsterSpawns;
            }
            return null;
        }
    }


}
