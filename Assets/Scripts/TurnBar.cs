using TMPro;
using UnityEngine;

public class TurnBar : MonoBehaviour
{
    private TextMeshPro _turnText;

    // Start is called before the first frame update
    private void OnEnable()
    {
        _turnText = GetComponent<TextMeshPro>();
        GetComponentInParent<Unit>().OnMovesChanged += (currentTP, baseTP) => SetTurnText(currentTP, baseTP);
    }

    public void SetTurnText(int currentPoints, int maxPoints)
    {
        _turnText.text = $"{currentPoints}/{maxPoints}";
    }
}