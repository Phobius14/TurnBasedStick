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

        pickSomeoneFromTheEnemyTeam();

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
                        break;
                    case ATTACK_ACTION.HARD:
                    default:
                        afterAttacking((int)action);
                        break;
                }
            }
        );
    }

    private void pickSomeoneFromTheEnemyTeam()
    {
        var thisUnitEnemies = _unit.Team == 1
            ? (Main._.Game as TheGame).Team2
            : (Main._.Game as TheGame).Team1;

        foreach (var u in thisUnitEnemies)
        {
            u.ActivatePickButton(onPickedEnemy);
        }
    }

    private void onPickedEnemy(Unit unit)
    {
        _unit.TargetEnemy = unit;

        var thisUnitEnemies = _unit.Team == 1
            ? (Main._.Game as TheGame).Team2
            : (Main._.Game as TheGame).Team1;

        foreach (var u in thisUnitEnemies)
        {
            u.HidePickButton();
        }

        mergeMapEndingTurn();
    }

    private void afterAttacking(int actionId)
    {
        _pickedActionId = actionId;
        mergeMapEndingTurn();
    }

    private int _pickedActionId;
    private int mergeMap = 0;
    private void mergeMapEndingTurn()
    {
        mergeMap++;
        if (mergeMap < 2) { return; }

        mergeMap = 0;
        _endTurnCallback(_pickedActionId);
    }
}
