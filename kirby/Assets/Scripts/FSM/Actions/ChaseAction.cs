using UnityEngine;

[CreateAssetMenu(fileName = "New ChaseAction", menuName = "KRB_RL/FSM/Actions/Chase")]
public class ChaseAction : AI.Action
{
    // 

    public override void Execute(Monster controller, float timeUnits)
    {
        if(!controller.ValidPath)
        {
            controller.RecalculatePath();            
        }
        else
        {
            controller.TickPathElapsed(timeUnits);
        }

        controller.FollowPath();
    }
}