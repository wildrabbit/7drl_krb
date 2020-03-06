using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbsorbingEntity: IEntity
{
    AbsorberTrait AbsorberTrait { get; }
    
    bool CanAbsorb(IAbsorbableEntity entity);
    void FinishAbsorption(bool manual);
    void StartAbsorption(IAbsorbableEntity entity);
}

public interface IAbsorbableEntity: IEntity
{
    AbsorptionData AbsorptionData { get; }
    bool CanBeAbsorbedBy(IAbsorbingEntity absorber);
    bool CanBeAbsorbed();
    void BeAbsorbed(IAbsorbingEntity absorber);
}

[Serializable]
public class AbsorberTrait
{
    public AbsorberTraitData Data => _data;
    public bool IsAbsorbing => _activeAbsorptionData != null && _activeAbsorptionData != Data.NoAbsorption && _elapsed >= 0 && _elapsed < _activeAbsorptionData.Duration;

    IAbsorbingEntity _owner;

    AbsorberTraitData _data;
    float _elapsed;
    AbsorptionData _activeAbsorptionData;

    public void Init(IAbsorbingEntity owner, AbsorberTraitData data)
    {
        _data = data;
        _activeAbsorptionData = _data.NoAbsorption;
        _owner = owner;
    }

    public void Tick(float units)
    {
        if(IsAbsorbing)
        {
            _elapsed += units;
            if(_elapsed >= _activeAbsorptionData.Duration)
            {
                _owner.FinishAbsorption(manual:false);
            }
        }
    }

    public bool TryAbsorb(IAbsorbableEntity target)
    {
        if(target.AbsorptionData == null || target.AbsorptionData == _data.NoAbsorption)
        {
            return false;
        }

        if(target.CanBeAbsorbedBy(_owner))
        {
            _activeAbsorptionData = target.AbsorptionData;
            _owner.StartAbsorption(target);
            target.BeAbsorbed(_owner);
            _elapsed = 0.0f;
            return true;
        }
        return false;
    }

    public void ResetAbsorption()
    {
        _elapsed = -1.0f;
        _activeAbsorptionData = _data.NoAbsorption;
    }
}
