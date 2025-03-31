using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button level1;
    public Button level2;
    public Button level3;

    public Button tutorial1;
    public Button tutorial2;
    public Button tutorial3;

    public Button endless;

    // Start is called before the first frame update
    void Start()
    {

        level2.interactable = false;
        level3.interactable = false;
        endless.interactable = false;
        
        //level1.gameObject.SetActive(true);
        //level2.gameObject.SetActive(false);
        //level3.gameObject.SetActive(false);

        tutorial1.gameObject.SetActive(false);
        tutorial2.gameObject.SetActive(false);
        tutorial3.gameObject.SetActive(false);

        //endless.gameObject.SetActive(false);

        if (SaveLoad.level1TutorialDone)
        {
            //level1.gameObject.SetActive(true);
            tutorial1.gameObject.SetActive(true);
        }

        if (SaveLoad.level1Done)
        {
            //level2.gameObject.SetActive(true);
            level2.interactable = true;
            if (SaveLoad.level2TutorialDone)
            {

                tutorial2.gameObject.SetActive(true);
            }

            if (SaveLoad.level2Done)
            {
                //level3.gameObject.SetActive(true);
                level3.interactable = true;
                if (SaveLoad.level3TutorialDone)
                {
                    //level3Tutorial.gameObject.SetActive(false);
                    
                    tutorial3.gameObject.SetActive(true);
                }
                else
                {
                    //level3Tutorial.interactable = true;
                }

                if (SaveLoad.level3Done)
                {
                    //endless.gameObject.SetActive(true);
                    endless.interactable = true;
                }
            }
        }
    }
}
