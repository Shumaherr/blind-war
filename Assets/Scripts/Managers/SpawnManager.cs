using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> unitsPrefabs;
    private Dictionary<Unit, Transform> units;

    private void Start()
    {
        units = new Dictionary<Unit, Transform>();
        unitsPrefabs.ForEach(p => units.Add(p.GetComponent<Unit>(), p));
    }

    public void SpawnUnit(BaseUnit unit, CityOwner owner, Vector3Int cellPos)
    {
        var type = owner == CityOwner.Player ? typeof(BaseInteractable) : typeof(EnemyUnitBase);
        var newUnit = Instantiate(units.First(u => u.Key.BaseUnit == unit && u.Key.GetType().BaseType == type).Value,
            GameManager.Instance.Grid.CellToWorld(cellPos), Quaternion.identity);
        GameManager.Instance.AddUnitToList(newUnit);
    }
}