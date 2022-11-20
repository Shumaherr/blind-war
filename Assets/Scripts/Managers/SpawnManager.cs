using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> unitsPrefabs;
    private Dictionary<Unit, Transform> units;

    private void Awake()
    {
        units = new Dictionary<Unit, Transform>();
        unitsPrefabs.ForEach(p => units.Add(p.GetComponent<Unit>(), p));
    }

    public GameObject SpawnUnit(BaseUnit unit, Player owner, Vector3Int cellPos)
    {
        var controllerType = owner.Type == PlayerType.AI ? typeof(AIController) : typeof(PlayerController);
        var unitToInst = units.First(u => u.Key.BaseUnit.GetType() == unit.GetType()).Value;
        var newUnit = Instantiate(unitToInst,
            GameManager.Instance.Grid.GetCellCenterWorld(cellPos), Quaternion.identity);
        newUnit.gameObject.AddComponent(controllerType);
        var unitComponent = newUnit.gameObject.GetComponent<Unit>();
        unitComponent.InitUnit(owner);
        var cellUnitPos = GameManager.Instance.Grid.WorldToCell(newUnit.transform.position);
        ControllerManager.Instance.AllUnits.Add(cellPos, unitComponent);
        return newUnit.gameObject;
    }
}