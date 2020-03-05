using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class KrbMovingTrait: BaseMovingTrait
{
    // TODO: Move amount?
    public List<KrbTileType> AllowedTiles;

    public override void Init(BaseMovingTraitData data)
    {
        if (data is KrbMovingTraitData traitData)
        {
            AllowedTiles = new List<KrbTileType>(traitData.AllowedTypes);
        }
    }


    public override bool EvaluateTile(TileBase t)
    {
        var krbTile = (KrbTile)t;
        return AllowedTiles.Contains(krbTile.TileType);
    }
}
