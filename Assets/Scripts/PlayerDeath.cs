using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerDeath : MonoBehaviour
{
    public WaveManager manager;
    public float minKillInterval;
    public float maxKillInterval;
    public float rampTime;

    public GameObject directionalLight;
    private Light skyLight;

    public UIScript mainUI;
    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;

    public AudioSource loseAudio;

    //public static List<GameObject> towers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        skyLight = directionalLight.GetComponent<Light>();
        WaveManager.LevelEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        loseAudio.Play();

        //foreach (GameObject tower in towers)
        //{
        //    Destroy(tower);
        //}

        skyLight.color = Color.red;

        for (int i = manager.enemies.Count - 1; i >= 0; i--)
        {
            if (!manager.enemies[i])
            {
                manager.enemies.RemoveAt(i);
            }
        }

        manager.enemies.Sort();

        float curRamp = 0;
        for (int i = manager.enemies.Count - 1; i >= 0; i--)
        {
            if (manager.enemies[i])
            {
                curRamp += Mathf.Lerp(minKillInterval, maxKillInterval, curRamp / rampTime);
                //print(curRamp + " " + manager.enemies[i].transform.name);
                manager.enemies[i].attkRange = 1000;
                Destroy(manager.enemies[i].gameObject, curRamp);
                Destroy(manager.enemies[i].GetComponent<NavMeshAgent>());
                Destroy(manager.enemies[i]);
            }
        }

        StartCoroutine(ShowScreen(curRamp + minKillInterval * 2));

        if (manager)
        {
            manager.gameObject.SetActive(false);
        }

        //Time.timeScale = 0;
    }

    IEnumerator ShowScreen(float delay)
    {
        yield return new WaitForSeconds(delay);
        // SHOW LOSS SCREEN
        WaveManager.LevelEnd = true;
        mainUI.SetScreen("Lose");
        HandRay.activeHandRays = true;
        leftRayInteractor.SetActive(true);
        rightRayInteractor.SetActive(true);

        //Hopefully stop late enemies
        WaveManager.Instance.StopAllCoroutines();
    }
}
