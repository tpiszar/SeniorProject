using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostUI : MonoBehaviour
{
    public TextMeshProUGUI costText;
    Color canColor;
    public int manaCost;

    // Start is called before the first frame update
    void Start()
    {
        canColor = costText.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mana.Instance.CheckMana(manaCost))
        {
            costText.color = canColor;
        }
        else
        {
            costText.color = Color.red;
        }
    }
}
