using System.Collections;
using TMPro;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int maxHealth;
    public int startingHealth;
    int health;

    public bool immune = false;

    public bool annihilate = false;

    public bool startMini = false;
    GameObject mini;

    bool isPlayer;

    public TextMeshProUGUI healthText;
    Color startingColor;

    public AudioClip damageSound;
    [Range(0.0001f, 1f)]
    public float damageVolume = 1;
    public AudioClip destroySound;
    [Range(0.0001f, 1f)]
    public float destroyVolume = 1;

    public Vector2 fadeBounds = new Vector2(0.2f, 0.8f);

    public GameObject shatterBarrier;

    public Renderer mainRend;
    public float flashSpeed = 0.5f;
    protected Coroutine currentFlash;

    Material barrierMat;
    Color baseColor;
    Color curColor;
    Color maxColor;
    float startIntensity = 1f;
    public float endIntensity = .5f;
    public float maxIntensity = 1.75f;

    public float antiFlashRate = 1.4f;
    float nextFlash = 1;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (startMini)
        {
            mini = MiniMapTracker.instance.AddMapBarrier(transform);
        }
        
        if (health == 0)
        {
            isPlayer = true;
        }

        if (healthText)
        {
            startingHealth = maxHealth;

            startingColor = healthText.color;
            healthText.text = health.ToString();
        }

        if (!isPlayer)
        {
            health = startingHealth;

            barrierMat = mainRend.material;

            baseColor = barrierMat.GetColor("_Color");

            float newIntensity = Mathf.Lerp(endIntensity, startIntensity, (float)health / maxHealth);

            curColor = baseColor * (newIntensity / startIntensity);

            barrierMat.SetColor("_Color", curColor);

            maxColor = baseColor * (maxIntensity / startIntensity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //nextFlash -= Time.deltaTime;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (!immune)
        {
            health -= damage;
        }

        if (annihilate)
        {
            Destroy(attacker);
        }

        if (healthText)
        {
            healthText.text = health.ToString();
            float t = (float)health / maxHealth;
            Color lerpColor = Color.Lerp(Color.red, startingColor, t);
            healthText.color = lerpColor;
        }

        if (health <= 0)
        {
            if (isPlayer)
            {
                GetComponent<PlayerDeath>().Die();
            }
            else
            {
                SoundManager.instance.PlayClip(destroySound, transform.position, destroyVolume);
                Destroy(gameObject);
            }
        }
        else
        {
            SoundManager.instance.PlayClip(damageSound, transform.position, damageVolume);

            if (!isPlayer)
            {
                float newIntensity = Mathf.Lerp(endIntensity, startIntensity, (float)health / maxHealth);

                curColor = baseColor * (newIntensity / startIntensity);


                if (nextFlash > 0)
                {
                    barrierMat.SetColor("_Color", curColor);
                }
                else
                {
                    if (currentFlash != null)
                    {
                        StopCoroutine(currentFlash);
                    }

                    currentFlash = StartCoroutine(DamageFlash());
                }
            }
        }
    }

    IEnumerator DamageFlash()
    {
        nextFlash = antiFlashRate;

        float timer = 0;

        barrierMat.SetColor("_Color", maxColor);

        timer = 0;
        while (timer < flashSpeed)
        {
            timer += Time.deltaTime;

            barrierMat.SetColor("_Color", Color.Lerp(curColor, maxColor, timer / flashSpeed));

            yield return null;
        }

        barrierMat.SetColor("_Color", curColor);
    }

    public bool CanMaximizeCharge(int gain)
    {
        return (health + gain) <= maxHealth;
    }

    public void GainHealth(int gain)
    {
        health += gain;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (!isPlayer)
        {
            float newIntensity = Mathf.Lerp(endIntensity, startIntensity, (float)health / maxHealth);

            curColor = baseColor * (newIntensity / startIntensity);

            barrierMat.SetColor("_Color", curColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Root") || other.CompareTag("Shade Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                enemy.barrierClose(transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Root") || other.CompareTag("Shade Root"))
        {
            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy)
            {
                enemy.barrierFar(transform);
            }
        }
    }

    private void OnDestroy()
    {
        if (UIScript.sceneLoading) return;

        if (mini)
        {
            Destroy(mini);
        }

        if (shatterBarrier)
        {
            Instantiate(shatterBarrier, transform.position, transform.rotation);
        }
    }
}
