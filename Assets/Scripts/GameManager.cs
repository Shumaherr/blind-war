using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera mainCamera;

    public Camera MainCamera => mainCamera;

    [SerializeField] private Tilemap grid;
    private GridInteractor _gridInteractor;
    private List<UnitInteractable> _playerUnits;

    private Dictionary<Vector3Int, EnemyUnit> _enemyUnits;

    private List<Vector3Int> _takenCells;

    private List<CityController> _playerCities;
    private List<CityController> _AICities;
    private Dictionary<Vector3Int, CityController> _allCities;
    public List<Vector3Int> TakenCells
    {
        get => _takenCells;
        set => _takenCells = value;
    }

    private TurnManager _turnManager;
    private ControllerManager _controller;

    void Start()
    {
        _playerUnits = new List<UnitInteractable>();
        _enemyUnits = new Dictionary<Vector3Int, EnemyUnit>();
        _takenCells = new List<Vector3Int>();
        _allCities = new Dictionary<Vector3Int, CityController>();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("PlayerUnit"))
        {
            _playerUnits.Add(o.GetComponent<UnitInteractable>());
            _takenCells.Add(grid.LocalToCell(o.transform.position));
        }
        
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("AIUnit"))
        {
            _enemyUnits.Add(grid.LocalToCell(o.transform.position), o.GetComponent<EnemyUnit>());
        }
        _gridInteractor = grid.GetComponent<GridInteractor>();
        foreach (var unit in _playerUnits)
        {
            unit.OnUnitSelected += UnitOnOnUnitSelected;
        }

        foreach (GameObject city in GameObject.FindGameObjectsWithTag("PlayerCity"))
        {
            _allCities.Add(grid.WorldToCell(city.gameObject.transform.position), city.GetComponent<CityController>());
        }
        foreach (GameObject city in GameObject.FindGameObjectsWithTag("AICity"))
        {
            _allCities.Add(grid.WorldToCell(city.gameObject.transform.position), city.GetComponent<CityController>());
        }
    }

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

    public bool HasEnemyCity(Vector3Int cell)
    {
        return _allCities.ContainsKey(cell) && _allCities[cell].Owner == CityOwner.AI;
    }

    public CityController GetCityInCell(Vector3Int cell)
    {
        return _allCities[cell];
    }

    public void KillUnit(Unit unitToKill)
    {
        if(unitToKill.gameObject.CompareTag("AIUnit"))
        {
            _enemyUnits.Remove(_enemyUnits.First((kvp => kvp.Value == unitToKill)).Key);
        }
        else
        {
            _playerUnits.Remove(unitToKill as UnitInteractable);
        }

        _takenCells.Remove(grid.WorldToCell(unitToKill.transform.position));
        
        Destroy(unitToKill.gameObject);
    }
}