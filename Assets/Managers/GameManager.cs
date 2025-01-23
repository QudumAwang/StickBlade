using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameModeEnum GameMode;
    public GameObject Key;
    public GameObject Doors;
    public GameObject EnemyGravestone;
    public GameObject PlayerGravestone;
    public GameObject AxeStore;
    public bool IsKeyAchieved;
    public bool AreDoorsAchieved;
    public AudioClip loseSound; // Tambahkan referensi loseSound
    private AudioSource audioSource;
    List<GameObject> gravestones;
    // Start is called before the first frame update
    private void Awake()
    {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is missing on GameManager!");
            }
    }
    void Start()
    {
        GameMode = GameModeEnum.MAIN_MENU;
        gravestones = new List<GameObject>();
    }
    void StopAxes()
    {
        foreach(var axe in AxeStore.GetComponentsInChildren<AxeManager>())
        {
            axe.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameMode == GameModeEnum.GAME)
            {
                MainManager.CanvasManager.SetPauseActive();
            }
            else if (GameMode == GameModeEnum.PAUSE)
            {
                MainManager.CanvasManager.SetGameActive();
            }
        }
    }
    public void StartGame()
    {
        ResetGame();
        MainManager.GameManager.IsKeyAchieved = false;
        AreDoorsAchieved = false;
        AreDoorsAchieved = false;
        MainManager.CanvasManager.SetItemsOnScreen();
        StartCoroutines();
        GameMode = GameModeEnum.GAME;
    }

    private void StartCoroutines()
    {
        MainManager.MainEnemyManager.StartRandomShoting();
        StartCoroutine(WaitForAllEnemiesKilled());
        StartCoroutine(WaitForKeyToGet());
        StartCoroutine(WaitForDoorsToGet());
    }
    void ShowKey()
    {
        Key.SetActive(true);
    }
    IEnumerator WaitForAllEnemiesKilled()
    {
        yield return new WaitUntil(() => MainManager.MainEnemyManager.Enemies.Where(x => x.activeSelf == true).Count() == 0);
        ShowKey();
    }
    void ShowDoors()
    {
        Doors.SetActive(true);
    }

    private IEnumerator WaitForDoorsToGet()
    {
        yield return new WaitUntil(() => AreDoorsAchieved);
        MainManager.CanvasManager.SetWinGameActive();
    }

    public void WinGame()
    {
        StopMovingObjects();
        GameMode = GameModeEnum.WIN_GAME;
    }

    private IEnumerator WaitForKeyToGet()
    {
        yield return new WaitUntil(() => IsKeyAchieved);
        ShowDoors();
        MainManager.CanvasManager.KeyImage.SetActive(true);
    }

    public void GoToMainMenu()
    {
        StopMovingObjects();
        GameMode = GameModeEnum.MAIN_MENU;
    }
    public void ResetGame()
    {
        SetObjectsVisiblity();
        SetDefaultPositionsForObjects();
        MainManager.ElevatorManager.StopAllCoroutines();
        StartMovingObjects();
        DestroyAxes();
    }
    private void ResumeAxes()
    {
        foreach (var axe in AxeStore.GetComponentsInChildren<AxeManager>())
        {
            axe.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    private void DestroyAxes()
    {
        foreach (var axe in AxeStore.GetComponentsInChildren<AxeManager>())
        {
            Destroy(axe.gameObject);
        }
    }

    private void SetObjectsVisiblity()
    {
        foreach (var enemy in MainManager.MainEnemyManager.Enemies)
        {
            enemy.SetActive(true);
        }
        foreach(var gravestone in gravestones)
        {
            Destroy(gravestone);
        }
        gravestones.Clear();
        Key.gameObject.SetActive(false);
        Doors.gameObject.SetActive(false);
        MainManager.PlayerManager.gameObject.SetActive(true);
    }

    private void SetDefaultPositionsForObjects()
    {
        MainManager.PlayerManager.GetComponent<PlayerManager>().ResetRotationAndPosition();
        MainManager.ElevatorManager.transform.localPosition = MainManager.ElevatorManager.DefaultElevatorPosition;
        foreach (var enemy in MainManager.MainEnemyManager.Enemies)
        {
            enemy.GetComponent<EnemyManager>().ResetRotationAndPosition();
            enemy.GetComponent<EnemyManager>().EnemyStateEnum = Assets.Enums.EnemyStateEnum.ALIVE;
            enemy.GetComponent<Animator>().enabled = true;
        }
    }

    internal void GameOver()
    {
        StopMovingObjects();
        MainManager.PlayerManager.SetPlayerFreeze();
        GameMode = GameModeEnum.GAME_OVER;
            if (audioSource && loseSound)
            {
                audioSource.PlayOneShot(loseSound);
            }
    }

    public void PauseGame()
    {
        StopMovingObjects();
        GameMode = GameModeEnum.PAUSE;
    }

    private void StopMovingObjects()
    {
        MainManager.ElevatorManager.MoveSpeed = 0;
        MainManager.ElevatorManager.CanElevatorMove = false;
        MainManager.PlayerManager.SetPlayerFreeze();
        foreach (var enemy in MainManager.MainEnemyManager.Enemies)
        {
            enemy.GetComponent<EnemyManager>().MoveSpeed = 0;
            enemy.GetComponent<Animator>().enabled = false;
        }
        StopAxes();
    }

    public void ResumeGame()
    {
        StartMovingObjects();
        GameMode = GameModeEnum.GAME;
    }

    private void StartMovingObjects()
    {
        MainManager.PlayerManager.SetPlayerMoving();
        MainManager.ElevatorManager.MoveSpeed = MainManager.ElevatorManager.DefaultElevatorSpeed;
        MainManager.ElevatorManager.CanElevatorMove = true;
        foreach (var enemy in MainManager.MainEnemyManager.Enemies)
        {
            enemy.GetComponent<EnemyManager>().MoveSpeed = MainManager.MainEnemyManager.DefaultEnemySpeed;
            enemy.GetComponent<Animator>().enabled = true;
        }
        ResumeAxes();
    }
    public void ShowEnemyGravestone(Vector2 location)
    {
        gravestones.Add(Instantiate(EnemyGravestone, new Vector3(location.x, location.y, 0), EnemyGravestone.transform.rotation));
    }
    public void ShowPlayerGravestone(Vector2 location)
    {
        gravestones.Add(Instantiate(PlayerGravestone, new Vector3(location.x, location.y, 0), PlayerGravestone.transform.rotation));
    }
}
