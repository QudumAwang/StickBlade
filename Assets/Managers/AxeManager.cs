using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeManager : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    Rigidbody2D rigidbody2d;
    Vector2 startPosition;
    int direction = 1;
    public int MaxDistance;
    public AxeTypeEnum AxeTypeEnum;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        startPosition = gameObject.transform.localPosition;
        SetDirections();
        transform.parent = MainManager.GameManager.AxeStore.transform;
    }
    void SetDirections()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x + 180, rot.y, rot.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.eulerAngles.y == 0)
            direction = 1;
        else if (transform.rotation.eulerAngles.y == 180)
            direction = -1;

        if (direction == 1 && startPosition.x + MaxDistance * direction < gameObject.transform.localPosition.x)
            Destroy(gameObject);
        else if (direction == -1 && startPosition.x + MaxDistance * direction > gameObject.transform.localPosition.x)
            Destroy(gameObject);

        rigidbody2d.linearVelocity = new Vector2(moveSpeed * direction, rigidbody2d.linearVelocity.y);
        Rotate();
    }

    private void Rotate()
    {
        if(rigidbody2d.bodyType == RigidbodyType2D.Dynamic)
            transform.Rotate(Vector3.forward * -rotateSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
