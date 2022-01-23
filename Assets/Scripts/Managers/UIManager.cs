using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;

    // Start is called before the first frame update
    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        turnText.text = newturn == TurnStates.PlayerTurn ? "Player turn" : "Enemy turn";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}