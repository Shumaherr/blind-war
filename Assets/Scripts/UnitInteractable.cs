using System;
using System.Linq;
using FMODUnity;
using TMPro;
using UnityEngine;

public class UnitInteractable : Unit
{
    public delegate void OnUnitSelectedDelegate(UnitInteractable unit);
    public event OnUnitSelectedDelegate OnUnitSelected;

    private Transform _dialogBox;

    private string _dialogText;

    private TextMeshPro _textMeshPro;

    public string DialogText
    {
        get => _dialogText;
        set
        {
            _dialogText = value;
            _textMeshPro.text = _dialogText;
        }
    }
    
    private void Start()
    {
        _dialogBox = transform.Find("Dialog/DialogBox");
        DeactivateDialog();
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        TurnManager.Instance.OnTurnChanged += ChangeTurn;
        Debug.Log("Unit: " + baseUnit.UnitType);
        InitMoves();
    }

    private void ChangeTurn(TurnStates newturn)
    {
        switch (newturn)
        {
            case TurnStates.PlayerTurn:
                InitMoves();
                break;
            case TurnStates.AITurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newturn), newturn, null);
        }
    }

    public void ChangeMoves(int moves = 1)
    {
        _moves = Math.Max(0, _moves - moves);
    }

    public bool CanMove()
    {
        return _moves > 0;
    }

    public override void InitMoves()
    {
        _moves = baseUnit.Moves;
    }

    public override Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }

    private void OnMouseDown()
    {
        if (!TurnManager.Instance.isPlayerTurn())
            return;
        RuntimeManager.PlayOneShot("event:/SFX/ui/select", transform.position);
        ControllerManager.Instance.SelectedUnit?.DeactivateDialog();
        ControllerManager.Instance.SelectedUnit = this;
        OnUnitSelected?.Invoke(this);
        Debug.Log("Unit clicked" + baseUnit.UnitType);
    }

    public void UsePerk()
    {
        var neighbourTypes = GameManager.Instance.GetNeighbourUnitTypes();
        if (neighbourTypes == null)
        {
            DialogText = "There is no enemy units";
            ActivateDialog();
            return;
        }

        var tempString = neighbourTypes.Aggregate("I feel ", (current, neighbourType) => current + neighbourType + " ");
        DialogText = tempString;
        ActivateDialog();
    }

    public void ActivateDialog()
    {
        _dialogBox.localScale = new Vector3(1, 1, 1);
    }

    public void DeactivateDialog()
    {
        _dialogBox.localScale = new Vector3(0, 0, 0);
    }

    public void SetDialogText(string newText)
    {
        _dialogText = newText;
    }
}