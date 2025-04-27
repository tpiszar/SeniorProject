using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSwitcher : MonoBehaviour
{
    public static WallSwitcher Instance;

    [SerializeField]
    private MeshRenderer rend;

    [SerializeField]
    private Material fadeMat;
    [SerializeField]
    private Material solidMat;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void SetFade()
    {
        rend.material = fadeMat;
    }

    public void SetSolid()
    {
        rend.material = solidMat;
    }
}
