using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int itemNum;
    public delegate void OnItemSelectedDelegate(int itemNum);
    public event OnItemSelectedDelegate OnItemSelected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemSelected?.Invoke(itemNum);
    }
}
