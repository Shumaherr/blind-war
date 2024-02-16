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
        
        _enemyUnits = new List<EnemyUnitBase>();
        _enemyUnitsToDelete = new List<EnemyUnitBase>();
        _gridInteractor = grid.GetComponent<GridInteractor>();

        SetPlayers();
        Players.ForEach(player => player.InitPlayer());
        ControllerManager.Instance.AllUnits.ToList().ForEach(unit => unit.Value.OnUnitDie += OnUnitDie);
        _turnManager.ChangeTurn();
        Debug.Log("Turn:" + _turnManager.Turn.Name);
        GameState = GameState.GameStart;
        var unitPos = ControllerManager.Instance.AllUnits.Values.First(u => u.Owner == _turnManager.Turn).gameObject
            .transform.position;
        SetCamera(unitPos);
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
            //new("Player2", PlayerType.LocalPlayer),
            new("Bad boys", PlayerType.AI)
        };
    }

    public event OnGameStateChangedDelegate OnGameStateChanged;

    private void OnTurnChanged(Dictionary<string, object> dictionary)
    {
        var newTurn = (Player)dictionary["whoseTurn"];
        var units = ControllerManager.Instance.AllUnits.Values.Where(u => u.Owner == newTurn).ToList();
        foreach (var unit in units)
        {
            unit.InitMoves();
        }

        if (newTurn.Type == PlayerType.AI)
        {
            foreach (var unit in _enemyUnitsToDelete) _enemyUnits.Remove(unit);

            _enemyUnitsToDelete = new List<EnemyUnitBase>();
            foreach (var unit in units)
            {
                unit.Controller.DoTurn();
            }
            return;
        }

        var unitPos = ControllerManager.Instance.AllUnits.Values.First(u => u.Owner == newTurn).gameObject
            .transform.position;
        SetCamera(unitPos);
    }

    private void SetCamera(Vector3 unitPos)
    {
        var cameraPosition = mainCamera.transform.position;
        cameraPosition.x = unitPos.x;
        cameraPosition.y = unitPos.y;
        mainCamera.transform.position = cameraPosition;
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
    

    public void KillUnit(Unit unitToKill)
    {
        Destroy(unitToKill.gameObject);
    }

    public bool HasUnit(Vector3Int cell)
    {
        return Players.Any(player => player.GetUnitInCell(cell));
    }

    public bool IsCellTaken(Vector3Int cell)
    {
        return HasCity(cell) || HasUnit(cell);
    }

    public bool HasCity(Vector3Int cell)
    {
        return Players.Any(player => player.GetCityInCell(cell));
    }

    public List<Vector3Int> GetPath(Vector3 start, Vector3Int finish)
    {
        return _pathfinding.FindPath(grid, grid.WorldToCell(start), finish);
    }



    public void UpdateInventoryUI()
    {
        _uiManager.UpdateInventory();
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
    
    public void NextTurn()
    {
        TurnManager.ChangeTurn();
    }
    
    public List<Player> GetActivePlayers()
    {
        return Players.Where(player => player.Active).ToList();
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Scene_Defeat");
    }
}