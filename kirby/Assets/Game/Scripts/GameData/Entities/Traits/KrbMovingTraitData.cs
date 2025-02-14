﻿using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New moving trait set", menuName = "KRB_RL/Data/Traits/Moving")]
public class KrbMovingTraitData : BaseMovingTraitData
{
    public KrbTileType[] AllowedTypes;
    // TODO: move to some factory?
    public override BaseMovingTrait CreateRuntimeTrait()
    {
        var movingTrait = new KrbMovingTrait();
        movingTrait.Init(this);
        return movingTrait;

    }

    public override bool MatchesTile(TileBase t)
    {
        if(t is KrbTile krbTile)
        {
            return System.Array.IndexOf(AllowedTypes, krbTile.TileType) >= 0;
        }
        return false;
    }
}
