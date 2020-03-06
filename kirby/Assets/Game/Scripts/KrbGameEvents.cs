using System;

public class KrbGameEvents: BaseGameEvents
{
    public class AbsorptionEvents
    {
        public event Action<IAbsorbableEntity> CanBeAbsorbed;
        public event Action<IAbsorbableEntity, IAbsorbingEntity, string[]> WasAbsorbed;
        public event Action<IAbsorbingEntity> AbsorptionExpired;
        public event Action<IAbsorbingEntity> AbsorptionCancelled;

        public void SendAbsorbReady(IAbsorbableEntity entity)
        {
            CanBeAbsorbed?.Invoke(entity);
        }

        public void SendAbsorptionHappened(IAbsorbableEntity target, IAbsorbingEntity absorber, string[] traitsGained)
        {
            WasAbsorbed?.Invoke(target, absorber, traitsGained);
        }

        public void SendAbsorptionExpired(IAbsorbingEntity absorber)
        {
            AbsorptionExpired?.Invoke(absorber);
        }

        public void SendAbsorptionCancelled(IAbsorbingEntity absorber)
        {
            AbsorptionCancelled?.Invoke(absorber);
        }
    }

    public AbsorptionEvents Absorption;

    public KrbGameEvents():base()
    {
        Absorption = new AbsorptionEvents();
    }
}
