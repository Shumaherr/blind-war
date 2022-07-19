using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public enum CityOwner
{
    Player = 0,
    AI = 1
}

public class CityController : MonoBehaviour
{
    private int _health;

    private Sprite _sprite;
    private SpriteRenderer _spriteRenderer;
    private Healthbar _healthbar;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private CityOwner owner;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int _turnsToProduce = 3;
    private Unit _producingUnit;
    private int _turnsToProduceLeft = 0;
    [SerializeField] private List<Unit> _unitsToProduce;

    public CityOwner Owner
    {
        get => owner;
        set => owner = value;
    }

    private int Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthbar.SetHealthLevel((float)_health / maxHealth);
            if (value <= 0)
                ChangeOwner();
        }
    }

    private void ChangeSprite()
    {
        _sprite = owner == CityOwner.Player ? sprites[0] : sprites[1];
        _spriteRenderer.sprite = _sprite;
    }

    private void ChangeOwner()
    {
        Owner = Owner == CityOwner.Player ? CityOwner.AI : CityOwner.Player;
        ChangeSprite();
        Health = maxHealth;
        _producingUnit = null;
        if (Owner == CityOwner.Player)
        {
            GameManager.Instance.AddCityToList(transform.position);
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_capture");
        }
        else
        {
            GameManager.Instance.RemoveCityToList(transform.position);
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_lose");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _sprite = owner == CityOwner.Player ? sprites[0] : sprites[1];
        _healthbar = GetComponentInChildren<Healthbar>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = _sprite;
        Health = maxHealth;
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        if(_producingUnit == null)
            ChooseRandomUnitToProduce();
        if (newturn == TurnStates.PlayerTurn && Owner == CityOwner.Player ||
            newturn == TurnStates.AITurn && Owner == CityOwner.AI)
        {
            _turnsToProduceLeft--;
            CheckProductionIsReady();
        }
    }

    private void CheckProductionIsReady()
    {
        if (_turnsToProduceLeft == 0)
        {
            ProduceUnit();
        }
    }

    private void ProduceUnit()
    {
        var ciyPos = GameManager.Instance.AllCities.First(c => c.Value == this).Key;
        var randPos = GameManager.Instance.GetFreeRandomNeighbourCell(ciyPos);
        if (randPos != Vector3Int.zero)
        {
            GameManager.Instance.SpawnManager.SpawnUnit(_producingUnit, randPos);
        }

    }

    private void ChooseRandomUnitToProduce()
    {
        _producingUnit = _unitsToProduce[Random.Range(0, _unitsToProduce.Count)];
        _turnsToProduceLeft = _turnsToProduce;
    }

    public void TakeDamage(int damage)
    {
        if(_health - damage > 0) //for avoid castle_siege & castle_capture sounds at one time
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_siege");
        Health -= damage;
    }
}