using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    public int maxJarMana;

    public int startingMana;

    int currentMana;

    public Transform jar;

    public static Mana Instance;

    public float manaGainTime = 5f;
    float nextMana;

    // Start is called before the first frame update
    void Start()
    {
        if (!Instance)
        {
            Instance = this;
            currentMana = startingMana;
            nextMana = manaGainTime;
            UpdateJar();
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        nextMana -= Time.deltaTime;
        if (nextMana < 0)
        {
            nextMana = manaGainTime;
            currentMana++;
            UpdateJar();
        }
    }

    void UpdateJar()
    {
        float ratio = (float)currentMana / maxJarMana;
        Vector3 localScale = jar.localScale;
        localScale.y = ratio;
        jar.localScale = localScale;

        Vector3 localPos = jar.localPosition;
        localPos.y = ratio - 1;
        jar.localPosition = localPos;
    }

    public bool checkMana(int amount)
    {
        if (currentMana < amount)
        {
            return false;
        }
        return true;
    }

    public bool useMana(int amount)
    {
        if (checkMana(amount))
        {
            currentMana -= amount;

            UpdateJar();

            return true;
        }
        return false;
    }

    public void GainMana(int amount)
    {
        currentMana += amount;

        UpdateJar();
    }
}
