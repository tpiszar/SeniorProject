using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndlessScreen : MonoBehaviour
{
    public UIScript ui;

    public Button level1;
    public Button level2;
    public Button level3;

    int curLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectMap(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectMap(int level)
    {
        curLevel = level;

        switch (curLevel)
        {
            case 0:
                level1.interactable = false;
                level2.interactable = true;
                level3.interactable = true;
                break;
            case 1:
                level1.interactable = true;
                level2.interactable = false;
                level3.interactable = true;
                break;
            case 2:
                level1.interactable = true;
                level2.interactable = true;
                level3.interactable = false;
                break;
        }
    }

    public void StartEndless()
    {
        ui.LoadScene("Endless" + (curLevel + 1).ToString());
    }
}
