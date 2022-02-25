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

    void Start()
    {
        AttackText.gameObject.SetActive(false);
    }

    public override void Attack1(CoreIdCallback afterAttack)
    {
        _afterAttack = afterAttack;

        AttackText.gameObject.SetActive(true);

        Debug.Log("<b>TestEnemy</b> -> <b>Attack 1</b> !!");

        _initialScale = AttackText.transform.localScale;
        var toScale = _initialScale * 2;
        var twId = LeanTween.scale(
            AttackText.gameObject,
            toScale,
            ATTACK_1_TIME / 2
        ).id;

        LeanTween.descr(twId).setEase(LeanTweenType.easeOutQuart);
        LeanTween.descr(twId).setOnComplete(() =>
        {
            var twId = LeanTween.scale(
                AttackText.gameObject,
                _initialScale,
                ATTACK_1_TIME / 2
            ).id;
            LeanTween.descr(twId).setOnComplete(() =>
            {
                AttackText.gameObject.SetActive(false);

                _afterAttack(1);
            });
        });
    }
}
