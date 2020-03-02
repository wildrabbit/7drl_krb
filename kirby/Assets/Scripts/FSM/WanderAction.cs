using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New WanderAction", menuName ="KRB_RL/FSM/Actions/Wander")]
public class WanderAction : AI.Action
{
    public float WanderChance = 0.5f;

    public override void Execute(Monster controller, float timeUnits)
    {
        bool willMoveThisTurn = UnityEngine.Random.value <= WanderChance;
        if(willMoveThisTurn)
        {
            controller.WanderStep();
        }
    }
}
