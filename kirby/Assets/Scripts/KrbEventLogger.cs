using System;
using System.Collections.Generic;

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
    }

    public void DisconnectEvents()
    {
        _gameEvents.Health.HealthEvent -= EntityHealth;
        _gameEvents.Player.Died -= PlayerDied;
        _gameEvents.Player.IdleTurn -= PlayerIdle;
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

    private void EntityHealth(IHealthTrackingEntity entity, int delta, bool heal, bool attack, bool regen)
    {
        if(entity is Player)
        {
            EntityHealthEvent evt = CreateEvent<EntityHealthEvent>();
            Player p = (Player)entity;
            evt.name = p.Name;
            evt.isPlayer = true;
            evt.isHeal = heal;
            evt.isAttack = attack;
            evt.isRegen = regen;
            evt.delta = delta;
            AddEvent(evt);
        }
    }

    protected T CreateEvent<T>() where T: BaseEvent, new()
    {
        T evt = new T()
        {
            Turns = _timeController.Turns,
            Time = _timeController.TimeUnits
        };
        return evt;
    }



}
