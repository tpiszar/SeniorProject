using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static bool towerPlacing = true;
    public static bool energyBlast = true;
    public static bool fireball = true;
    public static bool lightning = true;
    public static bool barrier = true;
    public static bool teleport = true;
    public static bool crystalTower = true;
    public static bool flameOrbTower = true;
    public static bool psychicTower = true;

    public GameObject[] tutObjs;
    List<int> tutorials = new List<int>();
    int counter = -1;
    int curTut = 0;

    bool replayTower = false;
    bool replayEnergy = false;
    bool replayFireball = false;
    bool replayLightning = false;
    bool replayBarrier = false;
    bool replayTeleport = false;
    bool replayCrystal = false;
    bool replayFlameOrb = false;
    public GameObject endScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (towerPlacing)
        {
            tutorials.Add(0);
        }
        if (energyBlast)
        {
            tutorials.Add(1);
        }
        if (fireball)
        {
            tutorials.Add(2);
        }
        if (lightning)
        {
            tutorials.Add(3);
        }
        if (barrier)
        {
            tutorials.Add(4);
        }
        if (teleport)
        {
            tutorials.Add(5);
        }
        if (crystalTower)
        {
            tutorials.Add(6);
        }
        if (flameOrbTower)
        {
            tutorials.Add(7);
        }
        if (psychicTower)
        {
            tutorials.Add(8);
        }
        
        if (tutorials.Count == 0) 
        {
            End();
        }
        else
        {
            NextTutorial();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void End()
    {
        switch(TutorialSetter.currentTutorial)
        {
            case 0:
                SaveLoad.level1TutorialDone = true;
                break;
            case 1:
                SaveLoad.level2TutorialDone = true;
                break;
            case 2:
                SaveLoad.level3TutorialDone = true;
                break;
        }
        endScreen.SetActive(true);
    }

    public void NextTutorial()
    {
        if (curTut >= 0)
        {
            tutObjs[curTut].SetActive(false);
        }

        CreateTower[] towers = FindObjectsOfType<CreateTower>();
        foreach (CreateTower t in towers)
        {
            if (t && t.hasTower())
            {            
                Destroy(t.gameObject);
            }
        }

        counter++;
        if (counter == tutorials.Count)
        {
            End();
            return;
        }

        curTut = tutorials[counter];

        tutObjs[curTut].SetActive(true);

        //switch (curTut)
        //{
        //    case 0:
        //        towerPlacing = false; 
        //        break;
        //    case 1:
        //        energyBlast = false; 
        //        break;
        //    case 2:
        //        fireball = false; 
        //        break;
        //    case 3:
        //        lightning = false; 
        //        break;
        //    case 4:
        //        barrier = false; 
        //        break;
        //    case 5:
        //        teleport = false;
        //        break;
        //    case 6:
        //        crystalTower = false; 
        //        break;
        //    case 7:
        //        flameOrbTower = false; 
        //        break;
        //}
    }

    //public void Replay()
    //{
    //    foreach (int tut in tutorials)
    //    {
    //        switch (tut)
    //        {
    //            case 0:
    //                towerPlacing = true; break;
    //            case 1:
    //                energyBlast = true; break;
    //            case 2:
    //                fireball = true; break;
    //            case 3:
    //                lightning = true; break;
    //            case 4:
    //                barrier = true; break;
    //            case 5:
    //                teleport = true; break;
    //            case 6:
    //                crystalTower = true; break;
    //            case 7:
    //                flameOrbTower = true; break;
    //        }
    //    }
    //}
}
