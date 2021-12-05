using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    private float speed;
    public float boostTimer;
    private bool boosting;

    public int maxHealth = 5;

    public int cogAmount;

    public GameObject projectilePrefab;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    bool GameOver;
    bool Stage1Done;
    bool FourRobots;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip victorySound;
    public AudioClip lossSound;
    public AudioClip backgroundMusic;
    public AudioClip collectedClip;
    public AudioClip pickUpCog;
    public AudioClip cogBagSound;
    public AudioClip speedMusic;
    public AudioSource musicSource;

    public int scoreValue;

    public Text score;
    public Text winText;
    public Text loseText;
    public Text createdBy;
    public Text cogs;
    public Text visitJambi;
    public Text speedTime;


    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        winText.text = " ";
        loseText.text = "";
        createdBy.text = " ";
        cogs.text = "";
        visitJambi.text = "";
        speedTime.text = " ";
        scoreValue = 0;
        GameOver = false;
        Stage1Done = false;
        FourRobots = false;
        cogAmount = 4;

        speed = 3;
        boostTimer = 8;
        boosting = false;

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        musicSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        cogs.text = "Cogs: " + cogAmount.ToString();

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if(FourRobots == true)
                    {
                        SceneManager.LoadScene("Challenge3 real with scene 2");
                        Stage1Done = true;
                    }
                    else
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (GameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // this loads the currently active scene
            }
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Challenge3 real with scene 2"))
        {
            Stage1Done = true;
            print("Stage2!");
        }

        if (boosting == true)
        {

            //speedTime.text = "TIME: " + boostTimer.ToString(); 
            boostTimer -= Time.deltaTime;            
            
            if (boostTimer <= 0)
            {
                speed = 3;
                boostTimer = 0;
                boosting = false;
                musicSource.clip = backgroundMusic;
                musicSource.Play();
                speedTime.text = " ";
            }
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            PlaySound(hitSound);
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth <= 0)
        {
            musicSource.clip = lossSound;
            musicSource.Play();
            loseText.text = "YOU LOSE! Press r to restart";
            createdBy.text = "Created by: Hallie Richardson";
            GameOver = true;
            speed = 0;
            //Destroy(GameObject);
        }

    }

    void Launch()
    {
        if (cogAmount > 0)
        { GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);

            cogAmount = cogAmount - 1;
        }

    }

    public void changeScore(int addAmount)
    {
        scoreValue = scoreValue + addAmount;
        score.text = "FixedRobots: " + scoreValue.ToString();

        if (scoreValue >= 4)
        {
            if (Stage1Done == true)
            {
                winText.text = "YOU WIN! Press r to restart";
                createdBy.text = "Created by: Hallie Richardson";
                musicSource.clip = victorySound;
                musicSource.Play();
                speed = 0;
                GameOver = true;
            }
            else
            {
                visitJambi.text = "Visit Jambi for Stage 2!";
                musicSource.clip = victorySound;
                musicSource.Play();
                FourRobots = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Cog")
        {
            cogAmount += 1;
            Destroy(collision.collider.gameObject);
            PlaySound(pickUpCog);
        }
        if(collision.collider.tag == "CogBag")
        {
            cogAmount += 8;
            Destroy(collision.collider.gameObject);
            PlaySound(cogBagSound);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "SpeedBoost")
        {
            boosting = true;
            speed = 6;
            musicSource.clip = speedMusic;
            musicSource.Play();
            Destroy(other.gameObject);
        }
    }
 }