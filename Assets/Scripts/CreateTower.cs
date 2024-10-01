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
    public int refundCost;


    //List<MeshRenderer> meshes = new List<MeshRenderer>();
    //public Material invisbleMat;
    //Material baseMat;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Grabbed);
        grabbable.selectExited.AddListener(Dropped);

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

    bool socketGrab = true;

    public void Grabbed(SelectEnterEventArgs args)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        holderTrans = args.interactor.transform;
#pragma warning restore CS0618 // Type or member is obsolete

        if (tower)
        {
            if (!socketGrab)
            {
                Mana.Instance.PreGaining(refundCost);
            }
            socketGrab = !socketGrab;
        }
        else
        {
            Mana.Instance.PreUsing(manaCost);
        }
    }

    bool socketDrop = true;

    public void Dropped(SelectExitEventArgs args)
    {
        if (tower)
        {
            if (!socketDrop)
            {
                Mana.Instance.PreGaining(refundCost * -1);
            }
            socketDrop = !socketDrop;
        }
        else
        {
            Mana.Instance.PreUsing(manaCost * -1);
        }
    }

    public void Create()
    {
        //Vector3 groundedPoint = socket.position;
        //groundedPoint.y -= attachPoint.localPosition.y * transform.localScale.y;
        //Vector3 relativeSpot = miniReference.InverseTransformPoint(groundedPoint) * mapScale;


        //Vector3 rotation = socket.GetChild(0).rotation.eulerAngles;
        //rotation.x = 0; 
        //rotation.z = 0;

        

        if (Mana.Instance.UseMana(manaCost))
        {
            Vector3 relativeSpot = miniReference.InverseTransformPoint(createPoint) * mapScale;
            Quaternion relativeRot = Quaternion.Inverse(Quaternion.Euler(createRot)) * miniReference.rotation;

            tower = Instantiate(towerPrefab, relativeSpot, relativeRot);
            //tower.GetComponent<BasicTowerDetection>().player = player;
            transform.parent = miniReference;
            socket.parent = miniReference;

            //PlayerDeath.towers.Add(tower);
        }
    }

    private void OnDestroy()
    {
        if (tower)
        {
            //PlayerDeath.towers.Remove(tower);
            Mana.Instance.GainMana(refundCost);
            Destroy(tower);
            //Destroy(socket.gameObject);
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
