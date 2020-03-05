using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Base Attack", menuName = "KRB_RL/Data/Attack")]
public class KrbBaseAttackData : BaseAttackData
{
    public override BaseAttack SpawnRuntime()
    {
        return new KrbBaseAttack
        {
            Data = this,
            Elapsed = -1
        };
    }
}