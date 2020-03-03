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
        offsets.ForEach(x => x += source);
        for (int i = 0; i < 4; ++i)
        {
            if(offsets.Contains(target))
            {
                dir = rotations[i];
                return true;
            }

            offsets.ForEach(x => Rotate90CC(ref x));
        }
        return false;
    }
}