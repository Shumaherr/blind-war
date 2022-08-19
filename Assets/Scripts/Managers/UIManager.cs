using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private List<RawImage> iconsSlots;

    // Start is called before the first frame update
    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
        iconsSlots.ForEach(item => item.GetComponent<ItemSelector>().OnItemSelected += OnOnItemSelected);
        HideInventory();
    }

    private void OnOnItemSelected(int itemnum)
    {
        if (ControllerManager.Instance.SelectedUnit.Inventory.Count > 0)
            ControllerManager.Instance.SelectedUnit.Inventory[itemnum].TryToUse();
    }

    public void HideInventory()
    {
        iconsSlots.ForEach(item => item.texture = null);
        inventoryCanvas.gameObject.SetActive(false);
    }

    public void ShowInventory()
    {
        if (ControllerManager.Instance.SelectedUnit.Inventory.Count > 0)
            ControllerManager.Instance.SelectedUnit.Inventory.ForEach(item =>
                iconsSlots[0].texture = item.Icon.texture);
        inventoryCanvas.gameObject.SetActive(true);
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        turnText.text = newturn == TurnStates.PlayerTurn ? "Player turn" : "Enemy turn";
        HideInventory();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void UpdateInventory()
    {
        HideInventory();
        ShowInventory();
    }
}