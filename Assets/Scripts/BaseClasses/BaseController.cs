using UnityEngine;

public class BaseController: MonoBehaviour
{
    protected Unit Unit { get; set; }

    private void Start()
    {
        Unit = GetComponent<Unit>();
    }
    
}