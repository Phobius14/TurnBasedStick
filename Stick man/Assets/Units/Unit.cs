using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public int Haste;
    public int Health;
    public int Damage;
    internal int Team;
    internal bool AI;
    internal int ID;
    public UnitSettings UnitSettings;
    protected CoreIdCallback _afterAttack;

    public virtual void Attack1(CoreIdCallback afterAttack)
    {
        _afterAttack = afterAttack;

        Debug.Log(gameObject.name + " -> <b>Attack 1</b> !!");
        __.Time.RxWait(() =>
        {
            _afterAttack((int)ATTACK_ACTION.LIGHT);
        }, 1f);
    }

    public virtual void Attack2(CoreIdCallback afterAttack = null)
    {
        _afterAttack = afterAttack;

        Debug.Log(gameObject.name + " -> <b>Attack 2</b> !!");

        if (afterAttack == null) { return; }
        __.Time.RxWait(() =>
        {
            _afterAttack((int)ATTACK_ACTION.MEDIUM);
        }, 1f);
    }

    public virtual void Attack3(CoreIdCallback afterAttack = null)
    {
        _afterAttack = afterAttack;

        Debug.Log(gameObject.name + " -> <b>Attack 3</b> !!");

        if (afterAttack == null) { return; }
        __.Time.RxWait(() =>
        {
            _afterAttack((int)ATTACK_ACTION.HARD);
        }, 1f);
    }

    public virtual void DelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {
        
    }
}
