using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera mainCamera;

    public Camera MainCamera => mainCamera;

    [SerializeField] private Grid grid;
    private List<BaseUnit> _playerUnits;
    private List<BaseUnit> _enemyUnits;
    private TurnManager _turnManager;
    private ControllerManager _controller;

}
