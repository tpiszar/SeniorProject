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

    //public int overlap = 0;
    public BoxCollider overlapObj;

    public Transform holderTrans;

    public Vector3 createPoint = Vector3.zero;
    public Vector3 createRot = Vector3.zero;

    public int manaCost;
    public int refundCost;
    
    public CurrentHand hand;

    public AudioSource pickupSound;
    public AudioSource createSound;
    public AudioClip breakSound;
    [Range(0.0001f, 1f)]
    public float breakVolume = 1;

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

    public void Grabbed(SelectEnterEventArgs args)
    {
        holderTrans = hand.movementSource;
        
        if (!hand.noHand)
        {
            pickupSound.Play();
        }


        if (tower)
        {
            if (!hand.noHand)
            {
                Mana.Instance.PreGaining(refundCost);
            }
        }
        else
        {
            Mana.Instance.PreUsing(manaCost);
        }
    }

    public void Dropped(SelectExitEventArgs args)
    {
        if (tower)
        {
            if (!hand.noHand)
            {
                Mana.Instance.PreGaining(refundCost * -1);
            }
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
            Quaternion relativeRot = Quaternion.Euler(createRot) * miniReference.rotation;

            tower = Instantiate(towerPrefab, relativeSpot, relativeRot);
            //tower.GetComponent<BasicTowerDetection>().player = player;
            transform.parent = miniReference;
            socket.parent = miniReference;

            //PlayerDeath.towers.Add(tower);

            createSound.Play();
        }
    }

    public bool hasTower()
    {
        return tower;
    }

    private void OnDestroy()
    {
        if (tower)
        {
            //PlayerDeath.towers.Remove(tower);
            Mana.Instance.GainMana(refundCost);
            Destroy(tower);
            //Destroy(socket.gameObject);

            SoundManager.instance.PlayClip(breakSound, transform.position, breakVolume);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //CreateTower tower = other.GetComponentInParent<CreateTower>();
    //    //if (tower)// && other.transform != transform)
    //    //{
    //    //    overlap++;
    //    //}

    //    if (other.CompareTag("Blocker"))
    //    {
    //        overlap++;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    //CreateTower tower = other.GetComponentInParent<CreateTower>();
    //    //if (tower)// && other.transform != transform)
    //    //{
    //    //    overlap--;
    //    //}

    //    if (other.CompareTag("Blocker"))
    //    {
    //        overlap--;
    //    }
    //}

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
