using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class UnitAi : MonoBehaviour, IUnitControlable
{
    private Unit _unit;
    private CoreCallback _afterDecisionTurnCallback;
    private CoreCallback _endTurnCallback;

    public void DoTurn(CoreCallback afterDecisionTurnCallback, CoreCallback endTurnCallback)
    {
        _afterDecisionTurnCallback = afterDecisionTurnCallback;
        _endTurnCallback = endTurnCallback;
        if (_unit == null)
        {
            _unit = gameObject.GetComponent<Unit>();
        }

        // TODO: make AI make a decision
        _afterDecisionTurnCallback();

        _unit.Attack1(() =>
        {
            _endTurnCallback();
        });
    }
}
