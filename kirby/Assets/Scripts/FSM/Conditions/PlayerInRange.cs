using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerInRange", menuName = "KRB_RL/FSM/Conditions/PlayerInRange")]
public class PlayerInRange : AI.Condition
{
    public int Range;

    public override bool Evaluate(Monster monster, float timeUnits) => monster.DistanceFromPlayer() <= Range;
}
