using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnBar : MonoBehaviour
{
    private TextMeshPro _turnText;
    // Start is called before the first frame update
    void OnEnable()
    {
        _turnText = GetComponent<TextMeshPro>();
    }

    public void SetTurnText(int currentPoints, int maxPoints)
    {
        _turnText.text = $"{currentPoints}/{maxPoints}";
    }   
}
