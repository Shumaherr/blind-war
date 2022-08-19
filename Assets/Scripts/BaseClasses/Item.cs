using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string name;
    [SerializeField] private bool usable;

    public Sprite Icon => icon;

    protected abstract void PassiveProps();
    protected abstract void Use();

    public void TryToUse()
    {
        if (usable)
        {
            Use();
            RemoveItem();
        }
    }

    private void RemoveItem()
    {
        ControllerManager.Instance.SelectedUnit.Inventory.Remove(this);
        GameManager.Instance.UpdateInventoryUI();
    }
}