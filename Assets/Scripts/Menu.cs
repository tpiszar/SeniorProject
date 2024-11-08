using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button level1Tutorial;
    public Button level2Tutorial;
    public Button level3Tutorial;

    public Button level1;
    public Button level2;
    public Button level3;

    public Button tutorial1;
    public Button tutorial2;
    public Button tutorial3;

    // Start is called before the first frame update
    void Start()
    {
        level1Tutorial.gameObject.SetActive(true);
        level2Tutorial.gameObject.SetActive(true);
        level3Tutorial.gameObject.SetActive(true);
        level2Tutorial.interactable = false;
        level3Tutorial.interactable = false;
        
        level1.gameObject.SetActive(false);
        level2.gameObject.SetActive(false);
        level3.gameObject.SetActive(false);

        tutorial1.gameObject.SetActive(false);
        tutorial2.gameObject.SetActive(false);
        tutorial3.gameObject.SetActive(false);

        if (SaveLoad.level1TutorialDone)
        {
            level1Tutorial.gameObject.SetActive(false);
            level1.gameObject.SetActive(true);
            tutorial1.gameObject.SetActive(true);
        }

        if (SaveLoad.level1Done)
        {


            if (SaveLoad.level2TutorialDone)
            {
                level2Tutorial.gameObject.SetActive(false);
                level2.gameObject.SetActive(true);
                tutorial2.gameObject.SetActive(true);
            }
            else
            {
                level2Tutorial.interactable = true;
            }

            if (SaveLoad.level2Done)
            {
                if (SaveLoad.level3TutorialDone)
                {
                    level3Tutorial.gameObject.SetActive(false);
                    level3.gameObject.SetActive(true);
                    tutorial3.gameObject.SetActive(true);
                }
                else
                {
                    level3Tutorial.interactable = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
