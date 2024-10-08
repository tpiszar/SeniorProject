using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIScript : MonoBehaviour
{
    public Renderer fade;
    public float fadeTime;
    public float padding;

    Color fadeColor;

    bool paused = false;
    public GameObject pauseScreen;
    public GameObject barrierSphere;


    public bool pauseable = true;
    public InputActionProperty pauseButton;

    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;

    private void Awake()
    {
        fadeColor = fade.material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade(1, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseable && pauseButton.action.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    IEnumerator Fade(float a, float b)
    {
        leftRayInteractor.SetActive(false);
        rightRayInteractor.SetActive(false);

        if (a > b)
        {
            yield return new WaitForSeconds(padding);
        }

        float timer = 0;
        float maxTime = fadeTime;
        
        if (a < b)
        {
            maxTime += padding;
        }

        while (timer <= maxTime)
        {
            timer += Time.deltaTime;

            fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, Mathf.Lerp(a, b, timer / fadeTime));
            fade.material.color = fadeColor;

            yield return null;
        }

        leftRayInteractor.SetActive(true);
        rightRayInteractor.SetActive(true);
    }

    public void Previous(GameObject deactivate)
    {
        deactivate.SetActive(false);
    }

    public void Next(GameObject activate)
    {
        activate.SetActive(true);
    }

    public void TogglePause()
    {
        if (WaveManager.LevelEnd)
        {
            return;
        }


        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            barrierSphere.SetActive(false);

            HandRay.activeHandRays = true;
            leftRayInteractor.SetActive(true);
            rightRayInteractor.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            barrierSphere.SetActive(true);

            HandRay.activeHandRays = false;
            leftRayInteractor.SetActive(false);
            rightRayInteractor.SetActive(false);
        }
    }

    public void LoadScene(string scene)
    {
        if (paused)
        {
            TogglePause();
        }
        StartCoroutine(Fade(0, 1));
        StartCoroutine(Load(scene));
    }

    public IEnumerator Load(string scene)
    {
        yield return new WaitForSeconds(fadeTime + padding);

        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        StartCoroutine(Fade(0, 1));
        StartCoroutine(EndApplication());
    }

    public IEnumerator EndApplication()
    {
        yield return new WaitForSeconds(fadeTime);

        Application.Quit();
    }
}
