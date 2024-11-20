using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookButton : MonoBehaviour
{
    bool selected = false;

    public Book book;
    public int pageNum;

    MeshRenderer mesh;
    public Material selectedMat;
    Material baseMat;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        baseMat = mesh.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pointer"))
        {
            mesh.material = selectedMat;
            selected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (selected && other.CompareTag("Pointer"))
        {
            mesh.material = baseMat;
            selected = false;

            book.Turn(pageNum);
        }
    }
}
