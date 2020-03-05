using UnityEngine;

[CreateAssetMenu(fileName = "New TrueCondition", menuName = "KRB_RL/FSM/Conditions/True")]
public class TrueCondition: AI.Condition
{
    public override bool Evaluate(Monster monster, float timeUnits) => true;
}

