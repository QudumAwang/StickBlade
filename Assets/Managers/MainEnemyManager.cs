using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemyManager : MonoBehaviour
{
    public List<GameObject> Enemies;
    public GameObject AxeEnemy;
    public float DefaultEnemySpeed = 1;
    System.Random Random;
    float enemyWidth;
    // Start is called before the first frame update
    void Start()
    {
        Random = new System.Random();
        enemyWidth = Enemies[0].GetComponent<SpriteRenderer>().size.x * transform.localScale.x;
    }
    public void StartRandomShoting()
    {
        foreach(var enemy in Enemies)
        {
            StartCoroutine(RandomShoot(enemy));
        }
    }
    IEnumerator RandomShoot(GameObject enemy)
    {
        int seconds = Random.Next(1, 5);
        yield return new WaitForSeconds(seconds);
        yield return new WaitUntil(() => MainManager.GameManager?.GameMode == Assets.GameModeEnum.GAME && enemy.GetComponent<EnemyManager>().EnemyStateEnum == Assets.Enums.EnemyStateEnum.ALIVE);

        if (transform.rotation.eulerAngles.y == 0)
            Instantiate(MainManager.MainEnemyManager.AxeEnemy, new Vector3(enemy.transform.position.x + enemyWidth, enemy.transform.position.y, 0), enemy.transform.rotation);
        else
            Instantiate(MainManager.MainEnemyManager.AxeEnemy, new Vector3(enemy.transform.position.x - enemyWidth, enemy.transform.position.y, 0), enemy.transform.rotation);
        StartCoroutine(RandomShoot(enemy));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
