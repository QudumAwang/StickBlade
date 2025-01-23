using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject Player;
    private Vector3 Offset;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        Offset = transform.position - Player.transform.position;
        Offset.y -= 1.5f;
    }

    void LateUpdate()
    {
        transform.position = Player.transform.position + Offset;
    }
}
