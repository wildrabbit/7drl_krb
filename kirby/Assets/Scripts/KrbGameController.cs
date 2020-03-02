using UnityEngine;

public class KrbGameController : GameController
{
    [SerializeField] KrbMapController _mapControllerPrefab;
   
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

    protected override BaseGameEvents CreateGameEvents()
    {
        return new KrbGameEvents();
    }

    protected override BaseInputController CreateInputController()
    {
        return new KrbInputController();
    }
}
