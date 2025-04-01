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

    //public LayerMask handLayer;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        baseMat = mesh.material;
    }

    // Update is called once per frame
    void Update()
    {
        //Physics.SyncTransforms();

        //Collider[] colliders = Physics.OverlapBox(transform.position, transform.lossyScale / 2, transform.rotation, handLayer);

        //if (selected && colliders.Length == 0)
        //{
        //    Exit();

        //    return;
        //}

        //foreach (Collider col in colliders)
        //{
        //    if (col.gameObject.CompareTag("Pointer"))
        //    {
        //        if (!selected && colliders.Length > 0)
        //        {
        //            Enter();
        //        }

        //        return;
        //    }
        //}

        //if (selected)
        //{
        //    Exit();
        //}
    }

    void Enter()
    {
        mesh.material = selectedMat;
        selected = true;
    }

    void Exit()
    {
        mesh.material = baseMat;
        selected = false;

        book.Turn(pageNum);
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

    private void OnDisable()
    {
        if (UIScript.sceneLoading) { return; }

        selected = false;
        mesh.material = baseMat;
    }
}
