using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateTower : MonoBehaviour
{
    public Transform socket;
    public Transform attachPoint;

    public Transform miniReference;
    public float mapScale;

    public GameObject towerPrefab;

    GameObject tower;
    public Transform player;

    public float overlap = 0;

    public Transform holderTrans;

    public Vector3 createPoint = Vector3.zero;
    public Vector3 createRot = Vector3.zero;

    public int manaCost;

    //List<MeshRenderer> meshes = new List<MeshRenderer>();
    //public Material invisbleMat;
    //Material baseMat;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Grabbed);


        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    MeshRenderer mesh = transform.GetChild(i).GetComponent<MeshRenderer>();
        //    if (mesh)
        //    {
        //        meshes.Add(mesh);
        //        if (!baseMat)
        //        {
        //            baseMat = mesh.material;
        //        }
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Grabbed(SelectEnterEventArgs args)
    {
        holderTrans = args.interactor.transform;
    }

    public void Create()
    {
        //Vector3 groundedPoint = socket.position;
        //groundedPoint.y -= attachPoint.localPosition.y * transform.localScale.y;
        //Vector3 relativeSpot = miniReference.InverseTransformPoint(groundedPoint) * mapScale;


        //Vector3 rotation = socket.GetChild(0).rotation.eulerAngles;
        //rotation.x = 0; 
        //rotation.z = 0;

        

        if (Mana.Instance.useMana(manaCost))
        {
            Vector3 relativeSpot = miniReference.InverseTransformPoint(createPoint) * mapScale;
            Quaternion relativeRot = Quaternion.Inverse(Quaternion.Euler(createRot)) * miniReference.rotation;

            tower = Instantiate(towerPrefab, relativeSpot, relativeRot);
            tower.GetComponent<BasicTowerDetection>().player = player;
            transform.parent = miniReference;
            socket.parent = miniReference;
        }
    }

    private void OnDestroy()
    {
        if (tower)
        {
            Destroy(tower);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CreateTower tower = other.GetComponentInParent<CreateTower>();
        if (tower)// && other.transform != transform)
        {
            overlap++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CreateTower tower = other.GetComponentInParent<CreateTower>();
        if (tower)// && other.transform != transform)
        {
            overlap--;
        }
    }

    //bool visible = true;
    //public void ToggleVisible()
    //{
    //    foreach (MeshRenderer mesh in meshes)
    //    {
    //        if (visible)
    //        {
    //            mesh.material = invisbleMat;
    //            visible = false;
    //        }
    //        else
    //        {
    //            mesh.material = baseMat;
    //            visible = true;
    //        }
    //    }
    //}
}
