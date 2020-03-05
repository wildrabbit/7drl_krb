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
        if(Input.GetKeyDown(KeyCode.G))
        {
            contextData.EntityController.Player.ResetSpeedRate();
        }

        // REMOVE
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (Monster m in contextData.EntityController.Monsters)
            {
                m.ToggleDebug();
            }
        }

        timeWillPass = false;
        nextPlayContext = GameController.PlayStates.Action;
        return false;
    }

    protected override bool HandleAdditionalMoveInteractions(PlayerActionStateContext contextData, Vector2Int newPlayerCoords, ref int nextPlayState, ref bool canMove)
    {
        // TODO: Automatic absorption attempt
        IEntityController entities = contextData.EntityController;
        KrbPlayer player = (KrbPlayer)entities.Player;

        if (!player.AbsorberTrait.Data.AutoAbsorb)
        {
            return false;
        }

        IAbsorbableEntity candidate = null;
        var targetCandidates = entities.GetEntitiesAt(newPlayerCoords);
        foreach(var c in targetCandidates)
        {
            if (c is IAbsorbableEntity test && player.AbsorberTrait.TryAbsorb(test))
            {
                canMove = true;
                break;
            }
        }
        return false;
    }
}
