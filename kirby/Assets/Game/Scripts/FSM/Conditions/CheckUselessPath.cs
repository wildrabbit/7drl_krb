using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UselessPath", menuName = "KRB_RL/FSM/Conditions/CheckUselessPath")]
public class CheckUselessPath : AI.Condition
{
    public override bool Evaluate(Monster monster, float timeUnits)
    {
        return monster.UselessPath;
    }
}
