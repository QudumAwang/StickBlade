using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
    Animator animator;
    Rigidbody2D rigidbody2d;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask WhatIsGround;
    public float jumpHeight;
    public float moveSpeed;
    public bool grounded;
    public int maxJumps;
    private int jumps;
    public GameObject Axe;
    Quaternion turnRight;
    Quaternion turnLeft;
    float playerWidth;
    public float underPlatformsPosition = -6f;
    public Vector2 DefaultPlayerPosition;
    public int maxLives = 3; // Jumlah nyawa maksimum
    private int currentLives; // Jumlah nyawa saat ini
    public Image[] lifeImages; // Array untuk menyimpan Image nyawa
    public Sprite fullLifeSprite; // Sprite untuk nyawa penuh
    public Sprite emptyLifeSprite; // Sprite untuk nyawa habis
    private bool hasAxe = false; // Status apakah pemain memiliki kapak
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public AudioClip damageSound;
    private AudioSource audioSource;
    public AudioClip pickupSound;



    void Start() {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        SetDirections();
        jumps = maxJumps;
        playerWidth = GetComponent<SpriteRenderer>().size.x * transform.localScale.x;
        DefaultPlayerPosition = new Vector2(-15.192f, transform.localPosition.y);
        currentLives = maxLives;
        audioSource = GetComponent<AudioSource>(); // Ambil AudioSource
        UpdateLifeUI();
    }
    void SetDirections()
    {
        turnRight = transform.rotation;
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        turnLeft = Quaternion.Euler(rot);
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
    void GameOver()
    {
        currentLives = maxLives;
        UpdateLifeUI(); // Perbarui UI nyawa
        MainManager.GameManager.ShowPlayerGravestone(gameObject.transform.localPosition);
        gameObject.SetActive(false);
        MainManager.CanvasManager.SetGameOverActive();
    }


    private void TakeDamage()
    {
        currentLives--; // Kurangi nyawa
        Debug.Log("Lives remaining: " + currentLives);
        UpdateLifeUI();


        // Mainkan suara terkena kerusakan
        audioSource.PlayOneShot(damageSound);

        if (currentLives <= 0)
        {
            GameOver(); // Panggil GameOver jika nyawa habis
        }
        else
        {
            // Jika masih ada nyawa, reset posisi pemain
            ResetRotationAndPosition();
        }
    }

    private void UpdateLifeUI() {
        for (int i = 0; i < lifeImages.Length; i++) {
            if (i < currentLives) {
                lifeImages[i].sprite = fullLifeSprite;
            } else {
                lifeImages[i].sprite = emptyLifeSprite;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ckeckCollisionWithEnemies(collision))
        {
            TakeDamage();
        }
    }
    bool ckeckCollisionWithEnemies(Collision2D collision)
    {
        return
            collision.gameObject.CompareTag("Enemy")
            || collision.gameObject.CompareTag("AxeEnemy");
    }
    public void ResetRotationAndPosition()
    {
        transform.localPosition = DefaultPlayerPosition;
        transform.rotation = turnRight;
    }
    public void SetPlayerFreeze()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        if(animator != null)
            animator.enabled = false;
    }
    public void SetPlayerMoving()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        animator.enabled = true;
    }

    // Update is called once per frame
    void Update() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, WhatIsGround) ;
        if (grounded)
        {
            jumps = maxJumps;
        }
        animator.SetFloat("Speed", Mathf.Abs(rigidbody2d.linearVelocity.x));
        animator.SetBool("Grounded", grounded);
        

        if (MainManager.GameManager.GameMode != Assets.GameModeEnum.GAME)
            return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetAxisRaw("Horizontal") < 0)
        {
            rigidbody2d.linearVelocity = new Vector2(-moveSpeed, rigidbody2d.linearVelocity.y);
            transform.rotation = turnLeft;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetAxisRaw("Horizontal") > 0)
        {
            rigidbody2d.linearVelocity = new Vector2(moveSpeed, rigidbody2d.linearVelocity.y);
            transform.rotation = turnRight;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Shoot();
        }
        if (transform.localPosition.y < underPlatformsPosition)
            GameOver();
        if (!Input.anyKey)
            rigidbody2d.linearVelocity = new Vector2(rigidbody2d.linearVelocity.x * 0.95f, rigidbody2d.linearVelocity.y);
    }
    IEnumerator DecreaseJumpsAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        jumps --;
    }
    private void Shoot()
    {
        if (!hasAxe)
        {
            Debug.Log("You need to pick up the axe first!");
            return; // Tidak bisa menembak jika belum memiliki kapak
        }

        if (transform.rotation.eulerAngles.y == 0)
            Instantiate(Axe, new Vector3(transform.position.x + playerWidth, transform.position.y, 0), transform.rotation);
        else
            Instantiate(Axe, new Vector3(transform.position.x - playerWidth, transform.position.y, 0), transform.rotation);

        // Mainkan suara menyerang
        audioSource.PlayOneShot(attackSound);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AxePickup")) // Tag untuk objek kapak
        {
            hasAxe = true; // Pemain sekarang memiliki kapak
            Destroy(collision.gameObject); // Hapus objek kapak dari dunia
            Debug.Log("Axe picked up! You can now shoot.");
            audioSource.PlayOneShot(pickupSound);
        }
    }
    private void Jump()
    {
        if (jumps > 0)
        {
            grounded = false;
            rigidbody2d.linearVelocity = new Vector2(0, jumpHeight);
            StartCoroutine(DecreaseJumpsAfterDelay());


            // Mainkan suara lompat
            audioSource.PlayOneShot(jumpSound);
        }
        if (jumps == 0)
        {
            return;
        }
    }
}
