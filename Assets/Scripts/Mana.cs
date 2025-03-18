using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mana : MonoBehaviour
{
    public int maxJarMana;

    public int startingMana;

    int currentMana;

    //public Transform fill;
    //public Transform useFill;
    //public Transform gainFill;

    public static Mana Instance;

    public float manaGainTime = 5f;
    float nextMana;

    public int preUse = 0;
    public int preGain = 0;

    public TextMeshProUGUI manaText;

    public MeshRenderer fillMesh;
    public MeshRenderer useFillMesh;
    public MeshRenderer gainFillMesh;

    public AnimationCurve fillCurve;

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
        if (manaText)
        {
            manaText.text = currentMana.ToString();
        }

        float ratio = 1;


        float appendedMana = currentMana - preUse;

        if (currentMana > maxJarMana)
        {
            appendedMana = maxJarMana - preUse;
        }

        if (appendedMana + preGain > maxJarMana)
        {
            appendedMana = maxJarMana - preUse - preGain;
        }

        //appendedMana -= preGain;

        //if (preUse > currentMana)
        //{
        //    appendedMana = currentMana;
        //}

        //if (appendedMana < maxJarMana)
        //{
        //    ratio = (float)appendedMana / maxJarMana;
        //}

        ratio = (float)appendedMana / maxJarMana;

        fillMesh.material.SetFloat("_Fill", fillCurve.Evaluate(ratio));

        //Vector3 localScale = fill.localScale;
        //localScale.y = ratio;
        //fill.localScale = localScale;

        //Vector3 localPos = fill.localPosition;
        //localPos.y = ratio - 1;
        //fill.localPosition = localPos;

        float useRatio = ((float)preUse + appendedMana) / maxJarMana;

        //if (preUse > currentMana)
        //{
        //    useRatio = 0;
        //}

        useFillMesh.enabled = preUse > 0;
        useFillMesh.material.SetFloat("_Fill", fillCurve.Evaluate(useRatio));

        //Vector3 useLocalScale = useFill.localScale;
        //useLocalScale.y = useRatio + .001f; // Anti z fighting
        //useFill.localScale = useLocalScale;

        //Vector3 useLocalPos = useFill.localPosition;
        //useLocalPos.y = Mathf.Clamp(localPos.y + localScale.y + useLocalScale.y, useRatio - 1, 1 - useRatio);
        //useFill.localPosition = useLocalPos;

        float gainRatio = ((float)preGain + preUse + appendedMana) / maxJarMana;

        gainFillMesh.enabled = preGain > 0;
        gainFillMesh.material.SetFloat("_Fill", fillCurve.Evaluate(gainRatio));

        //Vector3 gainLocalScale = gainFill.localScale;
        //gainLocalScale.y = gainRatio; // Anti z fighting
        //gainFill.localScale = gainLocalScale;

        //Vector3 gainLocalPos = gainFill.localPosition;
        //gainLocalPos.y = Mathf.Clamp(useLocalPos.y + useLocalScale.y + gainLocalScale.y, gainRatio - 1, 1 - gainRatio);
        //gainFill.localPosition = gainLocalPos;
    }

    public bool CheckMana(int amount)
    {
        if (currentMana < amount)
        {
            return false;
        }
        return true;
    }

    public void PreUsing(int amount)
    {

        preUse += amount;
        //if (preUse > 0)
        //{
        //    useFill.gameObject.SetActive(true);
        //}
        //else
        //{
        //    useFill.gameObject.SetActive(false);
        //}
        UpdateJar();
    }

    public void PreGaining(int amount)
    {
        preGain += amount;
        //if (preGain > 0)
        //{
        //    gainFill.gameObject.SetActive(true);
        //}
        //else
        //{
        //    gainFill.gameObject.SetActive(false);
        //}
        UpdateJar();
    }

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
