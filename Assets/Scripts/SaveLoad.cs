using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public static bool level1TutorialDone = false;
    public static bool level2TutorialDone = false;
    public static bool level3TutorialDone = false;

    public static bool level1Done = false;
    public static bool level2Done = false;
    public static bool level3Done = false;

    public static bool lefty = false;

    public static bool snapTurn = true;
    public static int snapAmount = 45;

    public static float masterVolume = 1f;
    public static float musicVolume = 2f;
    public static float sounfFXVolume = 2f;

    public static int priority = 2;

    public static SaveLoad Instance;

    public void Save(ref SaveData data)
    {
        data.l1Tut = level1TutorialDone;
        data.l2Tut = level2TutorialDone;
        data.l3Tut = level3TutorialDone;

        data.l1 = level1Done;
        data.l2 = level2Done;
        data.l3 = level3Done;

        data.lefty = lefty;

        data.snap = snapTurn;
        data.amount = snapAmount;

        data.master = masterVolume;
        data.fx = sounfFXVolume;
        data.music = musicVolume;

        data.priority = priority;
    }

    public void Load(SaveData data)
    {
        level1TutorialDone = data.l1Tut;
        level2TutorialDone = data.l2Tut;
        level3TutorialDone = data.l3Tut;

        level1Done = data.l1;
        level2Done = data.l2;
        level3Done = data.l3;

        lefty = data.lefty;

        snapTurn = data.snap;
        snapAmount = data.amount;

        masterVolume = data.master;
        sounfFXVolume = data.fx;
        musicVolume = data.music;

        priority = data.priority;
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;

            try
            {
                SaveSystem.Load();
                print("Loaded data");
            }
            catch (FileNotFoundException)
            {
                print("Save file not found. New Save Created");
#if UNITY_STANDALONE_WIN
                priority = 2;
#elif UNITY_ANDROID
                priority = 1;
#endif

                SaveSystem.Save();
            }

            DontDestroyOnLoad(this);
        }
        else
        {
            SaveSystem.Save();

            Destroy(this);
        }
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
        print("Saved data");
    }
}
