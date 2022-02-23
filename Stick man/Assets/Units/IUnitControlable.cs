using System.Collections;
using System.Collections.Generic;
using Assets.HeadStart.Core;
using UnityEngine;

public interface IUnitControlable
{
    void DoTurn(CoreCallback afterDecisionTurnCallback, CoreCallback endTurnCallback);
}
