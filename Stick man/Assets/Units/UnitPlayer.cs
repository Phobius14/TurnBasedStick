using System.Collections;
using System.Collections.Generic;
using Assets.HeadStart.Core;
using UnityEngine;

public class UnitPlayer : MonoBehaviour, IUnitControlable
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

        TurnGameView._.ActivateActions(
            _duringDecisionCallback,
            (int actionId) =>
            {
                ATTACK_ACTION action = (ATTACK_ACTION)actionId;
                switch (action)
                {
                    case ATTACK_ACTION.LIGHT:
                        _unit.Attack1(afterAttacking);
                        break;
                    case ATTACK_ACTION.MEDIUM:
                        afterAttacking((int)action);
                        // _unit.Attack2(afterAttacking);
                        break;
                    case ATTACK_ACTION.HARD:
                    default:
                        afterAttacking((int)action);
                        // _unit.Attack3(afterAttacking);
                        break;
                }
            }
        );
    }

    private void afterAttacking(int actionId)
    {
        _endTurnCallback(actionId);
    }
}
