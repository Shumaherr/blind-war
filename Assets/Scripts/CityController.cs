using System.Collections.Generic;
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
        if (Owner == CityOwner.Player)
            GameManager.Instance.AddCityToList(transform.position);
        else
            GameManager.Instance.RemoveCityToList(transform.position);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _sprite = owner == CityOwner.Player ? sprites[0] : sprites[1];
        _healthbar = GetComponentInChildren<Healthbar>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = _sprite;
        Health = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}