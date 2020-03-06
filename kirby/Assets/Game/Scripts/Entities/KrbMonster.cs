using System;
using UnityEngine;

public class KrbMonster : Monster, IAbsorbableEntity
{
    KrbMonsterData _krbMonsterData;
    AbsorptionData _absorptionData;
    private KrbGameEvents.AbsorptionEvents _absorbEvents;
    BaseGameEvents.HPEvents _healthEvents;

    GameObject _absorbableView;

    public AbsorptionData AbsorptionData => _absorptionData;

    bool HasTrait()
    {
        return _absorptionData != null;
    }

    protected override void DoInit(BaseEntityDependencies deps)
    {
        base.DoInit(deps);

        _absorbEvents = ((KrbGameEvents)deps.GameEvents).Absorption;

        _healthEvents = deps.GameEvents.Health;

        _healthEvents.HealthEvent += OnHealthEvent;

        _krbMonsterData = (KrbMonsterData)_monsterData;
        _absorptionData = _krbMonsterData.AbsorptionData;
    }

    protected override void ViewCreated()
    {
        if (_absorptionData != null && !_absorptionData.Equals(_absorptionData.NoData))
        {
            if (_krbMonsterData.AbsorbablePrefab != null)
            {
                 _absorbableView = Instantiate(_krbMonsterData.AbsorbablePrefab, _view.transform);
                _absorbableView.SetActive(CanBeAbsorbed());
            }
        }
    }

    public override void OnDestroyed()
    {
        _healthEvents.HealthEvent -= OnHealthEvent;
    }

    private void OnHealthEvent(IHealthTrackingEntity entity, int delta, bool arg3, bool arg4, bool arg5)
    {
        if (entity != this)
        {
            return;
        }

        bool canBeAbsorbed = CanBeAbsorbed();
        if (_absorbableView != null)
        {
            _absorbableView.SetActive(canBeAbsorbed);
        }

        if (canBeAbsorbed)
        {
            _absorbEvents.SendAbsorbReady(this);
        }
    }

    public void BeAbsorbed(IAbsorbingEntity absorber)
    {
        TakeDamage(_hpTrait.HP);
    }

    public bool CanBeAbsorbedBy(IAbsorbingEntity absorber)
    {
        if (_mapController.Distance(absorber.Coords, Coords) > absorber.AbsorberTrait.Data.AbsorbDistance)
        {
            return false;
        }

        if (_absorptionData == absorber.AbsorberTrait.Data.NoAbsorption)
        {
            return false;
        }

        return _hpTrait.HPRatio <= absorber.AbsorberTrait.Data.HPPercentToEnableAbsorb;        
    }

    public bool CanBeAbsorbed()
    {
        if(_absorptionData == null || _absorptionData.Equals(_absorptionData.NoData))
        {
            return false;
        }

        return (_hpTrait.HPRatio > 0 && _hpTrait.HPRatio <= _absorptionData.HPRatioThreshold);        
    }
}
