using Assets.HeadStart.Core;
using UnityEngine;

public class Magician : Unit
{
    [Header("TestEnemy")]
    public Transform AttackText;
    //
    private Vector3 _initialScale;
    //
    private readonly float ATTACK_1_TIME = 0.3f;

    void Start()
    {
        AttackText.gameObject.SetActive(false);
    }

    public override void Attack1(CoreIdCallback afterAttack)
    {
        _afterAttack = afterAttack;

        AttackText.gameObject.SetActive(true);

        Debug.Log("<b>Magician</b>" + " -> <b>Attack 1</b> !!");

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

                _afterAttack((int)ATTACK_ACTION.LIGHT);
            });
        });
    }

    public override void Attack2(CoreIdCallback afterAttack = null)
    {
        _afterAttack = afterAttack;

        AttackText.gameObject.SetActive(true);

        Debug.Log("<b>Magician</b>" + " -> <b>Attack 2</b> !!");

        _initialScale = AttackText.transform.localScale;
        var toScale = _initialScale * 2;
        var twId = LeanTween.scale(
            AttackText.gameObject,
            toScale,
            ATTACK_1_TIME / 1.5f
        ).id;

        LeanTween.descr(twId).setEase(LeanTweenType.easeOutQuart);
        LeanTween.descr(twId).setOnComplete(() =>
        {
            var twId = LeanTween.scale(
                AttackText.gameObject,
                _initialScale,
                ATTACK_1_TIME / 1.5f
            ).id;
            LeanTween.descr(twId).setOnComplete(() =>
            {
                AttackText.gameObject.SetActive(false);

                if (afterAttack == null) { return; }
                _afterAttack((int)ATTACK_ACTION.MEDIUM);
            });
        });
    }

    public override void Attack3(CoreIdCallback afterAttack = null)
    {
        _afterAttack = afterAttack;

        AttackText.gameObject.SetActive(true);

        Debug.Log("<b>Magician</b>" + " -> <b>Attack 3</b> !!");

        _initialScale = AttackText.transform.localScale;
        var toScale = _initialScale * 2;
        var twId = LeanTween.scale(
            AttackText.gameObject,
            toScale,
            ATTACK_1_TIME / 1.33f
        ).id;

        LeanTween.descr(twId).setEase(LeanTweenType.easeOutQuart);
        LeanTween.descr(twId).setOnComplete(() =>
        {
            var twId = LeanTween.scale(
                AttackText.gameObject,
                _initialScale,
                ATTACK_1_TIME / 1.33f
            ).id;
            LeanTween.descr(twId).setOnComplete(() =>
            {
                AttackText.gameObject.SetActive(false);

                if (afterAttack == null) { return; }
                _afterAttack((int)ATTACK_ACTION.HARD);
            });
        });
    }

    public override void DelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {
        if (actionId == 1)
        {
            Attack1(afterAttack);
        }
        else if (actionId == 2)
        {
            Attack2(afterAttack);
        }
        else
        {
            Attack3(afterAttack);
        }
    }
}
