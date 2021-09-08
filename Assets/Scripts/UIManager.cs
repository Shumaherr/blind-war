using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    // Start is called before the first frame update
    void Start()
    {
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        turnText.text = newturn == TurnStates.PlayerTurn ? "Player turn" : "Enemy turn";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
