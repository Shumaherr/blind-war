using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Tilemaps;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class ControllerManager : Singleton<ControllerManager>
{
    private GridInteractor _gridInteractor;
    private UnitInteractable _selectedUnit;

    private EventInstance instance;
    [SerializeField] private Tilemap walkableTilemap;

    public UnitInteractable SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            
                GameManager.Instance.UnselectUnit(); //TODO redraw inventory
            _selectedUnit = value;
        }
    }

    private void Start()
    {
        _gridInteractor = walkableTilemap.GetComponent<GridInteractor>();
        instance = new EventInstance();
        _gridInteractor.OnTileSelected += TileSelected;
        //Subscribe to event click on tile
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
            if (!TurnManager.Instance.isPlayerTurn() || !_selectedUnit.CanMove())
                return;
            if (MapManager.Instance.GetTurnPoints(_selectedUnit.BaseUnit, walkableTilemap.GetTile(tilePos)) > _selectedUnit.Moves)
                return;
            _selectedUnit.ChangeMoves(MapManager.Instance.GetTurnPoints(_selectedUnit.BaseUnit, walkableTilemap.GetTile(tilePos)));
            if (GameManager.Instance.HasEnemyUnit(tilePos))
                if (!StartBattle(GameManager.Instance.GetEnemyUnitInCell(tilePos), _selectedUnit))
                    return;

            MoveUnitToTile(_selectedUnit.transform, tilePos);
            var tempUnit = GameManager.Instance.PlayerUnits[_selectedUnit.GetUnitCell()];
            GameManager.Instance.PlayerUnits.Remove(_selectedUnit.GetUnitCell());
            GameManager.Instance.PlayerUnits.Add(tilePos, tempUnit);
            ClearSelected();
        }
    }

    public void MoveUnitToTile(Transform unitToMove, Vector3Int tilePos)
    {
        var position = unitToMove.position;
        var unitCell = walkableTilemap.WorldToCell(position);
        var countCells = (int) Mathf.Round(Vector3.Distance(tilePos, unitCell));
        if (countCells == 0)
            return;
        var cellCenterWorld = walkableTilemap.GetCellCenterWorld(tilePos);
        Debug.Log("Move for " + countCells + " cells");
        StartCoroutine(MoveFromTo(unitToMove, position, cellCenterWorld, 3));
        GameManager.Instance.ChangeTakenCell(unitCell, tilePos);
        Debug.Log("End");
    }

    public bool StartBattle(Unit enemyUnit, Unit playerUnit)
    {
        if (!enemyUnit.enabled || !playerUnit.enabled)
            return false;
        var winner = BattleSystem.Fight(playerUnit, enemyUnit);
        if (winner == null)
            return false;
        var winnerType = winner != null ? winner.BaseUnit.UnitType : playerUnit.BaseUnit.UnitType;
        RuntimeManager.PlayOneShot(SoundManager.GetWinnerSfxEventToPlay(winnerType), transform.position);

        if (playerUnit.BaseUnit == winner)
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
        if (TurnManager.Instance.isPlayerTurn())
        {
            instance = RuntimeManager.CreateInstance(SoundManager.GetMovementSfxEventToPlay(_selectedUnit.BaseUnit.UnitType));
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
    }

    public Vector3Int SelectedUnitCell()
    {
        return walkableTilemap.LocalToCell(_selectedUnit.transform.position);
    }
}