using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New KrbMonsterData", menuName = "KRB_RL/Data/Entities/Monster")]
public class KrbMonsterData: MonsterData
{
    public AbsorptionData AbsorptionData;
    public GameObject AbsorbablePrefab;
}
