using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.Universal;

public class Frogify : MonoBehaviour
{
    public Material TheUndyingOne;

    public GameObject fade;
    public GameObject vignette;

    bool active = false;

    private HashSet<Renderer> froggedRends = new HashSet<Renderer>();

    public AudioClip forestMusic;
    public AudioSource music;

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Renderer[] allRends = FindObjectsOfType<Renderer>();

            foreach (Renderer r in allRends)
            {
                if (!froggedRends.Contains(r))
                {
                    froggedRends.Add(r);

                    Infect(r);
                }
            }
        }
    }

    public void Activate()
    {
        active = true;

        music.Stop();
        music.clip = forestMusic;
        music.volume = 0.3f;
        music.Play();

        if (!Camera.main) { return; }

        Camera.main.backgroundColor = Color.black;

        Destroy(fade);
        Destroy(vignette);
    }

    private void Infect(Renderer rend)
    {
        Material[] mats = rend.materials;

        for (int i = 0; i < rend.materials.Length; i++)
        {
            mats[i] = TheUndyingOne;
        }

        rend.materials = mats;
    }
}
