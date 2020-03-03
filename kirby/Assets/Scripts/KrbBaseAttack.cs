using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class KrbBaseAttack : BaseAttack
{
    public override bool CanTargetBeReached(Vector2Int source, Vector2Int target, out MoveDirection dir)
    {
        dir = MoveDirection.None;
        MoveDirection[] rotations = new MoveDirection[]
        {
        MoveDirection.N, MoveDirection.E, MoveDirection.S, MoveDirection.W
        };

        List<Vector2Int> offsets = new List<Vector2Int>(Data.TargetOffsetsNorth);

        for (int i = 0; i < 4; ++i)
        {
            for(int j = 0; j < offsets.Count; ++j)
            {
                if(source + offsets[j] == target)
                {
                    dir = rotations[i];
                    return true;
                }
                offsets[j] = Rotate90CC(offsets[j]);
            }
        }
        return false;
    }
}