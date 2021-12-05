using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieController : MonoBehaviour
{
    public int maxHealth = 3;

    public int currentHealth;

    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    //public ParticleSystem smokeEffect;
    public GameObject explosionParticlesPrefab;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool alive = true;

    Animator animator;

    private RubyController rubyController;
    //public GameObject zombiePrefab;
    public zombieHealthBar zombieHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        zombieHealthBar.SetHealth(currentHealth, maxHealth);

        GameObject rubyControllerObject = GameObject.FindWithTag("Player");
        if (rubyControllerObject != null)

        {

            rubyController = rubyControllerObject.GetComponent<RubyController>(); //and this line of code finds the rubyController and then stores it in a variable

            print("Found the RubyController Script!");

        }

        if (rubyController == null)

        {

            print("Cannot find GameController Script!");

        }
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!alive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!alive)
        {
            return;
        }

        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
        }

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
            GameObject explosionObject = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        }
    }

    //Public because we want to call it from elsewhere like the projectile script
    public void Die(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        //zombieHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        zombieHealthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            alive = false;
            rigidbody2D.simulated = false;
            //optional if you added the fixed animation
            animator.SetTrigger("Die");
            //rubyController.changeScore(1);
            //Destroy(gameObject);
        }
    }
}
