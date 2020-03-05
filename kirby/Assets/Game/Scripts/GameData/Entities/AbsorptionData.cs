using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New AbsorptionData", menuName ="KRB_RL/Data/Entities/AbsorptionData")]
public class AbsorptionData: ScriptableObject
{
    public int Duration;

    public bool WillChangeView;
    public GameObject AbsorptionView; // Visual change to the player
    public bool ViewWillReplaceDefault; // Additive vs Replace

    public bool WillChangeBattleTrait;
    public BattleTraitData AttackReplace;

    public bool WillChangeHPTrait;
    public HPTraitData HPReplace;

    public bool WillChangeMoveTrait;
    public BaseMovingTraitData MovingReplace;

    public float HPRatioThreshold;
    public AbsorptionData NoData;
}
