using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;


public class CityController : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int _turnsToProduce = 3;
    [SerializeField] private List<BaseUnit> _unitsToProduce;
    
    public Player Owner { get; set; }
    private int _health;
    private Healthbar _healthbar;
    private BaseUnit _producingUnit;

    private Sprite _sprite;
    private SpriteRenderer _spriteRenderer;
    private int _turnsToProduceLeft;

    private int Health
    {
        get => _health;
        set
        {
            _health = value > 0 ? value : 0;
            _healthbar.SetHealthLevel((float) _health / maxHealth);
        }
    }

    private int TurnsToProduceLeft
    {
        get => _turnsToProduceLeft;
        set
        {
            if (value <= 0)
                _turnsToProduceLeft = 0;
            else
                _turnsToProduceLeft = value;
        }
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("turnChanged", OnTurnChanged);
            
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("turnChanged", OnTurnChanged);
            
    }

    // Start is called before the first frame update
    private void Start()
    {
        _sprite =  sprites[0];
        _healthbar = GetComponentInChildren<Healthbar>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = _sprite;
        Health = maxHealth;
    }

    private void ChangeSprite()
    {
        //_sprite = owner == CityOwner.Player ? sprites[0] : sprites[1];
        //_spriteRenderer.sprite = _sprite;
    }

    private void ChangeOwner(Player newOwner)
    {
        Owner = newOwner;
        ChangeSprite();
        Health = maxHealth;
        _producingUnit = null;
        ChooseRandomUnitToProduce();
        /*if (Owner == CityOwner.Player)
        {
            GameManager.Instance.AddCityToList(transform.position);
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_capture");
        }
        else
        {
            GameManager.Instance.RemoveCityToList(transform.position);
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_lose");
        }*/
    }

    private void OnTurnChanged(Dictionary<string, object> dictionary)
    {
        var newturn = (Player) dictionary["newTurn"];
        if (_producingUnit == null)
            ChooseRandomUnitToProduce();
        if (newturn == Owner)
        {
            TurnsToProduceLeft--;
            CheckProductionIsReady();
        }
    }

    private void CheckProductionIsReady()
    {
        if (TurnsToProduceLeft == 0) ProduceUnit();
    }

    private void ProduceUnit()
    {
        var ciyPos = GameManager.Instance.AllCities.First(c => c.Value == this).Key;
        var randPos = GameManager.Instance.GetFreeRandomNeighbourCell(ciyPos);
        if (randPos != Vector3Int.zero)
        {
            GameManager.Instance.SpawnManager.SpawnUnit(_producingUnit, Owner, randPos);
            ChooseRandomUnitToProduce();
        }
    }

    private void ChooseRandomUnitToProduce()
    {
        _producingUnit = _unitsToProduce[Random.Range(0, _unitsToProduce.Count)];
        _turnsToProduceLeft = _turnsToProduce;
    }

    public void TakeDamage(int damage)
    {
        if (_health - damage > 0) //for avoid castle_siege & castle_capture sounds at one time
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_siege");
        Health -= damage;
    }

    public void TakeDamage(Unit damageDealer)
    {
        if (_health - damageDealer.Damage > 0) //for avoid castle_siege & castle_capture sounds at one time
            RuntimeManager.PlayOneShot("event:/SFX/environment/castle_siege");
        Health -= damageDealer.Damage;
    }
}