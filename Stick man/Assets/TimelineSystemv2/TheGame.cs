using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGame : GameBase
{
    [Header("The Game")]

    public Timeline TimelineRef;

    public bool Team1AI;
    public List<Unit> Team1;
    public bool Team2AI;
    public List<Unit> Team2;
    private int _currentTurn = 0;
    public override void PreStartGame()
    {
        initTeam(Team1, 1, Team1AI);
        initTeam(Team2, 2, Team2AI);

        TimelineRef.Init(Team1, Team2);
        TurnGameView._.Init();

        StartGame();
    }

    public override void StartGame()
    {
        NextTurn();
    }

    private void initTeam(List<Unit> team, int teamId, bool ai)
    {
        int unitId = teamId * 100;
        foreach (Unit u in team)
        {
            u.ID = unitId;
            u.gameObject.name = "(" + u.ID + ") " + u.Name;
            u.Team = 1;
            u.AI = ai;
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
        TimelineRef.DragAllCloserToTurn((object obj) =>
        {
            IUnitControlable unitControlable;
            var ti = (obj as TimelineIndicator);
            var currentUnit = ti.Unit;

            if (currentUnit.AI)
            {
                unitControlable = currentUnit.gameObject.GetComponent<UnitAi>();
            }
            else
            {
                unitControlable = currentUnit.gameObject.GetComponent<UnitPlayer>();
            }

            unitControlable.DoTurn(duringDecision, endTurn);
        });
    }

    private void duringDecision()
    {
        Debug.Log("------------duringTurnAnimation()");
    }

    private void endTurn()
    {
        Debug.Log("-------------------End Turn");

        TimelineRef.MoveCurrentIndicatorToHisNextTurn(() =>
        {
            NextTurn();
            Debug.Log("Hit <b>A</b> to move NextTurn()");
        });
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            NextTurn();
        }
    }
}
