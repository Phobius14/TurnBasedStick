using Assets.HeadStart.Core;
using UnityEngine;
using UnityEngine.UI;

public class TurnGameView : MonoBehaviour
{
    private static TurnGameView _turnGameView;
    public static TurnGameView _ { get { return _turnGameView; } }
    void Awake() { _turnGameView = this; }

    public Button LightAttackButton;
    public Button MediumAttackButton;
    public Button HardAttackButton;
    private CoreIdCallback _onHoverAction;
    private CoreIdCallback _onPickedAction;

    public void Init()
    {
        activateButtons(false);
    }

    public void ActivateActions(CoreIdCallback onHoverAction, CoreIdCallback onPickedAction)
    {
        _onHoverAction = onHoverAction;
        _onPickedAction = onPickedAction;

        activateButtons();
    }

    public void LightAttack()
    {
        _onPickedAction((int)ATTACK_ACTION.LIGHT);
        activateButtons(false);
    }

    public void MediumAttack()
    {
        _onPickedAction((int)ATTACK_ACTION.MEDIUM);
        activateButtons(false);
    }

    public void HardAttack()
    {
        _onPickedAction((int)ATTACK_ACTION.HARD);
        activateButtons(false);
    }

    public void HoverAttack(int attackType)
    {
        _onHoverAction(attackType);
    }

    private void activateButtons(bool active = true)
    {
        LightAttackButton.gameObject.SetActive(active);
        MediumAttackButton.gameObject.SetActive(active);
        HardAttackButton.gameObject.SetActive(active);
    }
}
