using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New KrbAttack", menuName = "KRB_RL/Data/Attacks/CustomAttack")]
public class KrbBaseAttackData : BaseAttackData
{
    public List<Vector2Int> TargetOffsetsNorth;
    public BaseEntityData SpawnData; // Projectiles, volumes, etc

    public override BaseAttack SpawnRuntime()
    {
        return new KrbBaseAttack(this);
    }
}