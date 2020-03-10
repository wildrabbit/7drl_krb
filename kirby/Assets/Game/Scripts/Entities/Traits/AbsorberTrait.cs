using System;

public interface IAbsorbingEntity: IEntity
{
    string[] Attributes { get; }
    AbsorberTrait AbsorberTrait { get; }
    
    bool CanAbsorb(IAbsorbableEntity entity);
    void FinishAbsorption();
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
    KrbGameEvents.AbsorptionEvents _absorbEvents;

    public void Init(IAbsorbingEntity owner, AbsorberTraitData data, KrbGameEvents.AbsorptionEvents absorbEvents)
    {
        _data = data;
        _activeAbsorptionData = _data.NoAbsorption;
        _owner = owner;
        _absorbEvents = absorbEvents;
    }

    public void Tick(float units)
    {
        if (IsAbsorbing)
        {
            _elapsed += units;
            if (_elapsed >= _activeAbsorptionData.Duration)
            {
                ResetAbsorption(manual:false);
            }
        }
    }

    public bool TryAbsorb(IAbsorbableEntity target)
    {
        if (target.AbsorptionData == null || target.AbsorptionData == _data.NoAbsorption)
        {
            _absorbEvents.SendAbsorbFailed(_owner, false);
            return false;
        }

        if (!IsAbsorbing && target.CanBeAbsorbedBy(_owner))
        {
            _activeAbsorptionData = target.AbsorptionData;
            _owner.StartAbsorption(target);
            target.BeAbsorbed(_owner);
            _elapsed = 0.0f;

            _absorbEvents.SendAbsorptionHappened(target, _owner, _owner.Attributes);
            return true;
        }
        if(IsAbsorbing)
        {
            _absorbEvents.SendAbsorbFailed(_owner, true);
        }
        return false;
    }

    public void ResetAbsorption(bool manual)
    {
        _owner.FinishAbsorption();
        _elapsed = -1.0f;
        _activeAbsorptionData = _data.NoAbsorption;
        if(manual)
        {
            _absorbEvents.SendAbsorptionCancelled(_owner);
        }
        else
        {
            _absorbEvents.SendAbsorptionExpired(_owner);

        }
    }
}