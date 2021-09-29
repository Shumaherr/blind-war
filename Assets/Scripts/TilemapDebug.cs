using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDebug : MonoBehaviour
{
    private Tilemap _tilemap;

    // Start is called before the first frame update
    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (var cell in GameManager.Instance.TakenCells)
        {
            _tilemap.SetTileFlags(cell, TileFlags.None);
            _tilemap.SetColor(cell, Color.red);
        }
    }
}