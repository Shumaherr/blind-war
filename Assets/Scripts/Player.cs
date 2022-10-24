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
        _startUnits = new Dictionary<Vector2Int, BaseUnit>();
        //For tests
        _startUnits.Add(new Vector2Int(0,0), new SpearmanSO());
    }

    public string Name { get; private set; }
    public PlayerType Type { get; private set; }
    public bool Active { get; set; }

    private Dictionary<Vector2Int, BaseUnit> _startUnits;

    public Dictionary<Vector2Int, BaseUnit> StartUnits
    {
        get => _startUnits;
        private set => _startUnits = value;
    }
}