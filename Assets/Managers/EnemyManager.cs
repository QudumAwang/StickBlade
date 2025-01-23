using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Assets.Enums;

public class EnemyManager : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public float MoveSpeed;
    Quaternion turnRight;
    Quaternion turnLeft;
    public GameObject StartEnemyPosition;
    public GameObject EndEnemyPosition;
    public Vector2 DefaultEnemyPosition;
    int direction = 1;
    public EnemyStateEnum EnemyStateEnum;
    void SetDirections()
    {
        turnRight = transform.rotation;
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        turnLeft = Quaternion.Euler(rot);
    }
    public void ResetRotationAndPosition()
    {
        transform.localPosition = DefaultEnemyPosition;
        transform.rotation = turnRight;
        direction = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        DefaultEnemyPosition = transform.localPosition;
        SetDirections();
        EnemyStateEnum = EnemyStateEnum.ALIVE;
    }
    void FixedUpdate()
    {
        if (MainManager.GameManager.GameMode != Assets.GameModeEnum.GAME)
            return;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
        if (rigidbody2d.linearVelocity.magnitude < .01)
        {
            rigidbody2d.linearVelocity = Vector3.zero;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.x <= StartEnemyPosition.transform.position.x)
        {
            transform.rotation = turnRight;
            direction = 1;
        }
        else if (transform.localPosition.x >= EndEnemyPosition.transform.position.x)
        {
            transform.rotation = turnLeft;
            direction = -1;
        }

        rigidbody2d.linearVelocity = new Vector2(MoveSpeed * direction, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Axe"))
        {
            Die();
        }
    }

    private void Die()
    {
        MainManager.GameManager.ShowEnemyGravestone(gameObject.transform.localPosition);
        gameObject.SetActive(false);
        EnemyStateEnum = EnemyStateEnum.DEAD;
    }
}
