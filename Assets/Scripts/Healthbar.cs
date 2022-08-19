using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public void SetHealthLevel(float healthPercent)
    {
        transform.localScale = new Vector3(healthPercent, 1, 1);
    }
}