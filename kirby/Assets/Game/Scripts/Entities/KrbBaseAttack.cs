using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class KrbBaseAttack : BaseAttack
{
    public override int MaxRadius => _maxRadius;

    int _maxRadius;

    public override void Init()
    {
        _maxRadius = -1;
        foreach(var offset in Data.TargetOffsetsNorth)
        {
            var absX = Mathf.Abs(offset.x);
            var absY = Mathf.Abs(offset.y);
            _maxRadius = Mathf.Max(_maxRadius, Mathf.Max(absX, absY));
        }
    }

    public override bool CanTargetBeReached(Vector2Int source, Vector2Int target, out List<MoveDirection> validDirections)
    {
        validDirections = new List<MoveDirection>();
        MoveDirection[] rotations = new MoveDirection[]
        {
            MoveDirection.N, MoveDirection.E, MoveDirection.S, MoveDirection.W
        };

        List<Vector2Int> offsets = new List<Vector2Int>(Data.TargetOffsetsNorth);

        for (int i = 0; i < rotations.Length; ++i)
        {
            for(int j = 0; j < offsets.Count; ++j)
            {
                if(source + offsets[j] == target)
                {
                    validDirections.Add(rotations[i]);
                }
                offsets[j] = Rotate90CC(offsets[j]);
            }
        }
        return validDirections.Count > 0;
    }


    Vector2Int Rotate90CC(Vector2Int coords)
    {
        return new Vector2Int(-coords.y, coords.x);
    }

    public override bool CanTargetBeReachedAtDir(Vector2Int source, Vector2Int target, MoveDirection direction)
    {
        List<Vector2Int> offsets = GetRotatedOffsets(direction);
        for(int j = 0; j < offsets.Count; ++j)
        {
            if (source + offsets[j] == target)
            {
                return true;
            }
        }
        return false;
    }

    public override List<Vector2Int> GetRotatedOffsets(MoveDirection moveDir)
    {
        List<Vector2Int> offsets = new List<Vector2Int>(Data.TargetOffsetsNorth);
        int rotations = 0;
        switch (moveDir)
        {
            case MoveDirection.N:
            {
                break;
            }
            case MoveDirection.E:
            {
                rotations = 1;
                break;
            }
            case MoveDirection.S:
            {
                rotations = 2;
                break;
            }
            case MoveDirection.W:
            {
                rotations = 3;
                break;
            }
        }
        for (int j = 0; j < offsets.Count; ++j)
        {
            for (int i = 0; i < rotations; ++i)
            {
                offsets[j] = Rotate90CC(offsets[j]);
            }
        }
        return offsets;
    }
}