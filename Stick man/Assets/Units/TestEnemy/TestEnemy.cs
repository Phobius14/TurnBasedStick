using System.Collections;
using System.Collections.Generic;
using Assets.HeadStart.Core;
using UnityEngine;

public class TestEnemy : Unit
{
    [Header("TestEnemy")]
    public Transform AttackText;
    //
    private Vector3 _initialScale;
    //
    private readonly float ATTACK_1_TIME = 0.5f;

    public override void Init(int unitId, int team, bool ai)
    {
        base.Init(unitId, team, ai);

        AttackText.gameObject.SetActive(false);
    }

    public override void Attack1(CoreIdCallback afterAttack)
    {
        _afterAttack = afterAttack;

        defaultAttack(0);
    }

    public override void Attack2(CoreIdCallback afterAttack = null)
    {
        _afterAttack = afterAttack;

        defaultAttack(1);
    }

    public override void Attack3(CoreIdCallback afterAttack = null)
    {
        _afterAttack = afterAttack;

        defaultAttack(2);
    }

    private void defaultAttack(int attackType)
    {
        AttackText.gameObject.SetActive(true);
        _attack_duration = ATTACK_1_TIME * TheGame.TIME_m;

        Debug.Log(
            "<b>" + gameObject.name + "</b>" +
            " -> Attack " + (attackType + 1) + " -> " +
            "<b>" + TargetEnemy.gameObject.name + "</b>"
        );

        MoveToAttack();

        _initialScale = AttackText.transform.localScale;
        var toScale = _initialScale * 2;
        var twId = LeanTween.scale(
            AttackText.gameObject,
            toScale,
            _attack_duration
        ).id;

        LeanTween.descr(twId).setEase(LeanTweenType.easeOutQuart);
        LeanTween.descr(twId).setOnComplete(() =>
        {
            DamageTargetEnemy(attackType);

            var twId = LeanTween.scale(
                AttackText.gameObject,
                _initialScale,
                _attack_duration
            ).id;
            LeanTween.descr(twId).setOnComplete(() =>
            {
                AttackText.gameObject.SetActive(false);

                if (_afterAttack == null) { return; }
                _afterAttack(attackType);
            });
        });
    }

    public override void DamageTargetEnemy(int multiply)
    {
        base.DamageTargetEnemy(multiply);
    }

    public override void DoDelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {
        if (actionId == 2)
        {
            Attack2(afterAttack);
        }
        else if (actionId == 3)
        {
            Attack3(afterAttack);
        }
    }

    public override void SetupDelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {
        if (actionId >= 2)
        {
            afterAttack(actionId - 1);
        }
    }
}
