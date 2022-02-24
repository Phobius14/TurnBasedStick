using System.Collections;
using System.Collections.Generic;
using Assets.HeadStart.Core;
using UnityEngine;

public class UnitPlayer : MonoBehaviour, IUnitControlable
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

        TurnGameView._.ActivateActions((int actionId) =>
        {
            _duringDecisionCallback();

            ATTACK_ACTION action = (ATTACK_ACTION)actionId;
            switch (action)
            {
                case ATTACK_ACTION.LIGHT:
                    _unit.Attack1(afterAttacking);
                    break;
                case ATTACK_ACTION.MEDIUM:
                    _unit.Attack2(afterAttacking);
                    break;
                case ATTACK_ACTION.HARD:
                default:
                    _unit.Attack3(afterAttacking);
                    break;
            }
        });
    }

    private void afterAttacking()
    {
        _endTurnCallback();
    }
}
