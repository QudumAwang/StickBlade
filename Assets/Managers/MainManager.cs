using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainManager
{
    public static CanvasManager CanvasManager;
    public static MainEnemyManager MainEnemyManager;
    public static GameManager GameManager;
    public static ElevatorManager ElevatorManager;
    public static PlayerManager PlayerManager;
    static GameObject Managers;

    // Start is called before the first frame update
    static MainManager()
    {
        Managers = GameObject.Find("Managers");
        CanvasManager = Managers.GetComponent<CanvasManager>();
        MainEnemyManager = Managers.GetComponent<MainEnemyManager>();
        GameManager = Managers.GetComponent<GameManager>();
        ElevatorManager = GameObject.Find("Elevator").GetComponent<ElevatorManager>();
        PlayerManager = GameObject.Find("Stickman").GetComponent<PlayerManager>();
    }
}
