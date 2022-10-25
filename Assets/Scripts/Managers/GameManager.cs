using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum GameState
{
    GameInit,
    GameStart,
    TurnChanged,
    DoTurn
}



public class GameManager : Singleton<GameManager>
{
    public delegate void OnGameStateChangedDelegate(GameState newState);

    [SerializeField] private Tilemap grid;
    [SerializeField] private Camera mainCamera;
    private List<CityController> _AICities;
    private ControllerManager _controller;
    private List<EnemyUnitBase> _enemyUnits;

    private List<EnemyUnitBase> _enemyUnitsToDelete;
    private GameState _gameState;
    private GridInteractor _gridInteractor;

    private Pathfinding _pathfinding;

    private List<CityController> _playerCities;
    private SoundManager _soundManager;
    private TurnManager _turnManager;

    public TurnManager TurnManager => _turnManager;
    private UIManager _uiManager;
    public List<Player> Players => _players;
    private List<Player> _players;

    public Camera MainCamera => mainCamera;
    public Tilemap Grid => grid;

    private Dictionary<Vector3Int, EnemyUnitBase> EnemyUnitsPos { get; set; }

    public List<Vector3Int> TakenCells { get; set; }

    public Dictionary<Vector3Int, Unit> PlayerUnits { get; private set; }

    public Dictionary<Vector3Int, CityController> AllCities { get; private set; }

    public SpawnManager SpawnManager { get; set; }

    protected GameState GameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(_gameState);
        }
    }

    private void Awake()
    {
        GameState = GameState.GameInit;
        _soundManager = new SoundManager();
        _pathfinding = new Pathfinding();
        _turnManager = new TurnManager();
        
        _uiManager = GetComponent<UIManager>();
        SpawnManager = GetComponent<SpawnManager>();
        
        PlayerUnits = new Dictionary<Vector3Int, Unit>();
        EnemyUnitsPos = new Dictionary<Vector3Int, EnemyUnitBase>();
        _enemyUnits = new List<EnemyUnitBase>();
        _enemyUnitsToDelete = new List<EnemyUnitBase>();
        TakenCells = new List<Vector3Int>();
        AllCities = new Dictionary<Vector3Int, CityController>();
        _gridInteractor = grid.GetComponent<GridInteractor>();
        foreach (var o in GameObject.FindGameObjectsWithTag("PlayerUnit"))
        {
            PlayerUnits.Add(grid.WorldToCell(o.transform.position), o.GetComponent<Unit>());
            TakenCells.Add(grid.WorldToCell(o.transform.position));
        }

        foreach (var o in GameObject.FindGameObjectsWithTag("AIUnit"))
        {
            _enemyUnits.Add(o.GetComponent<EnemyUnitBase>());
            EnemyUnitsPos.Add(grid.WorldToCell(o.transform.position), o.GetComponent<EnemyUnitBase>());
        }
        
        foreach (var unit in PlayerUnits) unit.Value.OnUnitDie += OnUnitDie;
        foreach (var unit in EnemyUnitsPos) unit.Value.OnUnitDie += OnUnitDie;

        foreach (var city in GameObject.FindGameObjectsWithTag("PlayerCity"))
        {
            var position = city.gameObject.transform.position;
            AllCities.Add(grid.WorldToCell(position), city.GetComponent<CityController>());
            TakenCells.Add(grid.WorldToCell(position));
        }

        foreach (var city in GameObject.FindGameObjectsWithTag("AICity"))
        {
            var position = city.gameObject.transform.position;
            AllCities.Add(grid.WorldToCell(position), city.GetComponent<CityController>());
        }
        SetPlayers();
        GameState = GameState.GameStart;
    }

    private void Update()
    {
        switch (GameState)
        {
            case GameState.GameInit:
                break;
            case GameState.GameStart:
                break;
            case GameState.TurnChanged:
                break;
            case GameState.DoTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

    private void SetPlayers()//TODO This method have to be called from Menu during game init
    {
        _players = new List<Player>
        {
            new("Player1", PlayerType.LocalPlayer),
            new("Bad boys", PlayerType.AI)
        };
    }

    public event OnGameStateChangedDelegate OnGameStateChanged;

    private void OnTurnChanged(Dictionary<string, object> dictionary)
    {
        var newturn = ((Player) dictionary["whoseTurn"]).Type;
        if (newturn == PlayerType.AI)
        {
            foreach (var unit in _enemyUnitsToDelete) _enemyUnits.Remove(unit);

            _enemyUnitsToDelete = new List<EnemyUnitBase>();
            foreach (var unit in _enemyUnits) unit.DoTurn();
        }
    }

    private void OnUnitDie(Unit unit)
    {
        KillUnit(unit);
    }

    private void UnitOnOnUnitSelected(Unit unit)
    {
        if (unit.Inventory.Count > 0)
            _uiManager.ShowInventory();
    }

    public EnemyUnitBase GetEnemyUnitInCell(Vector3Int cell)
    {
        return EnemyUnitsPos[cell];
    }

    public bool HasEnemyUnit(Vector3Int cell)
    {
        return EnemyUnitsPos.ContainsKey(cell);
    }

    public bool HasPlayerUnit(Vector3Int cell)
    {
        return PlayerUnits.ContainsKey(cell);
    }

    /*public bool HasEnemyCity(Vector3Int cell)
    {
        return AllCities.ContainsKey(cell) && AllCities[cell].Owner == CityOwner.AI;
    }

    public bool HasPlayerCity(Vector3Int cell)
    {
        return AllCities.ContainsKey(cell) && AllCities[cell].Owner == CityOwner.Player;
    }*/

    public CityController GetCityInCell(Vector3Int cell)
    {
        return AllCities[cell];
    }

    public void KillUnit(Unit unitToKill)
    {
        if (unitToKill.gameObject.CompareTag("AIUnit"))
        {
            EnemyUnitsPos.Remove(unitToKill.GetUnitCell());
            _enemyUnitsToDelete.Add((EnemyUnitBase)unitToKill);
        }
        else
        {
            ControllerManager.Instance.SelectedUnit = null;
            _gridInteractor.UnhighlightCells();
            PlayerUnits.Remove(unitToKill.GetUnitCell());
        }

        TakenCells.Remove(unitToKill.GetUnitCell());
        Destroy(unitToKill.gameObject);
    }

    public void AddCityToList(Vector3 pos)
    {
        TakenCells.Add(grid.WorldToCell(pos));
    }

    public void RemoveCityToList(Vector3 pos)
    {
        TakenCells.Remove(grid.WorldToCell(pos));
    }

    public bool HasUnit(Vector3Int cell)
    {
        return HasEnemyUnit(cell) || HasPlayerUnit(cell);
    }

    public bool IsCellTaken(Vector3Int cell)
    {
        return TakenCells.Contains(cell);
    }

    public bool HasCity(Vector3Int cell)
    {
        return AllCities.ContainsKey(cell);
    }

    public List<Vector3Int> GetPath(Vector3 start, Vector3Int finish)
    {
        return _pathfinding.FindPath(grid, grid.WorldToCell(start), finish);
    }

    public void ChangeTakenCell(Vector3Int startCell, Vector3Int finishCell)
    {
        if (!TakenCells.Contains(startCell))
            return;
        TakenCells.Remove(startCell);
        TakenCells.Add(finishCell);
    }

    public void ChangeEnemyCell(Vector3Int startCell, Vector3Int finishCell)
    {
        if (!EnemyUnitsPos.ContainsKey(startCell))
            return;
        var unit = EnemyUnitsPos[startCell];
        EnemyUnitsPos.Remove(startCell);
        EnemyUnitsPos.Add(finishCell, unit);
    }

    public void CheckPlayerLose(Player player)
    {
        if (PlayerUnits.Count(u => u.Value.Owner == player) <= 0 &&
            AllCities.Count(pair => pair.Value.Owner == player) <= 0)
            //TODO change to event
            _players.Find(p => p == player).Active = false;
    }

    public HashSet<UnitType> GetNeighbourUnitTypes()
    {
        var neighbourUnits = new HashSet<UnitType>();
        foreach (var neighborCell in Utils.Neighbors(_gridInteractor.UnitCell(ControllerManager.Instance.SelectedUnit)))
            if (EnemyUnitsPos.ContainsKey(neighborCell))
                neighbourUnits.Add(EnemyUnitsPos[neighborCell].BaseUnit.UnitType);

        return neighbourUnits.Count == 0 ? null : neighbourUnits;
    }

    public void UpdateInventoryUI()
    {
        _uiManager.UpdateInventory();
    }

    public void UnselectUnit()
    {
        _gridInteractor.UnhighlightCells();
        _uiManager.HideInventory();
    }

    public Vector3Int GetFreeNeighbourCell(Vector3Int cell)
    {
        var neighbours = Utils.Neighbors(cell);
        foreach (var neighbour in neighbours)
            if (!IsCellTaken(neighbour))
                return neighbour;
        return Vector3Int.zero;
    }

    public Vector3Int GetFreeRandomNeighbourCell(Vector3Int cell)
    {
        var neighbours = Utils.Neighbors(cell);
        var vector3Ints = neighbours.ToList();
        vector3Ints.Shuffle();
        foreach (var neighbour in vector3Ints)
            if (!IsCellTaken(neighbour))
                return neighbour;
        return Vector3Int.zero; //TODO change Vector3Int.zero to error
    }

    public void AddUnitToList(Transform unit) //With Unit it doesnt work because of event
    {
        if (unit.gameObject.CompareTag("AIUnit"))
        {
            EnemyUnitsPos.Add(unit.GetComponent<EnemyUnitBase>().GetUnitCell(), unit.GetComponent<EnemyUnitBase>());
        }
        else
        {
            PlayerUnits.Add(unit.GetComponent<UnitInteractable>().GetUnitCell(), unit.GetComponent<UnitInteractable>());
            TakenCells.Add(unit.GetComponent<UnitInteractable>().GetUnitCell());
            unit.GetComponent<UnitInteractable>().OnUnitSelected += UnitOnOnUnitSelected;
        }
    }

    public void NextTurn()
    {
        TurnManager.ChangeTurn();
    }
}