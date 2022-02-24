using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.HeadStart.Core;

public class TurnTimeline : MonoBehaviour
{
    public RectTransform MaskRt;
    public GameObject UnitCircle;
    public RectTransform LightAttackLane;
    public RectTransform MediumAttackLane;
    public RectTransform HardAttackLane;
    public float Width;
    public float Height;
    public float UnitOfMeasure;
    public int UnitsInTimeline;
    //
    private int? _moveCirclesTwId;
    private int? _moveToTurn;
    private CoreCallback _afterMove;
    private Vector2 _lastRelativeNewPos;
    private RectTransform _pivotCircle;
    private GameObject _valueTweenGo;

    public void Init()
    {
        _valueTweenGo = new GameObject();

        calculateInitialWidths();

        createTimelineUnits();

        foreach (Unit unit in TurnBasedGame._.Team1)
        {
            // int turns = UnitsInTimeline / unit.Initiative;
            int turns = 2;
            for (int turn = 1; turn <= turns; turn++)
            {
                CreateCircles(unit, turn, Color.green);
            }
        }

        foreach (Unit unit in TurnBasedGame._.Team2)
        {
            // int turns = (UnitsInTimeline) / unit.Initiative;
            int turns = 2;
            for (int turn = 1; turn <= turns; turn++)
            {
                CreateCircles(unit, turn, Color.red);
            }
        }
    }

    public void MoveAllCloserToTurn(int toTurn, CoreCallback afterMove)
    {
        _moveToTurn = toTurn;
        _afterMove = afterMove;
        var timelineUnits = TurnBasedGame._.TimelineUnits;
        _pivotCircle = timelineUnits[_moveToTurn.Value].CircleRt;
        var toPos = new Vector2(0, _pivotCircle.anchoredPosition.y);

        _moveCirclesTwId = LeanTween.value(
            _valueTweenGo,
            _pivotCircle.anchoredPosition,
            toPos,
            0.3f
        ).id;

        LeanTween.descr(_moveCirclesTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_moveCirclesTwId.Value)
            .setOnUpdate((Vector2 newPos) =>
            {
                var diff = _pivotCircle.anchoredPosition - newPos;
                foreach (var tUnit in TurnBasedGame._.TimelineUnits)
                {
                    if (tUnit.IsOccupied)
                    {
                        tUnit.CircleRt.anchoredPosition
                            = tUnit.CircleRt.anchoredPosition - diff;
                    }
                }
            });
        LeanTween.descr(_moveCirclesTwId.Value)
            .setOnComplete(() =>
            {
                _afterMove();
            });
    }

    public void MoveAllFromIndexAwayFromTurn(int fromIndex, CoreCallback afterMove)
    {
        _afterMove = afterMove;
        var timelineUnits = TurnBasedGame._.TimelineUnits;
        var fromPos = Vector2.zero;
        var toPos = new Vector2(UnitOfMeasure, 0);
        _lastRelativeNewPos = Vector2.zero;

        _moveCirclesTwId = LeanTween.value(
            _valueTweenGo,
            fromPos,
            toPos,
            0.3f
        ).id;

        LeanTween.descr(_moveCirclesTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_moveCirclesTwId.Value)
            .setOnUpdate((Vector2 newPos) =>
            {
                var pos = newPos - _lastRelativeNewPos;
                _lastRelativeNewPos = newPos;
                foreach (var tUnit in TurnBasedGame._.TimelineUnits)
                {
                    if (tUnit.TIndex >= fromIndex && tUnit.IsOccupied)
                    {
                        var newAnchor = tUnit.CircleRt.anchoredPosition - pos;
                        tUnit.CircleRt.anchoredPosition = newAnchor;
                    }
                }
            });
        LeanTween.descr(_moveCirclesTwId.Value)
            .setOnComplete(() =>
            {
                _afterMove();
            });
    }

    public void RecalculateTime()
    {
        // __debug.DebugList<TimelineUnit>(TurnBasedGame._.TimelineUnits, "timelineUnits: ");

        int siblingIndex = 0;

        foreach (var tu in TurnBasedGame._.TimelineUnits)
        {
            if (tu.CircleRt != null)
            {
                tu.CircleRt.gameObject.name = renameCircle(tu.TIndex, tu.Unit);
                tu.CircleRt.SetSiblingIndex(siblingIndex);
                siblingIndex++;
            }
        }

        // __debug.DebugList<TimelineUnit>(TurnBasedGame._.TimelineUnits, "timelineUnits: ");
    }

    private void calculateInitialWidths()
    {
        var canasRt = Main._.CoreCamera.Canvas.GetComponent<RectTransform>();

        float h = canasRt.rect.height;
        float w = canasRt.rect.width;

        Width = w * 0.6f;
        Height = h * 0.2f;

        int nrForeseeableTurns = 2;
        int maxInitiative = getMaxInitiative();

        UnitsInTimeline = (nrForeseeableTurns * maxInitiative);
        UnitOfMeasure = Width / UnitsInTimeline;

        LightAttackLane.sizeDelta = new Vector2(
            Width * 2, Height / 3
        );
        MediumAttackLane.sizeDelta = new Vector2(
            Width * 2, Height / 3
        );
        HardAttackLane.sizeDelta = new Vector2(
            Width * 2, Height / 3
        );
    }

    private int getMaxInitiative()
    {
        int t1Max = 0;
        int t2Max = 0;
        if (TurnBasedGame._.Team1 != null)
        {
            t1Max = TurnBasedGame._.Team1.Max(u => u.Haste);
        }
        if (TurnBasedGame._.Team2 != null)
        {
            t2Max = TurnBasedGame._.Team2.Max(u => u.Haste);
        }
        return t1Max > t2Max ? t1Max : t2Max;
    }

    private void createTimelineUnits()
    {
        TurnBasedGame._.TimelineUnits = new List<TimelineUnit>();
        for (int i = 0; i < UnitsInTimeline; i++)
        {
            TurnBasedGame._.TimelineUnits.Add(new TimelineUnit(i));
        }
    }

    private void CreateCircles(Unit unit, int turn, Color color)
    {
        var go = Instantiate(UnitCircle, Vector2.zero, Quaternion.identity, MaskRt);
        var unitCircleRt = (go.transform as RectTransform);

        unitCircleRt.GetComponent<Image>().color = color;

        int unitTurn = unit.Haste * turn;

        go.name = renameCircle(unitTurn, unit);

        TurnBasedGame._.TimelineUnits[unitTurn - 1].IsOccupied = true;
        TurnBasedGame._.TimelineUnits[unitTurn - 1].Unit = unit;
        TurnBasedGame._.TimelineUnits[unitTurn - 1].CircleRt = unitCircleRt;

        unitCircleRt.anchoredPosition = new Vector2(UnitOfMeasure * unitTurn, 0);
        unitCircleRt.sizeDelta = new Vector2(Height / 3, Height / 3);
    }

    private string renameCircle(int unitTurn, Unit unit)
    {
        return "(" + unitTurn + ") " + unit.Name;
    }
}
