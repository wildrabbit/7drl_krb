﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerItemEvent : EventLogMessage
{
    public bool isAdded;
    public bool isSelected;
    public bool isDropped;
    public bool isDepleted;
    public PlayerItemEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventLogCategory Category => EventLogCategory.Information;

    public override string Message()
    {
        return string.Empty;
    }
}

public class AbsorptionEvent : EventLogMessage
{
    public string AbsorberName;
    public Vector2Int AbsorberCoords;
    public string AbsorbedName;
    public Vector2Int AbsorbedCoords;

    public AbsorptionEvent() : base() { }

    public AbsorptionEvent(int turns, float units) : base(turns, units) { }

    public override EventLogCategory Category => EventLogCategory.Information;

    public override string Message()
    {
        return $"Monster {AbsorbedName} at {AbsorbedCoords} was absorbed by {AbsorberName} at {AbsorberCoords}!\n";
    }
}

public class AbsorptionExpiredEvent : EventLogMessage
{
    public string AbsorberName;
    public Vector2Int AbsorberCoords;
    public AbsorptionExpiredEvent() : base() { }

    public AbsorptionExpiredEvent(int turns, float units) : base(turns, units) { }

    public override EventLogCategory Category => EventLogCategory.Information;

    public override string Message()
    {
        return $"{AbsorberName} at {AbsorberCoords} went back to its past shape!\n";
    }
}

public class AbsorptionReadyEvent : EventLogMessage
{
    public string Name;
    public Vector2Int Coords;

    public AbsorptionReadyEvent() : base() { }

    public AbsorptionReadyEvent(int turns, float units) : base(turns, units) { }

    public override EventLogCategory Category => EventLogCategory.Information;

    public override string Message()
    {
        return $"Monster {Name} at {Coords} can be absorbed!\n";
    }
}

public class EntityHealthEvent : EventLogMessage
{
    public string name;
    public int delta;
    public bool isPlayer;
    public bool isHeal;
    public bool isAttack;
    public bool isRegen;

    public EntityHealthEvent() : base() { }

    public EntityHealthEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventLogCategory Category => EventLogCategory.Information;

    public override string Message()
    {
        if (isAttack) return $"{name} got hit by an enemy attack! Received {delta} damage\n";
        if (isHeal) return $"{name} restored {delta} HP\n";
        if (isRegen) return $"{name} regenerated {delta} HP\n";
        return string.Empty;
    }
}

public class PlayerMonsterCollisionEvent : EventLogMessage
{
    public string PlayerName;
    public string MonsterName;
    public int PlayerDamageReceived;
    public int MonsterDamageReceived;

    public PlayerMonsterCollisionEvent(int Turns, float Units)
        : base(Turns, Units)
    {

    }

    public override EventLogCategory Category => EventLogCategory.Information;

    public override string Message()
    {
        StringBuilder b = new StringBuilder($"{MonsterName} and you have clashed! You've received {PlayerDamageReceived} damage!");
        return b.ToString();
    }
}




public class PlayerActionEvent : EventLogMessage
{
    enum ActionType
    {
        Movement,
        Idle,
        Death
    }

    public override EventLogCategory Category => EventLogCategory.PlayerAction;
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

public class TrapEvent : EventLogMessage
{
    public override EventLogCategory Category => EventLogCategory.Information;
    public Trap Trap;
    public BaseEntity Entity;

    public override string Message()
    {
        return $"{Entity.Name} fell into Trap at {Trap.Coords}, took {Trap.Data.Damage}\n";
    }
}



public class KrbEventLogger: GameEventLog
{
    public override void Init(TimeController timeController, BaseGameEvents events)
    {
        base.Init(timeController, events);
        ConnectEvents();
    }

    public override void Cleanup()
    {
        base.Cleanup();
        DisconnectEvents();
    }

    public void ConnectEvents()
    {
        _gameEvents.Health.HealthEvent += EntityHealth;
        _gameEvents.Player.Died += PlayerDied;
        _gameEvents.Player.IdleTurn += PlayerIdle;
        _gameEvents.Traps.EntityFellIntoTrap += TriggeredTrap;
        ((KrbGameEvents)_gameEvents).Absorption.CanBeAbsorbed += AbsorptionReady;
        ((KrbGameEvents)_gameEvents).Absorption.WasAbsorbed += AbsorptionHappened;
        ((KrbGameEvents)_gameEvents).Absorption.AbsorptionExpired += AbsorptionExpired;
    }

    public void DisconnectEvents()
    {
        _gameEvents.Health.HealthEvent -= EntityHealth;
        _gameEvents.Player.Died -= PlayerDied;
        _gameEvents.Player.IdleTurn -= PlayerIdle;
        _gameEvents.Traps.EntityFellIntoTrap -= TriggeredTrap;
        ((KrbGameEvents)_gameEvents).Absorption.CanBeAbsorbed -= AbsorptionReady;
        ((KrbGameEvents)_gameEvents).Absorption.WasAbsorbed -= AbsorptionHappened;
        ((KrbGameEvents)_gameEvents).Absorption.AbsorptionExpired -= AbsorptionExpired;
    }

    private void PlayerIdle()
    {
        PlayerActionEvent evt = CreateEvent<PlayerActionEvent>();
        evt.SetIdle();
        AddEvent(evt);
    }

    private void PlayerDied()
    {
        PlayerActionEvent evt = CreateEvent<PlayerActionEvent>();
        evt.SetDead();
        AddEvent(evt);
    }

    public void AbsorptionReady(IAbsorbableEntity entity)
    {
        AbsorptionReadyEvent evt = CreateEvent<AbsorptionReadyEvent>();
        evt.Name = entity.Name;
        evt.Coords = entity.Coords;
        AddEvent(evt);
    }

    public void AbsorptionHappened(IAbsorbableEntity absorbed, IAbsorbingEntity absorber)
    {
        AbsorptionEvent evt = CreateEvent<AbsorptionEvent>();
        evt.AbsorberName = absorber.Name;
        evt.AbsorberCoords = absorber.Coords;

        evt.AbsorbedName = absorbed.Name;
        evt.AbsorbedCoords = absorbed.Coords;
        AddEvent(evt);
    }

    public void AbsorptionExpired(IAbsorbingEntity entity)
    {
        AbsorptionExpiredEvent evt = CreateEvent<AbsorptionExpiredEvent>();
        evt.AbsorberName = entity.Name;
        evt.AbsorberCoords = entity.Coords;
        AddEvent(evt);
    }

    private void EntityHealth(IHealthTrackingEntity entity, int delta, bool heal, bool attack, bool regen)
    {
        EntityHealthEvent evt = CreateEvent<EntityHealthEvent>();
        evt.name = entity.Name;
        evt.isPlayer = true;
        evt.isHeal = heal;
        evt.isAttack = attack;
        evt.isRegen = regen;
        evt.delta = delta;
        AddEvent(evt);
    }

    private void TriggeredTrap(Trap leTrap, BaseEntity leEntity)
    {
        TrapEvent evt = CreateEvent<TrapEvent>();
        evt.Trap = leTrap;
        evt.Entity = leEntity;
        AddEvent(evt);
    }

    protected T CreateEvent<T>() where T: EventLogMessage, new()
    {
        T evt = new T()
        {
            Turns = _timeController.Turns,
            Time = _timeController.TimeUnits
        };
        return evt;
    }



}
