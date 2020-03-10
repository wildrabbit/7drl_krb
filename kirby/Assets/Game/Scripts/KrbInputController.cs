using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KrbInputController : BaseInputController
{
    public bool AbsorbCancel => dropAbsorption.Value;
    InputEntry dropAbsorption;

    public override void DoInit()
    {
        dropAbsorption = new InputEntry(KeyCode.U, _moveInputDelay);
    }
    protected override void InternalRead()
    {
        dropAbsorption.Read();
    }
}
