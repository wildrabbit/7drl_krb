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
    public int delta;
    public bool isPlayer;
    public bool isHeal;
    public bool isAttack;
    public bool isRegen;

    public EntityHealthEvent(): base() { }

    public EntityHealthEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventCategory Category => EventCategory.Information;

    public override string Message()
    {
        if (isAttack) return $"{name} got hit by an enemy attack! Received {delta} damage\n";
        if (isHeal) return $"{name} restored {delta} HP\n";
        if (isRegen) return $"{name} regenerated {delta} HP\n";
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
        Death
    }

    public override EventCategory Category => EventCategory.PlayerAction;
    ActionType PlayerActionType;
    public MoveDirection PlayerMoveDirection;
    public Vector2Int EventCoords;

    public PlayerActionEvent() : base() { }

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

    public void SetDead()
    {
        PlayerActionType = ActionType.Death;
    }

    public override string Message()
    {
        StringBuilder builder = new StringBuilder(PrintTime());
        switch (PlayerActionType)
        {
            case ActionType.Movement:
                {
                    builder.AppendLine($"You moved <b>{PlayerMoveDirection}</b> to <b>{EventCoords}</b>");
                    break;
                }
            case ActionType.Idle:
                {
                    builder.AppendLine($"You've remained idle, contemplating your own existence");
                    break;
                }
            case ActionType.Death:
                {
                    builder.AppendLine($"You've died!");
                    break;
                }
        }
        return builder.ToString();
    }
}

