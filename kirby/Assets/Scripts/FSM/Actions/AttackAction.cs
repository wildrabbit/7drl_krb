using UnityEngine;

[CreateAssetMenu(fileName = "New AttackAction", menuName = "KRB_RL/FSM/Actions/Attack")]
public class AttackAction : AI.Action
{
    // 

    public override void Execute(Monster controller, float timeUnits)
    {
        controller.LaunchAttack();
    }
}