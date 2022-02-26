using System;
using UnityEngine;

public class PickButton : MonoBehaviour
{
    private Action _callback;

    internal void Init(Action callback)
    {
        _callback = callback;
    }

    void OnMouseDown()
    {
        if (_callback == null) { return; }
        
        _callback();
    }
}
