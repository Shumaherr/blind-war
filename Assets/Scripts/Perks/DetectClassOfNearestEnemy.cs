using System.Linq;
using TMPro;
using UnityEngine;

public class DetectClassOfNearestEnemy : Perk
{
    private Transform _dialogBox;
    private TextMeshProUGUI _dialogTextUI;
    public string DialogText { get; private set; }

    private void Awake()
    {
        //_dialogBox = transform.Find("Dialog/DialogBox");
        //_dialogTextUI = _dialogBox.GetComponent<TextMeshProUGUI>();
    }

    public override void Use()
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
        //_dialogBox.localScale = new Vector3(1, 1, 1);
        Debug.Log(DialogText);
    }

    public void DeactivateDialog()
    {
        //_dialogBox.localScale = new Vector3(0, 0, 0);
    }
}