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
                if (action == 0)
                {
                    _unit.Attack1(_endTurnCallback);
                }
                else
                {
                    _unit.SetupDelayedAttack(action + 1, _endTurnCallback);
                }
            }, Timeline.MOVE_INDICATORS_TIME * TheGame.TIME_m);

        }, Timeline.MOVE_INDICATORS_TIME * TheGame.TIME_m);


    }

    private void pickSomeoneFromTheEnemyTeam()
    {
        var thisUnitEnemies = _unit.Team == 1
            ? (Main._.Game as TheGame).Team2
            : (Main._.Game as TheGame).Team1;

        int randomEnemyIndex = UnityEngine.Random.Range(0, thisUnitEnemies.Count);

        _unit.TargetEnemy = thisUnitEnemies[randomEnemyIndex];
    }
}
