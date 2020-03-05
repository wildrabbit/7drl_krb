using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New MapData", menuName = "KRB_RL/Data/Map")]
public class KrbMapData : BaseMapData
{
    public bool AllowDiagonals;
    public DistanceStrategy DistanceStrategy;
}
