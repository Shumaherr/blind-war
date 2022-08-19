using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelector : MonoBehaviour, IPointerClickHandler
{
    public delegate void OnItemSelectedDelegate(int itemNum);

    [SerializeField] private int itemNum;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemSelected?.Invoke(itemNum);
    }

    public event OnItemSelectedDelegate OnItemSelected;
}