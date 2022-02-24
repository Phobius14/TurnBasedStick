using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class UnitAi : MonoBehaviour, IUnitControlable
{
    private Unit _unit;
    private CoreCallback _duringDecisionCallback;
    private CoreCallback _endTurnCallback;

    public void DoTurn(CoreCallback duringDecisionCallback, CoreCallback endTurnCallback)
    {
        _duringDecisionCallback = duringDecisionCallback;
        _endTurnCallback = endTurnCallback;
        if (_unit == null)
        {
            _unit = gameObject.GetComponent<Unit>();
        }

        // TODO: make AI make a decision
        _duringDecisionCallback();

        _unit.Attack1(() =>
        {
            _endTurnCallback();
        });
    }
}
