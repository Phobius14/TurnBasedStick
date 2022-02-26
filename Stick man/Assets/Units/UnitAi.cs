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

        pickSomeoneFromTheEnemyTeam();

        int action = UnityEngine.Random.Range(0, 3);

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

    private void pickSomeoneFromTheEnemyTeam()
    {
        Debug.Log("_unit.Team: " + _unit.Team);
        var thisUnitEnemies = _unit.Team == 1
            ? (Main._.Game as TheGame).Team2
            : (Main._.Game as TheGame).Team1;

        int randomEnemyIndex = UnityEngine.Random.Range(0, thisUnitEnemies.Count);

        _unit.TargetEnemy = thisUnitEnemies[randomEnemyIndex];
    }
}
