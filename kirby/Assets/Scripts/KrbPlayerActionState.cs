using UnityEngine;
using System.Collections;

public class KrbPlayerActionState : PlayerActionState
{
    protected override bool HandleExtendedActions(PlayerActionStateContext contextData, out bool timeWillPass, out int nextPlayContext)
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            contextData.EntityController.Player.SetSpeedRate(40.0f);
        }
        else if(Input.GetKeyDown(KeyCode.G))
        {
            contextData.EntityController.Player.ResetSpeedRate();
        }

        timeWillPass = false;
        nextPlayContext = GameController.PlayStates.Action;
        return false;
    }
}
