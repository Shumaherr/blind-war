using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera mainCamera;

    public Camera MainCamera => mainCamera;

    [SerializeField] private Tilemap grid;
    public Tilemap Grid => grid;
    private GridInteractor _gridInteractor;
    private Dictionary<Vector3Int, UnitInteractable> _playerUnits;

    private Dictionary<Vector3Int, EnemyUnit> _enemyUnits;

    private Dictionary<Vector3Int, EnemyUnit> EnemyUnits
    {
        get => _enemyUnits;
        set
        {
            _enemyUnits = value;
            if (_enemyUnits.Count == 0)
                PlayerWin();
        }
    }

    private List<Vector3Int> _takenCells;

    private List<CityController> _playerCities;
    private List<CityController> _AICities;
    private Dictionary<Vector3Int, CityController> _allCities;
    
    private Pathfinding _pathfinding;
    
    public List<Vector3Int> TakenCells
    {
        get => _takenCells;
        set => _takenCells = value;
    }

    private TurnManager _turnManager;
    private ControllerManager _controller;

    void Start()
    {
        _playerUnits = new Dictionary<Vector3Int, UnitInteractable>();
        _enemyUnits = new Dictionary<Vector3Int, EnemyUnit>();
        _takenCells = new List<Vector3Int>();
        _allCities = new Dictionary<Vector3Int, CityController>();
        _pathfinding = new Pathfinding();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("PlayerUnit"))
        {
            _playerUnits.Add(grid.LocalToCell(o.transform.position), o.GetComponent<UnitInteractable>());
            _takenCells.Add(grid.WorldToCell(o.transform.position));
        }
        
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("AIUnit"))
        {
            _enemyUnits.Add(grid.WorldToCell(o.transform.position), o.GetComponent<EnemyUnit>());
        }
        _gridInteractor = grid.GetComponent<GridInteractor>();
        foreach (var unit in _playerUnits)
        {
            unit.Value.OnUnitSelected += UnitOnOnUnitSelected;
        }

        foreach (GameObject city in GameObject.FindGameObjectsWithTag("PlayerCity"))
        {
            var position = city.gameObject.transform.position;
            _allCities.Add(grid.WorldToCell(position), city.GetComponent<CityController>());
            _takenCells.Add(grid.WorldToCell(position));
        }
        foreach (GameObject city in GameObject.FindGameObjectsWithTag("AICity"))
        {
            var position = city.gameObject.transform.position;
            _allCities.Add(grid.WorldToCell(position), city.GetComponent<CityController>());
        }
        
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
        
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        if (newturn == TurnStates.AITurn)
        {
            foreach (var unit in _enemyUnits)
            {
                unit.Value.DoTurn();
                
            }
            TurnManager.Instance.ChangeTurn();
        }
    }

    public Dictionary<Vector3Int, UnitInteractable> PlayerUnits => _playerUnits;

    private void UnitOnOnUnitSelected(UnitInteractable unit)
    {
        if (!unit.CanMove())
        {
            _gridInteractor.UnhighlightCells();
            return;
        }
        _gridInteractor.HighLightCells(grid.LocalToCell(unit.gameObject.transform.position));
    }

    public EnemyUnit GetEnemyUnitInCell(Vector3Int cell)
    {
        return _enemyUnits[cell];
    }

    public bool HasEnemyUnit(Vector3Int cell)
    {
        return _enemyUnits.ContainsKey(cell);
    }
    
    public bool HasPlayerUnit(Vector3Int cell)
    {
        return _playerUnits.ContainsKey(cell);
    }

    public bool HasEnemyCity(Vector3Int cell)
    {
        return _allCities.ContainsKey(cell) && _allCities[cell].Owner == CityOwner.AI;
    }
    
    public bool HasPlayerCity(Vector3Int cell)
    {
        return _allCities.ContainsKey(cell) && _allCities[cell].Owner == CityOwner.Player;
    }

    public CityController GetCityInCell(Vector3Int cell)
    {
        return _allCities[cell];
    }

    public void KillUnit(Unit unitToKill)
    {
        if(unitToKill.gameObject.CompareTag("AIUnit"))
        {
            EnemyUnits.Remove(unitToKill.GetUnitCell());
            CheckPlayerWin();
        }
        else
        {
            _playerUnits.Remove(unitToKill.GetUnitCell());
        }

        _takenCells.Remove(unitToKill.GetUnitCell());
        Destroy(unitToKill.gameObject);
    }

    public void AddCityToList(Vector3 pos)
    {
        _takenCells.Add(grid.WorldToCell(pos));
    }

    public void RemoveCityToList(Vector3 pos)
    {
        _takenCells.Remove(grid.WorldToCell(pos));
    }

    public bool HasUnit(Vector3Int cell)
    {
        return HasEnemyUnit(cell) || HasPlayerUnit(cell);
    }

    public bool IsCellTaken(Vector3Int cell)
    {
        return TakenCells.Contains(cell);
    }

    public Dictionary<Vector3Int, CityController> AllCities => _allCities;

    public bool HasCity(Vector3Int cell)
    {
        return _allCities.ContainsKey(cell);
    }

    public List<Vector3Int> GetPath(Vector3 start, Vector3Int finish)
    {
        return _pathfinding.FindPath(grid, grid.WorldToCell(start), finish);
    }

    public void ChangeTakenCell(Vector3Int startCell, Vector3Int finishCell)
    {
        if (!_takenCells.Contains(startCell)) 
            return;
        _takenCells.Remove(startCell);
        _takenCells.Add(finishCell);
    }
    
    public void ChangeEnemyCell(Vector3Int startCell, Vector3Int finishCell)
    {
        if (!_enemyUnits.ContainsKey(startCell)) 
            return;
        EnemyUnit unit = _enemyUnits[startCell];
        _enemyUnits.Remove(startCell);
        _enemyUnits.Add(finishCell, unit);
    }

    private void PlayerWin()
    {
        SceneManager.LoadScene("Scene_Win");
    }

    public void CheckPlayerWin()
    {
        if (_enemyUnits.Count <= 0 && _allCities.Count(pair => pair.Value.Owner == CityOwner.AI) <= 0)
        {
            //TODO change to event
            PlayerWin();
        }
    }
    
}