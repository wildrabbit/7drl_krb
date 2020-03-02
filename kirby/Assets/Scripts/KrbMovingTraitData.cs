using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New moving trait set", menuName = "KRB_RL/Data/Traits")]
public class KrbMovingTraitData : BaseMovingTraitData
{
    public KrbTileType[] AllowedTypes;
    // TODO: move to some factory?
    public override BaseMovingTrait CreateRuntimeTrait()
    {
        return new KrbMovingTrait();
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
