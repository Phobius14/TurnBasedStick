using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.utils;

public class TurnBasedGame : GameBase, IGame
{
    private static TurnBasedGame _turnBasedGame;
    public static TurnBasedGame _ { get { return _turnBasedGame; } }

    [Header("TurnBasedGame")]
    public TurnTimeline TurnTimeline;
    public TurnGameView TurnGameView;
    //
    public int CurrentTurn;
    public List<Unit> Team1;
    public bool Team1AI;
    public List<Unit> Team2;
    public bool Team2AI;
    public List<TimelineUnit> TimelineUnits;

    public override void PreStartGame()
    {
        initTeam(Team1, 1, Team1AI);
        initTeam(Team2, 2, Team2AI);

        TurnTimeline.Init();
        TurnGameView.Init();

        StartGame();
    }

    public override void StartGame()
    {
        CurrentTurn = 0;
        // NextTurn();
    }

    private void initTeam(List<Unit> team, int teamId, bool ai)
    {
        int unitId = teamId * 100;
        foreach (Unit u in team)
        {
            u.ID = unitId;
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
        int nextTurn = TimelineUnits.FindIndex(tu => tu.IsOccupied);

        TurnTimeline.MoveAllCloserToTurn(nextTurn, () =>
        {
            // __debug.DebugList<TimelineUnit>(TurnBasedGame._.TimelineUnits, "timelineUnits: ");

            removeEmptyLines(nextTurn);

            TurnTimeline.RecalculateTime();

            IUnitControlable unitControlable;
            var currentUnit = TimelineUnits[0].Unit;
            if (currentUnit.AI)
            {
                unitControlable = currentUnit.gameObject.GetComponent<UnitAi>();
            }
            else
            {
                unitControlable = currentUnit.gameObject.GetComponent<UnitPlayer>();
            }

            unitControlable.DoTurn(
                duringTurnAnimation,
                () =>
                {
                    Debug.Log("Move Next Turn");
                    // NextTurn();
                }
            );
        });
    }

    private void removeEmptyLines(int nextTurn)
    {
        for (int i = 0; i < nextTurn; i++)
        {
            TimelineUnits.RemoveAt(0);
            TimelineUnits.Add(
                new TimelineUnit(TimelineUnits[TimelineUnits.Count - 1].TIndex + 1)
            );
        }

        foreach (var tu in TimelineUnits)
        {
            tu.TIndex = tu.TIndex - nextTurn;
        }
    }

    private void duringTurnAnimation()
    {
        int lastIndex = TimelineUnits.FindLastIndex(tu =>
        {
            if (tu.Unit)
            {
                return tu.Unit.ID == TimelineUnits[0].Unit.ID;
            }
            return false;
        });

        int tIndex = lastIndex + TimelineUnits[0].Unit.Initiative;
        Debug.Log("lastIndex: " + lastIndex);
        Debug.Log("tIndex: " + tIndex);
        // Debug.Log("TimelineUnits.Count: " + TimelineUnits.Count);
        if (tIndex >= TimelineUnits.Count)
        {
            var diff = tIndex - (TimelineUnits.Count - 1);
            for (int i = 0; i < diff; i++)
            {
                TimelineUnits.Add(new TimelineUnit(TimelineUnits.Count));
            }

            // __debug.DebugList<TimelineUnit>(TurnBasedGame._.TimelineUnits, "timelineUnits: ");
        }

        if (TimelineUnits[tIndex].IsOccupied)
        {
            TurnTimeline.MoveAllFromIndexAwayFromTurn(tIndex, () =>
            {
                MoveCircleAfterTurn(tIndex + 1);
            });
            return;
        }

        MoveCircleAfterTurn(tIndex);

        __debug.DebugList<TimelineUnit>(TurnBasedGame._.TimelineUnits, "timelineUnits: ");
    }

    private void MoveCircleAfterTurn(int tIndex)
    {
        TimelineUnits[0].CircleRt.anchoredPosition = new Vector2(
            TurnTimeline.UnitOfMeasure * tIndex, 0
        );

        TimelineUnits[tIndex] = TimelineUnits[0];
        TimelineUnits[tIndex].TIndex = tIndex;
        TimelineUnits[0] = new TimelineUnit(0);

        TurnTimeline.RecalculateTime();
    }

    void Awake()
    {
        _turnBasedGame = this;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            NextTurn();
        }
    }
}
