using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KrbGameEvents: BaseGameEvents
{
    public KrbGameEvents():base()
    {

    }
}

public class PlayerItemEvent : BaseEvent
{
    public bool isAdded;
    public bool isSelected;
    public bool isDropped;
    public bool isDepleted;
    public PlayerItemEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventCategory Category => EventCategory.Information;

    public override string Message()
    {        
        return string.Empty;
    }
}

public class EntityHealthEvent : BaseEvent
{
    public string name;
    public int dmg;
    public bool isPlayer;
    public bool isHeal;
    public bool isExplosion;
    public bool isCollision;
    public bool isPoison;
    public EntityHealthEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventCategory Category => EventCategory.Information;

    public override string Message()
    {
        if (isExplosion) return $"{name} got hit by Explosion! Received {dmg} damage\n";
        if (isHeal) return $"{name} restored {dmg} HP\n";
        if (isPoison) return $"{name} took {dmg} poison damage\n";
        return string.Empty;
    }
}

public class PlayerMonsterCollisionEvent : BaseEvent
{
    public string PlayerName;
    public string MonsterName;
    public int PlayerDamageReceived;
    public int MonsterDamageReceived;

    public PlayerMonsterCollisionEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventCategory Category => EventCategory.Information;

    public override string Message()
    {
        StringBuilder b = new StringBuilder($"{MonsterName} and you have clashed! You've received {PlayerDamageReceived} damage!");
        return b.ToString();
    }
}



public class PlayerActionEvent : BaseEvent
{
    enum ActionType
    {
        Movement,
        Idle,
        BombPlacement,
        BombDetonation,
        // TODO: Grabbing loot, equipping, consuming, etc
    }

    public override EventCategory Category => EventCategory.PlayerAction;
    ActionType PlayerActionType;
    public MoveDirection PlayerMoveDirection;
    public Vector2Int EventCoords;

    public PlayerActionEvent(int turns, float time)
        : base(turns, time)
    {

    }

    public void SetMovement(MoveDirection moveDir, Vector2Int coords)
    {
        PlayerActionType = ActionType.Movement;
        PlayerMoveDirection = moveDir;
        EventCoords = coords;
    }

    public void SetIdle()
    {
        PlayerActionType = ActionType.Idle;
    }

    public void SetBomb(Vector2Int coords)
    {
        PlayerActionType = ActionType.BombPlacement;
        EventCoords = coords;
    }

    public override string Message()
    {
        StringBuilder builder = new StringBuilder(PrintTime());
        switch (PlayerActionType)
        {
            case ActionType.Movement:
                {
                    builder.AppendLine($"Player moves <b>{PlayerMoveDirection}</b> to <b>{EventCoords}</b>");
                    break;
                }
            case ActionType.Idle:
                {
                    builder.AppendLine($"Player remains idle and contemplates her own existence");
                    break;
                }
            case ActionType.BombPlacement:
                {
                    builder.AppendLine($"Player places bomb @ <b>{EventCoords}</b>");
                    break;
                }
        }
        return builder.ToString();
    }
}

