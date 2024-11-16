using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class UIScript : MonoBehaviour
{
    public Renderer fade;
    public float fadeTime;
    public float padding;

    Color fadeColor;

    bool paused = false;
    //public GameObject pauseScreen;
    public GameObject barrierSphere;


    public bool pauseable = true;
    public InputActionProperty pauseButton;

    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;

    [System.Serializable]
    public class Screen
    {
        public string name;
        public GameObject obj;

        public Screen(string aName, GameObject aObj)
        {
            name = aName;
            obj = aObj;
        }
    }

    public GameObject back;

    [SerializeField] public Screen[] screenArr;

    string currentScreen = "";
    string previousScreen = "";
    [SerializeField]
    Dictionary<string, GameObject> screens = new Dictionary<string, GameObject>();


    public AudioSource UIClick;
    public AudioSource startSound;
    public AudioSource quitSound;

    private void Awake()
    {
        fadeColor = fade.material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade(1, 0));

        foreach (Screen screen in screenArr)
        {
            screens.Add(screen.name, screen.obj);
            if (screen.obj.activeSelf)
            {
                currentScreen = screen.name;
            }
        }
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
        UIClick.Play();

        activate.SetActive(true);
    }

    public void SetScreen(string name)
    {
        //Call play sound externally

        previousScreen = currentScreen;
        if (currentScreen != "")
        {
            screens[currentScreen].SetActive(false);
        }
        if (screens.ContainsKey(name))
        {
            screens[name].SetActive(true);
            currentScreen = name;

            back.SetActive(true);
        }
        else
        {
            currentScreen = "";
            back.SetActive(false);
        }
    }

    public void Back()
    {
        UIClick.Play();

        if (currentScreen != "")
        {
            screens[currentScreen].SetActive(false);
        }
        if (screens.ContainsKey(previousScreen))
        {
            screens[previousScreen].SetActive(true);
            string temp = currentScreen;
            currentScreen = previousScreen;
            previousScreen = temp;

            back.SetActive(true);
        }
        else
        {
            currentScreen = "";
            back.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (WaveManager.LevelEnd)
        {
            return;
        }

        UIClick.Play();

        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            //pauseScreen.SetActive(true);
            SetScreen("Pause");
            if (barrierSphere)
            {
                barrierSphere.SetActive(false);
            }

            HandRay.activeHandRays = true;
            leftRayInteractor.SetActive(true);
            rightRayInteractor.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            //pauseScreen.SetActive(false);
            SetScreen("");
            if (barrierSphere)
            {
                barrierSphere.SetActive(true);
            }

            HandRay.activeHandRays = false;
            leftRayInteractor.SetActive(false);
            rightRayInteractor.SetActive(false);
        }
    }

    public void LoadScene(string scene)
    {
        startSound.Play();

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
        quitSound.Play();

        StartCoroutine(Fade(0, 1));
        StartCoroutine(EndApplication());
    }

    public IEnumerator EndApplication()
    {
        yield return new WaitForSeconds(fadeTime);

        Application.Quit();
    }
}
