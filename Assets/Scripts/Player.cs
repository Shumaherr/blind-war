using System.Collections.Generic;
using System.Linq;
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
        //For tests
        var pos = Utils.GetRandomCell(GameManager.Instance.Grid);
        Allies = new List<Player>();
        while (!GameManager.Instance.Grid.HasTile(pos))
        { 
            pos = Utils.GetRandomCell(GameManager.Instance.Grid);
        }
    }
    public List<Player> Allies { get; private set; }
    public string Name { get; private set; }
    public PlayerType Type { get; private set; }
    public bool Active { get; set; }

    public void InitPlayer()
    {
        
    }

    public Unit GetUnitInCell(Vector3Int cell)
    {
        return ControllerManager.Instance.AllUnits.ContainsKey(cell) ? ControllerManager.Instance.AllUnits[cell] : null;
    }
    
    public CityController GetCityInCell(Vector3Int cell)
    {
        return ControllerManager.Instance.AllCitites.ContainsKey(cell) ? ControllerManager.Instance.AllCitites[cell] : null;
    }

    public bool CheckUnits()
    {
        if (ControllerManager.Instance.AllUnits.ToList().Count((unit => unit.Value.Owner == this)) == 0)
        {
            Active = false;
        }

        return Active;
    }
    
}