using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerCanBeAttacked", menuName = "KRB_RL/FSM/Conditions/PlayerCanBeAttacked")]
public class PlayerCanBeAttacked : AI.Condition
{
    public override bool Evaluate(Monster monster, float timeUnits)
    {
        return monster.TrySelectAvailableAttack();
    }
}
