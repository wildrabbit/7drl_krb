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
}
