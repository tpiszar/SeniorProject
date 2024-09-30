using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    public int maxJarMana;

    public int startingMana;

    int currentMana;

    public Transform fill;
    //public Transform useFill;
    //public Transform gainFill;

    public static Mana Instance;

    public float manaGainTime = 5f;
    float nextMana;

    //public int preUse = 0;
    //public int preGain = 0;

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
        float ratio = 1;

        if (currentMana < maxJarMana)
        {
            ratio = (float)currentMana / maxJarMana;
        }



        Vector3 localScale = fill.localScale;
        localScale.y = ratio;
        fill.localScale = localScale;

        Vector3 localPos = fill.localPosition;
        localPos.y = ratio - 1;
        fill.localPosition = localPos;
    }

    public bool CheckMana(int amount)
    {
        if (currentMana < amount)
        {
            return false;
        }
        return true;
    }

    //public void PreUsing(int amount)
    //{
    //    preUse += amount;
    //    if (preUse > 0)
    //    {
    //        useFill.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        useFill.gameObject.SetActive(false);
    //    }
    //    UpdateJar();
    //}

    //public void PreGaining(int amount)
    //{
    //    preGain += amount;
    //    if (preGain > 0)
    //    {
    //        gainFill.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        gainFill.gameObject.SetActive(false);
    //    }
    //    UpdateJar();
    //}

    public bool UseMana(int amount)
    {
        if (CheckMana(amount))
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
