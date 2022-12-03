﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Tilemaps;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class ControllerManager : Singleton<ControllerManager>
{
    [SerializeField] private Tilemap walkableTilemap;
    private GridInteractor _gridInteractor;
    private Unit _selectedUnit;

    private EventInstance instance;
    public Dictionary<Vector3Int, Unit> AllUnits { get; set; }
    public Dictionary<Vector3Int, CityController> AllCitites { get; set; }

    public Unit SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            _selectedUnit = value;
            if (_selectedUnit != null)
                _gridInteractor.HighlightNeighbourCells(_selectedUnit);
        }
    }

    public void Awake()
    {
        _gridInteractor = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridInteractor>();
        instance = new EventInstance();
        _gridInteractor.OnTileSelected += TileSelected;
        AllUnits = new Dictionary<Vector3Int, Unit>();
        AllCitites = new Dictionary<Vector3Int, CityController>();

        //Subscribe to event click on tile
    }

    private void OnEnable()
    {
        EventManager.StartListening("unitSelected", OnUnitSelected);
    }

    private void OnDisable()
    {
        EventManager.StopListening("unitSelected", OnUnitSelected);
    }

    private void OnUnitSelected(Dictionary<string, object> obj)
    {
        SelectedUnit = (Unit)obj["unit"];
        Debug.Log("Selected unit: " + SelectedUnit.name);
    }

    private void TileSelected(Vector3Int tilePos)
    {
        if (_selectedUnit)
        {
            //For tests
            /*Pathfinding pathfinding = new Pathfinding();
            foreach (var vector3Int in pathfinding.FindPath(walkableTilemap, SelectedUnitCell(), tilePos))
            {
                walkableTilemap.SetTileFlags(vector3Int, TileFlags.None);
                walkableTilemap.SetColor(vector3Int, Color.magenta);
            }*/
            //_selectedUnit.DeactivateDialog();
            if (!GameManager.Instance.TurnManager.IsLocalPlayerTurn() || !_selectedUnit.CanMove())
                return;
            if (MapManager.Instance.GetMoveCosts(_selectedUnit.BaseUnit, walkableTilemap.GetTile(tilePos)) >
                _selectedUnit.Moves)
                return;
            Unit unitInCell = AllUnits.ContainsKey(tilePos) && AllUnits[tilePos].Owner != _selectedUnit.Owner?AllUnits[tilePos]:null;
            _selectedUnit.ChangeMoves(MapManager.Instance.GetMoveCosts(_selectedUnit.BaseUnit,
                walkableTilemap.GetTile(tilePos)));
            if (unitInCell != null)
                if (!StartBattle(_selectedUnit, unitInCell))
                    return;

            MoveUnitToTile(_selectedUnit.transform, tilePos);
            var tempUnit = _gridInteractor.UnitCell(_selectedUnit);
            GameManager.Instance.TurnManager.Turn.Units.Remove(tempUnit);
            GameManager.Instance.TurnManager.Turn.Units.Add(tilePos, _selectedUnit);
            ClearSelected();
        }
    }

    public void MoveUnitToTile(Transform unitToMove, Vector3Int tilePos)
    {
        var position = unitToMove.position;
        var oldCellPos = walkableTilemap.WorldToCell(position);
        var countCells = (int)Mathf.Round(Vector3.Distance(tilePos, oldCellPos));
        if (countCells == 0)
            return;
        var cellCenterWorld = walkableTilemap.GetCellCenterWorld(tilePos);
        var oldNeighbors = Utils.Neighbors(oldCellPos).Where((i => AllUnits.ContainsKey(i)));
        AllUnits.Where((pair => oldNeighbors.Contains(pair.Key) )).ToList().ForEach((pair => pair.Value.HideUnit()));
        Debug.Log("Move for " + countCells + " cells");
        StartCoroutine(MoveFromTo(unitToMove, position, cellCenterWorld, 3));
        //GameManager.Instance.ChangeTakenCell(unitCell, tilePos);
        EventManager.TriggerEvent("unitMoved", null);
        var tempUnit = AllUnits[oldCellPos];
        AllUnits.Remove(oldCellPos);
        AllUnits.Add(tilePos, tempUnit);
        AllUnits.Where((pair => Utils.Neighbors(tilePos).Contains(pair.Key) && pair.Value.Owner != GameManager.Instance.TurnManager.Turn)).
            ToList().ForEach(pair => pair.Value.ShowGeneralizedSprite());
        Debug.Log("End");
    }

    public bool StartBattle(Unit enemyUnit, Unit playerUnit)
    {
        if (!enemyUnit.enabled || !playerUnit.enabled)
            return false;
        var winner = BattleSystem.Fight(playerUnit, enemyUnit);
        var loser = winner == playerUnit ? enemyUnit : playerUnit;
        if (winner == null)
            return false;
        loser.Perks.OfType<Fortificate>().First(p => p.IsActive()).DecreaseAttacksAmount();

        var winnerType = winner != null ? winner.BaseUnit.UnitType : playerUnit.BaseUnit.UnitType;
        RuntimeManager.PlayOneShot(SoundManager.GetWinnerSfxEventToPlay(winnerType), transform.position);

        if (playerUnit == winner)
        {
            GameManager.Instance.KillUnit(enemyUnit);
            return true;
        }

        GameManager.Instance.KillUnit(playerUnit);
        return false;
    }


    public void ClearSelected()
    {
        SelectedUnit = null;
    }

    private IEnumerator MoveFromTo(Transform objectToMove, Vector3 pos1, Vector3 pos2, float speed)
    {
        if (GameManager.Instance.TurnManager.IsLocalPlayerTurn())
        {
            instance = RuntimeManager.CreateInstance(
                SoundManager.GetMovementSfxEventToPlay(_selectedUnit.BaseUnit.UnitType));
            RuntimeManager.AttachInstanceToGameObject(instance, _selectedUnit.transform);
        }

        instance.start();
        var step = speed / (pos1 - pos2).magnitude * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(pos1, pos2, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }

        instance.stop(STOP_MODE.ALLOWFADEOUT);
        instance.release();
        objectToMove.position = pos2;
        var worldToCell = walkableTilemap.WorldToCell(pos2);
        var neighbors = Utils.Neighbors(worldToCell);
        
    }
    
    public bool IsNearAlienUnit(Vector3 pos)
    {
        return Utils.Neighbors(walkableTilemap.WorldToCell(pos)).
            Any(i => (AllUnits.ContainsKey(i) || AllCitites.ContainsKey(i)) && AllUnits[i].Owner == GameManager.Instance.TurnManager.Turn);
    }
}