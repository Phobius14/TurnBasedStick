using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class UnitAi : MonoBehaviour, IUnitControlable
{
    private Unit _unit;
    private CoreIdCallback _duringDecisionCallback;
    private CoreIdCallback _endTurnCallback;

    public void DoTurn(CoreIdCallback duringDecisionCallback, CoreIdCallback endTurnCallback)
    {
        _duringDecisionCallback = duringDecisionCallback;
        _endTurnCallback = endTurnCallback;
        if (_unit == null)
        {
            _unit = gameObject.GetComponent<Unit>();
        }

        // TODO: make AI make a decision
        _duringDecisionCallback((int)ATTACK_ACTION.LIGHT);

        _unit.Attack1((int actionId) =>
        {
            _endTurnCallback(actionId);
        });
    }
}
