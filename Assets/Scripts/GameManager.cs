using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera mainCamera;

    public Camera MainCamera => mainCamera;

    [SerializeField] private Tilemap grid;
    public Tilemap Grid => grid;
    private GridInteractor _gridInteractor;
    private Dictionary<Vector3Int, UnitInteractable> _playerUnits;

    private Dictionary<Vector3Int, EnemyUnit> _enemyUnitsPos;
    private List<EnemyUnit> _enemyUnits;
    private List<Vector3Int> _enemyUnitsToDelete;

    private Dictionary<Vector3Int, EnemyUnit> EnemyUnitsPos
    {
        get => _enemyUnitsPos;
        set =>_enemyUnitsPos = value;
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
        _enemyUnitsPos = new Dictionary<Vector3Int, EnemyUnit>();
        _enemyUnits = new List<EnemyUnit>();
        _enemyUnitsToDelete = new List<Vector3Int>();
        _takenCells = new List<Vector3Int>();
        _allCities = new Dictionary<Vector3Int, CityController>();
        _pathfinding = new Pathfinding();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("PlayerUnit"))
        {
            _playerUnits.Add(grid.WorldToCell(o.transform.position), o.GetComponent<UnitInteractable>());
            _takenCells.Add(grid.WorldToCell(o.transform.position));
        }
        
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("AIUnit"))
        {
            _enemyUnits.Add(o.GetComponent<EnemyUnit>());
            _enemyUnitsPos.Add(grid.WorldToCell(o.transform.position), o.GetComponent<EnemyUnit>());
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
        OnTurnChanged(TurnStates.PlayerTurn);
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        if (newturn == TurnStates.AITurn)
        {
            foreach (var unit in _enemyUnitsToDelete)
            {
                _enemyUnits.Remove(_enemyUnitsPos[unit]);
                _enemyUnitsPos.Remove(unit);
            }
            _enemyUnitsToDelete = new List<Vector3Int>();
            foreach (var unit in _enemyUnits)
            {
                unit.DoTurn();
                
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
        _gridInteractor.HighlightNeighbourCells(grid.LocalToCell(unit.gameObject.transform.position));
        unit.UsePerk();
    }

    public EnemyUnit GetEnemyUnitInCell(Vector3Int cell)
    {
        return _enemyUnitsPos[cell];
    }

    public bool HasEnemyUnit(Vector3Int cell)
    {
        return _enemyUnitsPos.ContainsKey(cell);
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
            //EnemyUnits.Remove(unitToKill.GetUnitCell());
            _enemyUnitsToDelete.Add(unitToKill.GetUnitCell());
            CheckPlayerWin();
        }
        else
        {
            _playerUnits.Remove(unitToKill.GetUnitCell());
            CheckPlayerLoose();
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
        if (!_enemyUnitsPos.ContainsKey(startCell)) 
            return;
        EnemyUnit unit = _enemyUnitsPos[startCell];
        _enemyUnitsPos.Remove(startCell);
        _enemyUnitsPos.Add(finishCell, unit);
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
    
    public void CheckPlayerLoose()
    {
        if (_playerUnits.Count <= 0 && _allCities.Count(pair => pair.Value.Owner == CityOwner.Player) <= 0)
        {
            //TODO change to event
            PlayerLoose();
        }
    }

    private void PlayerLoose()
    {
        SceneManager.LoadScene("Scene_Defeat");
    }

    public bool HighlightCellWithoutEnemy()
    {
        List<Vector3Int> tempList = Utils.Neighbors(ControllerManager.Instance.SelectedUnitCell()).
            Where(cell => _enemyUnitsPos.ContainsKey(cell)).ToList();
        if (tempList.Count == 0)
            return false;
        _gridInteractor.HighlightCell(tempList[Random.Range(0,tempList.Count)], Color.red);
        return true;
    }

    public HashSet<UnitType> GetNeighbourUnitTypes()
    {
        HashSet<UnitType> neighbourUnits = new HashSet<UnitType>();
        foreach (var neighborCell in  Utils.Neighbors(ControllerManager.Instance.SelectedUnitCell()))
        {
            if (_enemyUnitsPos.ContainsKey(neighborCell))
            {
                neighbourUnits.Add(_enemyUnitsPos[neighborCell].BaseUnit.UnitType);
            }
        }

        if (neighbourUnits.Count == 0)
            return null;
        return neighbourUnits;
    }
}