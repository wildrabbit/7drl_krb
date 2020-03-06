using UnityEngine;

public class KrbPlayer: Player, IAbsorbingEntity
{
    public AbsorberTrait AbsorberTrait => _absorberTrait;

    protected GameObject _absorptionDecorationView;
    protected GameObject _originalView;
    protected HPTrait _originalHP;
    protected BaseMovingTrait _originalMoving;
    protected BattleTrait _originalBattleTrait;

    BaseGameEvents _events;

    AbsorberTrait _absorberTrait;
    KrbGameEvents.AbsorptionEvents _absorbEvents;
    BaseGameEvents _gameEvents;

    protected override void DoInit(BaseEntityDependencies deps)
    {
        base.DoInit(deps);
        _events = deps.GameEvents;
        _absorbEvents = ((KrbGameEvents)deps.GameEvents).Absorption;
        _absorberTrait = new AbsorberTrait();
        _absorberTrait.Init(this, ((KrbPlayerData)_entityData).AbsorberData);
        _gameEvents = deps.GameEvents;
    }

    public override void AddTime(float timeUnits, ref int playState)
    {
        base.AddTime(timeUnits, ref playState);
        _absorberTrait.Tick(timeUnits);
    }

    public void StartAbsorption(IAbsorbableEntity entity)
    {
        string[] attributes = new string[] { };
        // Replace stuff
        if(entity.AbsorptionData.WillChangeView)
        {
            var view = Instantiate(entity.AbsorptionData.AbsorptionView, entity.AbsorptionData.ViewWillReplaceDefault ? _view.transform.parent: _view.transform);
            if(entity.AbsorptionData.ViewWillReplaceDefault)
            {
                _view.SetActive(false);
                _originalView = _view;
                _view = view;
            }
            else
            {
                _absorptionDecorationView = _originalView;
            }
        }

        if(entity.AbsorptionData.WillChangeBattleTrait)
        {
            _originalBattleTrait = _battleTrait;
            _battleTrait = new BattleTrait();
            _battleTrait.Init(_entityController, _mapController, entity.AbsorptionData.AttackReplace, this, _gameEvents);
            attributes = _battleTrait.FirstAttack.Attributes;            
        }

        if(entity.AbsorptionData.WillChangeHPTrait)
        {
            _originalHP = _hpTrait;
            _hpTrait = new HPTrait();
            _hpTrait.Init(this, entity.AbsorptionData.HPReplace, _events.Health);
        }

        if(entity.AbsorptionData.WillChangeMoveTrait)
        {
            _originalMoving = _movingTrait;
            _movingTrait = entity.AbsorptionData.MovingReplace.CreateRuntimeTrait();
        }

        _absorbEvents.SendAbsorptionHappened(entity, this, attributes);
    }

    public bool CanAbsorb(IAbsorbableEntity entity)
    {
        return false;
    }

    public void FinishAbsorption(bool manual = false)
    {
        // Restore all the things!
        if(_originalHP != null && _hpTrait != _originalHP)
        {
            _hpTrait = _originalHP;
            _originalHP = null;
        }

        if(_originalMoving != null && _movingTrait != _originalMoving)
        {
            _movingTrait = _originalMoving;
            _originalMoving = null;
        }

        if(_originalView != null && _view != _originalView)
        {
            GameObject.Destroy(_view);
            
            _view = _originalView;
            _view.SetActive(true);
            _originalView = null;
        }

        if(_absorptionDecorationView != null  && _absorptionDecorationView != null)
        {
            GameObject.Destroy(_absorptionDecorationView);
            _absorptionDecorationView = null;
        }

        if(_battleTrait != null && _battleTrait != _originalBattleTrait)
        {
            _battleTrait = _originalBattleTrait;
            _originalBattleTrait = null;
        }

        if(manual)
        {
            _absorbEvents.SendAbsorptionCancelled(this);
        }
        else
        {
            _absorbEvents.SendAbsorptionExpired(this);
        }
    }
}
