using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ControllerManager : Singleton<ControllerManager>
{
    [SerializeField] private Tilemap walkableTilemap;
    private UnitInteractable _selectedUnit;
    private GridInteractor _gridInteractor;
    private BattleSystem _battleSystem;
    
    private FMOD.Studio.EventInstance instance;

    public UnitInteractable SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            if (value == null)
                _gridInteractor.UnhighlightCells();
            _selectedUnit = value;
        }
    }

    private void Start()
    {
        _battleSystem = new BattleSystem();
        _gridInteractor = walkableTilemap.GetComponent<GridInteractor>();
        instance = new EventInstance();
        _gridInteractor.OnTileSelected += TileSelected;
        //Subscribe to event click on tile
    }

    private void TileSelected(Vector3Int tilePos)
    {
        Debug.Log("Tile" + tilePos + " clicked");
        if (_selectedUnit)
        {
            //For tests
            /*Pathfinding pathfinding = new Pathfinding();
            foreach (var vector3Int in pathfinding.FindPath(walkableTilemap, SelectedUnitCell(), tilePos))
            {
                walkableTilemap.SetTileFlags(vector3Int, TileFlags.None);
                walkableTilemap.SetColor(vector3Int, Color.magenta);
            }*/

            if (!TurnManager.Instance.isPlayerTurn() || !_selectedUnit.CanMove())
                return;
            _selectedUnit.ChangeMoves(1);
            if (GameManager.Instance.HasEnemyUnit(tilePos))
                if (!StartBattle(GameManager.Instance.GetEnemyUnitInCell(tilePos)))
                    return;
            if (GameManager.Instance.HasEnemyCity(tilePos))
            {
                GameManager.Instance.GetCityInCell(tilePos).TakeDamage(_selectedUnit.BaseUnit.Damage);
                GameManager.Instance.CheckPlayerWin();
                return;
            }

            MoveUnitToTile(_selectedUnit.transform, tilePos);
        }
    }

    public void MoveUnitToTile(Transform unitToMove, Vector3Int tilePos)
    {
        var position = unitToMove.position;
        Vector3Int unitCell = walkableTilemap.WorldToCell(position);
        int countCells = (int) Mathf.Round(Vector3.Distance(tilePos, unitCell));
        if (countCells == 0)
            return;
        Vector3 cellCenterWorld = walkableTilemap.GetCellCenterWorld(tilePos);
        Debug.Log("Move for " + countCells + " cells");
        StartCoroutine(MoveFromTo(unitToMove, position, cellCenterWorld, 3));
        GameManager.Instance.ChangeTakenCell(unitCell, tilePos);
        Debug.Log("End");
        ClearSelected();
    }

    private bool StartBattle(EnemyUnit enemyUnit)
    {
        BaseUnit winner = _battleSystem.Fight(_selectedUnit.BaseUnit, enemyUnit.BaseUnit);
        UnitType winnerType = winner != null ? winner.UnitType : _selectedUnit.BaseUnit.UnitType;
        FMODUnity.RuntimeManager.PlayOneShot(GetWinnerSfxEventToPlay(winnerType), transform.position);  
        if (winner == null)
        {
            GameManager.Instance.KillUnit(enemyUnit);
            GameManager.Instance.KillUnit(_selectedUnit);
        }

        if (_selectedUnit.BaseUnit == winner)
        {
            GameManager.Instance.KillUnit(enemyUnit);
            return true;
        }
        
        GameManager.Instance.KillUnit(_selectedUnit);
        return false;
    }

    private string GetWinnerSfxEventToPlay(UnitType winner)
    {
        string eventToPlay;
        switch (winner)
        {
            case UnitType.Swordman:
            case UnitType.Spearman:
                eventToPlay = "event:/SFX/characters/death_infantry";
                break;
            case UnitType.Horseman:
                eventToPlay = "event:/SFX/characters/death_horseman";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return eventToPlay;
    }
    
    private string GetMovementSfxEventToPlay(UnitType unitType)
    {
        string eventToPlay;
        switch (unitType)
        {
            case UnitType.Swordman:
            case UnitType.Spearman:
                eventToPlay = "event:/SFX/characters/move_infantry";
                break;
            case UnitType.Horseman:
                eventToPlay = "event:/SFX/characters/move_horseman";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return eventToPlay;
    }

    public void ClearSelected()
    {
        SelectedUnit = null;
    }

    private IEnumerator MoveFromTo(Transform objectToMove, Vector3 pos1, Vector3 pos2, float speed)
    {
        if (TurnManager.Instance.isPlayerTurn())
        {
            instance = FMODUnity.RuntimeManager.CreateInstance(GetMovementSfxEventToPlay(_selectedUnit.BaseUnit.UnitType));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, _selectedUnit.transform);
        }
            
        instance.start();
        float step = (speed / (pos1 - pos2).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(pos1, pos2, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
        objectToMove.position = pos2;
    }

    public Vector3Int SelectedUnitCell()
    {
        return walkableTilemap.LocalToCell(_selectedUnit.transform.position);
    }
}