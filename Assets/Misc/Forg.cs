using UnityEngine;
using UnityEngine.SceneManagement;

public class Forg : MonoBehaviour
{
    public Frogify omniFrog;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shot"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (endScene) return;

        omniFrog.Activate();
    }

    bool endScene = false;
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        endScene = true;
    }
}
