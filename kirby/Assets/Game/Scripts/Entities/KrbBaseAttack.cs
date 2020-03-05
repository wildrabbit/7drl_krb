using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class KrbBaseAttack : BaseAttack
{
    Dictionary<MoveDirection, Vector2Int[]> Offsets;

    public KrbBaseAttack(BaseAttackData data)
        :base(data)
    {
        BuildRotatedOffsets();
    }

    void BuildRotatedOffsets()
    {
        Offsets = new Dictionary<MoveDirection, Vector2Int[]>();
        var northOffsets = ((KrbBaseAttackData)Data).TargetOffsetsNorth.ToArray();
        Offsets.Add(MoveDirection.N, northOffsets);
        MoveDirection[] rotations = { MoveDirection.E, MoveDirection.S, MoveDirection.W };
        for(int i = 0; i < rotations.Length; ++i)
        {
            Vector2Int[] coords = new Vector2Int[northOffsets.Length];
            northOffsets.CopyTo(coords, 0);

            int numRotations = i + 1;

            for (int j = 0; j < coords.Length; ++j)
            {
                for (int k = 0; k < numRotations ; ++k)
                {
                    coords[j] = Rotate90CC(coords[j]);
                }
            }
            Offsets.Add(rotations[i], coords);
        }
    }

    Vector2Int Rotate90CC(Vector2Int coords)
    {
        return new Vector2Int(-coords.y, coords.x);
    }

    bool CanCoordsBeReached(Vector2Int srcCoords, Vector2Int tgtCoords)
    {
        foreach (var rotationOffsetsPair in Offsets)
        {
            for (int i = 0; i < rotationOffsetsPair.Value.Length; ++i)
            {
                if (tgtCoords == srcCoords + rotationOffsetsPair.Value[i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool CanTargetBeReached(IBattleEntity attacker, IBattleEntity target)
    {
        var srcCoords = attacker.Coords;
        var tgtCoords = target.Coords;
        return CanCoordsBeReached(srcCoords, tgtCoords);
    }

    public override List<IBattleEntity> FindAllReachableTargets(IBattleEntity source)
    {
        MoveDirection bestDir = MoveDirection.None;
        List<IBattleEntity> entities = new List<IBattleEntity>();
        var srcCoords = source.Coords;
        foreach (var pair in Offsets)
        {
            foreach(var offsetCoord in pair.Value)
            {
                Vector2Int sampleCoords = srcCoords + offsetCoord;
                var hostiles = _entityController.GetFilteredEntitiesAt<IBattleEntity>(sampleCoords).FindAll(x => x.IsHostileTo(source));

                // TODO: Add scoring system to qualify potential of that attack
                if(hostiles.Count >= entities.Count)
                {
                    bestDir = pair.Key;
                    entities = hostiles;
                }
            }
        }
        return entities;
    }


    public override List<IBattleEntity> FindTargetsAtCoords(IBattleEntity source, Vector2Int refCoords)
    {
        var srcCoords = source.Coords;
        if(CanCoordsBeReached(srcCoords, refCoords))
        {
            return _entityController.GetFilteredEntitiesAt<IBattleEntity>(refCoords).FindAll(x => x.IsHostileTo(source));
        }
        return new List<IBattleEntity>();
    }

    public override List<IBattleEntity> FindAllReachableTargets(IBattleEntity source, IBattleEntity requiredTarget)
    {
        MoveDirection bestDir = MoveDirection.None;
        List<IBattleEntity> entities = new List<IBattleEntity>();
        var srcCoords = source.Coords;
        foreach (var pair in Offsets)
        {
            foreach (var offsetCoord in pair.Value)
            {
                Vector2Int sampleCoords = srcCoords + offsetCoord;
                var hostiles = _entityController.GetFilteredEntitiesAt<IBattleEntity>(sampleCoords).FindAll(x => x.IsHostileTo(source));

                // TODO: Add scoring system to qualify potential of that attack
                if (hostiles.Contains(requiredTarget) && hostiles.Count >= entities.Count)
                {
                    bestDir = pair.Key;
                    entities = hostiles;
                }
            }
        }
        return entities;
    }
}