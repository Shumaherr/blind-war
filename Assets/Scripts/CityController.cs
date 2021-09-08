using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CityOwner
{
    Player = 0,
    AI = 1
}
public class CityController : MonoBehaviour
{
    [SerializeField] private CityOwner owner;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private List<Sprite> sprites;

    private Sprite _sprite;
    private SpriteRenderer _spriteRenderer;
    public CityOwner Owner
    {
        get => owner;
        set
        {
            owner = value;
        }
    }

    private void ChangeSprite()
    {
        _sprite =  owner == CityOwner.Player ? sprites[0] : sprites[1];
        _spriteRenderer.sprite = _sprite;
    }

    private int _health;

    private int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (value <= 0)
                ChangeOwner();
        }
    }

    private void ChangeOwner()
    {
        Owner = Owner == CityOwner.Player ? CityOwner.AI : CityOwner.Player;
        ChangeSprite();
        _health = maxHealth;
        if(Owner == CityOwner.Player)
            GameManager.Instance.AddCityToList(this.transform.position);
        else
        {
            GameManager.Instance.RemoveCityToList(this.transform.position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _sprite = owner == CityOwner.Player ? sprites[0] : sprites[1];
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = _sprite;
        _health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}
