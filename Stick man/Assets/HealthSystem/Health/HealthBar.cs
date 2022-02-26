using Assets.Scripts.utils;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image HealthImage;

    private Transform _unitToFollow;
    private Vector2 _screenPos;
    private RectTransform _rt;
    private int _fullHealthEquivalent;

    public void Init(Transform hpTarget)
    {
        _rt = transform as RectTransform;
        _unitToFollow = hpTarget;
        _fullHealthEquivalent = (int)(HealthImage.transform as RectTransform).sizeDelta.x;
    }

    public void CalculateHealthBar(HealthChange hpChange, int maxHealth)
    {
        // TOOD: show damage => hpChange.Dmg

        var percent = __percent.What(hpChange.CalculatedHp, maxHealth);
        var currentHealthEquivalent = __percent.Find(_fullHealthEquivalent, percent);
        HealthImage.rectTransform.sizeDelta
            = new Vector2(currentHealthEquivalent, HealthImage.rectTransform.sizeDelta.y);
    }

    void LateUpdate()
    {
        if (_unitToFollow != null)
            FollowUnit();
    }

    private void FollowUnit()
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(_unitToFollow.transform.position);

        var canvasSize = Main._.CoreCamera.CanvasRt.sizeDelta;

        _screenPos = new Vector2(
            ((viewportPosition.x * canvasSize.x) - (canvasSize.x * 0.5f)),
            ((viewportPosition.y * canvasSize.y) - (canvasSize.y * 0.5f))
        );

        //now you can set the position of the ui element
        _rt.anchoredPosition = _screenPos;
    }
}
