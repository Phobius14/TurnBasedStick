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
            _afterAttack(0);
        }, 1f);
    }

    public virtual void Attack2(CoreIdCallback afterAttack)
    {
        _afterAttack = afterAttack;

        Debug.Log(gameObject.name + " -> <b>Attack 2</b> !!");
        __.Time.RxWait(() =>
        {
            _afterAttack(1);
        }, 1f);
    }

    public virtual void Attack3(CoreIdCallback afterAttack)
    {
        _afterAttack = afterAttack;

        Debug.Log(gameObject.name + " -> <b>Attack 3</b> !!");
        __.Time.RxWait(() =>
        {
            _afterAttack(2);
        }, 1f);
    }
}
