using System.Collections.Generic;
using UnityEngine;

public class TheGame : GameBase
{
    [Header("The Game")]

    public Timeline TimelineRef;
    public float AutoTimeDelay;
    public static float TIME_m = 0;
    public bool StopTurns;
    public bool Team1AI;
    public List<Unit> Team1;
    public bool Team2AI;
    public List<Unit> Team2;
    public override void PreStartGame()
    {
        initTeam(Team1, 1, Team1AI);
        initTeam(Team2, 2, Team2AI);

        TimelineRef.Init(Team1, Team2);
        TurnGameView._.Init();

        TIME_m = AutoTimeDelay;

        StartGame();
    }

    public override void StartGame()
    {
        NextTurn();
    }

    public void OnUnitDeath(Unit unit)
    {
        int indexInTeam = -1;
        int unitTeam = unit.Team;
        if (unitTeam == 1)
        {
            indexInTeam = Team1.FindIndex(u => u.ID == unit.ID);
        }
        else
        {
            indexInTeam = Team2.FindIndex(u => u.ID == unit.ID);
        }

        TimelineRef.RemoveIndicator(unit);

        Destroy(unit.gameObject);

        if (unitTeam == 1)
        {
            Team1.RemoveAt(indexInTeam);
            if (Team1.Count == 0)
            {
                StopTurns = true;
            }
        }
        else
        {
            Team2.RemoveAt(indexInTeam);
            if (Team2.Count == 0)
            {
                StopTurns = true;
            }
        }
    }

    private void initTeam(List<Unit> team, int teamId, bool ai)
    {
        int unitId = teamId * 100;
        foreach (Unit u in team)
        {
            u.Init(unitId, teamId, ai);

            if (u.AI)
            {
                u.gameObject.AddComponent<UnitAi>();
            }
            else
            {
                u.gameObject.AddComponent<UnitPlayer>();
            }
            unitId++;
        }
    }

    private void NextTurn()
    {
        if (StopTurns) { return; }

        TimelineRef.DragAllCloserToTurn((object obj) =>
        {
            var ti = (obj as ITimelineIndicator);

            if (ti.Type == INDICATOR_TYPE.ATTACK)
            {
                ti.Unit.DoDelayedAttack(ti.Level, (int actionId) =>
                {
                    mergeMapEndingTurn();
                });
                TimelineRef.DoDelayedAttack(ti, mergeMapEndingTurn);
                ti.Type = INDICATOR_TYPE.UNIT;
                return;
            }

            IUnitControlable unitControlable;
            var currentUnit = ti.Unit;
            if (currentUnit.AI)
            {
                unitControlable = currentUnit.gameObject.GetComponent<UnitAi>();
            }
            else
            {
                unitControlable = currentUnit.gameObject.GetComponent<UnitPlayer>();
            }
            TimelineRef.ShowDelayedAttack(ATTACK_ACTION.LIGHT);
            unitControlable.DoTurn(duringDecision, endTurn);
        });
    }

    private void duringDecision(int actionId)
    {
        TimelineRef.ShowDelayedAttack((ATTACK_ACTION)actionId);
        // Debug.Log("------[" + actionId + "]------duringTurnAnimation()");
    }

    private void endTurn(int actionId)
    {
        // Debug.Log("---------[" + actionId + "]----------End Turn");

        TimelineRef.SetLevelToCurrent(actionId + 1);

        // TODO: write proper if
        if (actionId == 1 || actionId == 2)
        {
            // TODO: to think about making space for attack
            // - there is space in the first row, so we can drag all to where it meets attackIndctr
            // - and push all after
            TimelineRef.SetCurrentIndicatorType(INDICATOR_TYPE.ATTACK);
            TimelineRef.ResetTimeline();
            bool indicatorsOverlap = TimelineRef.CheckIfIndicatorsOverlap(endingTurn);
            if (indicatorsOverlap == false)
            {
                endingTurn();
            }
            return;
        }

        TimelineRef.SetCurrentIndicatorType(INDICATOR_TYPE.UNIT);
        TimelineRef.MoveCurrentIndicatorToHisNextTurn(endingTurn);
    }

    private int mergeMap = 0;
    private void mergeMapEndingTurn()
    {
        mergeMap++;
        if (mergeMap < 2) { return; }

        mergeMap = 0;
        endingTurn();
    }

    private void endingTurn()
    {
        NextTurn();
        // Debug.Log("Hit <b>A</b> to move NextTurn()");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            NextTurn();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StopTurns = !StopTurns;
        }
    }
}
