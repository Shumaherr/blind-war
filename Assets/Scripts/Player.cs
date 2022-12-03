using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    LocalPlayer,
    OnlinePLayer,
    AI
}
public class Player
{
    public Player(string name, PlayerType type)
    {
        Name = name;
        Type = type;
        Active = true;
        units = new Dictionary<Vector3Int, Unit>();
        //For tests
        var pos = Utils.GetRandomCell(GameManager.Instance.Grid);
        Allies = new List<Player>();
        while (!GameManager.Instance.Grid.HasTile(pos))
        { 
            pos = Utils.GetRandomCell(GameManager.Instance.Grid);
        }
        units.Add(pos, GameManager.Instance.SpawnManager.SpawnUnit(new SpearmanSO(), this, pos).GetComponent<Unit>());
    }
    public List<Player> Allies { get; private set; }
    public string Name { get; private set; }
    public PlayerType Type { get; private set; }
    public bool Active { get; set; }

    private Dictionary<Vector3Int, Unit> units;
    public Dictionary<Vector3Int, Unit> Units
    {
        get => units;
        private set => units = value;
    }

    private Dictionary<Vector3Int, CityController> cities;
    public Dictionary<Vector3Int, CityController> Cities
    {
        get => cities;
        private set => cities = value;
    }

    public void InitPlayer()
    {
        
    }

    public Unit GetUnitInCell(Vector3Int cell)
    {
        return Units.ContainsKey(cell) ? Units[cell] : null;
    }
    
    public CityController GetCityInCell(Vector3Int cell)
    {
        return Cities.ContainsKey(cell) ? Cities[cell] : null;
    }
    
}