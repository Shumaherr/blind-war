using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum GameState
{
    GameInit,
    GameStart,
    PlayerWin,
    PlayerLose
}
public class GameManager : Singleton<GameManager>
{
    private GameState _gameState;
    private SoundManager _soundManager;
    private List<CityController> _AICities;
    private ControllerManager _controller;
    private List<EnemyUnitBase> _enemyUnits;

    private List<EnemyUnitBase> _enemyUnitsToDelete;
    private GridInteractor _gridInteractor;

    private Pathfinding _pathfinding;

    private List<CityController> _playerCities;

    private TurnManager _turnManager;
    private UIManager _uiManager;

    [SerializeField] private Tilemap grid;
    [SerializeField] private Camera mainCamera;
    
    public Camera MainCamera => mainCamera;
    public Tilemap Grid => grid;

    private Dictionary<Vector3Int, EnemyUnitBase> EnemyUnitsPos { get; set; }

    public List<Vector3Int> TakenCells { get; set; }

    public Dictionary<Vector3Int, Unit> PlayerUnits { get; private set; }

    public Dictionary<Vector3Int, CityController> AllCities { get; private set; }
    
    public delegate void OnGameStateChangedDelegate(GameState newState);
    public event OnGameStateChangedDelegate OnGameStateChanged;
    private SpawnManager _spawnManager;

    public SpawnManager SpawnManager
    {
        get => _spawnManager;
        set => _spawnManager = value;
    }

    private void Awake()
    {
        
        _soundManager = new SoundManager();
        _uiManager = GetComponent<UIManager>();
        _spawnManager = GetComponent<SpawnManager>();
        GameState = GameState.GameInit;
        PlayerUnits = new Dictionary<Vector3Int, Unit>();
        EnemyUnitsPos = new Dictionary<Vector3Int, EnemyUnitBase>();
        _enemyUnits = new List<EnemyUnitBase>();
        _enemyUnitsToDelete = new List<EnemyUnitBase>();
        TakenCells = new List<Vector3Int>();
        AllCities = new Dictionary<Vector3Int, CityController>();
        _pathfinding = new Pathfinding();
        foreach (var o in GameObject.FindGameObjectsWithTag("PlayerUnit"))
        {
            PlayerUnits.Add(grid.WorldToCell(o.transform.position), o.GetComponent<UnitInteractable>());
            TakenCells.Add(grid.WorldToCell(o.transform.position));
        }

        foreach (var o in GameObject.FindGameObjectsWithTag("AIUnit"))
        {
            _enemyUnits.Add(o.GetComponent<EnemyUnitBase>());
            EnemyUnitsPos.Add(grid.WorldToCell(o.transform.position), o.GetComponent<EnemyUnitBase>());
        }

        _gridInteractor = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridInteractor>();
        foreach (var unit in PlayerUnits) unit.Value.OnUnitSelected += UnitOnOnUnitSelected;
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

        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
        OnTurnChanged(TurnStates.PlayerTurn);
        GameState = GameState.GameStart;
    }

    protected GameState GameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(_gameState);
        } 
    }

    private void OnTurnChanged(TurnStates newturn)
    {
        if (newturn == TurnStates.AITurn)
        {
            foreach (var unit in _enemyUnitsToDelete)
            {
                _enemyUnits.Remove(unit);
            }

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

        if(unit.Inventory.Count > 0)
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

    public bool HasEnemyCity(Vector3Int cell)
    {
        return AllCities.ContainsKey(cell) && AllCities[cell].Owner == CityOwner.AI;
    }

    public bool HasPlayerCity(Vector3Int cell)
    {
        return AllCities.ContainsKey(cell) && AllCities[cell].Owner == CityOwner.Player;
    }

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
            CheckPlayerWin();
        }
        else
        {
            ControllerManager.Instance.SelectedUnit = null;
            _gridInteractor.UnhighlightCells();
            PlayerUnits.Remove(unitToKill.GetUnitCell());
            CheckPlayerLose();
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

    private void PlayerWin()
    {
        GameState = GameState.PlayerWin;
        SceneManager.LoadScene("Scene_Win");
    }

    public void CheckPlayerWin()
    {
        if (EnemyUnitsPos.Count <= 0 && AllCities.Count(pair => pair.Value.Owner == CityOwner.AI) <= 0)
            //TODO change to event
            PlayerWin();
    }

    public void CheckPlayerLose()
    {
        if (PlayerUnits.Count <= 0 && AllCities.Count(pair => pair.Value.Owner == CityOwner.Player) <= 0)
            //TODO change to event
            PlayerLose();
    }

    private void PlayerLose()
    {
        GameState = GameState.PlayerLose;
        SceneManager.LoadScene("Scene_Defeat");
    }

    public bool HighlightCellWithoutEnemy()
    {
        var tempList = Utils.Neighbors(ControllerManager.Instance.SelectedUnitCell())
            .Where(cell => EnemyUnitsPos.ContainsKey(cell)).ToList();
        if (tempList.Count == 0)
            return false;
        _gridInteractor.HighlightCell(tempList[Random.Range(0, tempList.Count)], Color.red);
        return true;
    }

    public HashSet<UnitType> GetNeighbourUnitTypes()
    {
        var neighbourUnits = new HashSet<UnitType>();
        foreach (var neighborCell in Utils.Neighbors(ControllerManager.Instance.SelectedUnitCell()))
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
    

}