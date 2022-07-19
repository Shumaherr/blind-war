using System;
using System.Collections.Generic;
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

    public void SpawnUnit(Unit unit, Vector3Int cellPos)
    {
        var newUnit = Instantiate(units[unit], GameManager.Instance.Grid.CellToWorld(cellPos), Quaternion.identity);
        GameManager.Instance.AddUnitToList(newUnit);
        
    }
    
}