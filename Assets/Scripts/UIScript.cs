using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class UIScript : MonoBehaviour
{
    public Renderer fade;
    public float fadeTime;
    public float padding;

    Color fadeColor;

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
        
    }

    IEnumerator Fade(float a, float b)
    {
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
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(Fade(0, 1));
        StartCoroutine(Load(scene));
    }

    public IEnumerator Load(string scene)
    {
        yield return new WaitForSeconds(fadeTime + padding);

        Time.timeScale = 1f;
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
