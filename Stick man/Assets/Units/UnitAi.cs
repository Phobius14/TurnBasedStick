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

        int action = UnityEngine.Random.Range(0, 3);

        // TODO: make AI make a decision
        // _duringDecisionCallback((int)ATTACK_ACTION.LIGHT);

        __.Time.RxWait(() =>
        {
            _duringDecisionCallback(action);

            __.Time.RxWait(() =>
            {
                _unit.DelayedAttack(action + 1, (int actionId) =>
                {
                    _endTurnCallback(actionId);
                });
            }, Timeline.MOVE_INDICATORS_TIME);

        }, Timeline.MOVE_INDICATORS_TIME);


    }
}
