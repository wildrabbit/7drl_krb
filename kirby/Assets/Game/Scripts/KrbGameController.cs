using System.Collections.Generic;
using UnityEngine;

public class KrbGameController : GameController
{
#pragma warning disable 649
    [SerializeField] KrbMapController _mapControllerPrefab;
#pragma warning restore 649

    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();
    }

    protected override IMapController CreateMapController()
    {
        IMapController mapController = Instantiate<KrbMapController>(_mapControllerPrefab);
        return mapController;
    }

    protected override GameResult EvaluateVictory()
    {
        // TODO: Evaluate wtf has happened.
        string victoryMessage = string.Empty;
        GameFinishedEvent evt = new GameFinishedEvent(_timeController.Turns, _timeController.TimeUnits, GameResult.Won, victoryMessage);
        //_eventLog.EndSession(evt);

        return GameResult.Running;
    }

    protected override List<MonsterSpawnData> FetchMonsterSpawnPoints()
    {
        return ((KrbMapController)_mapController).MonsterSpawns;
    }

    protected override IEntityController CreateEntityController()
    {
        return new KrbEntityController();
    }

    protected override BaseGameEvents CreateGameEvents()
    {
        return new KrbGameEvents();
    }

    protected override BaseInputController CreateInputController()
    {
        return new KrbInputController();
    }

    protected override PlayerActionState CreatePlayerActionState()
    {
        return new KrbPlayerActionState();
    }

    protected override GameEventLog CreateGameLogger()
    {
        return new KrbEventLogger();
    }

    protected override void ExtendedPopulate()
    {
        BuildTraps();
        BuildBlocks();
    }

    void BuildTraps()
    {
        var traps = ((KrbMapController)_mapController).TrapSpawns;
        foreach (var trapSpawn in traps)
        {
            _entityController.CreateTrap(trapSpawn.TrapData, trapSpawn.Coords);
        }
    }

    void BuildBlocks()
    {
        var blocks = ((KrbMapController)_mapController).BlockSpawns;
        foreach (var block in blocks)
        {
            _entityController.CreateBlockingEntity(block.BlockData, block.Coords);
        }
    }
}
