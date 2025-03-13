using UnityEngine;

namespace kroland.fantasymonsters
{
    public class WraithEvent : MonoBehaviour
    {
        public GameObject fingerPoint;
        public GameObject magicObj;
        public float magicSpeed = 10;
        public void radiateMagic(){
            GameObject go = GameObject.Instantiate(magicObj,fingerPoint.transform.position,this.gameObject.transform.rotation);
            go.transform.SetParent(this.gameObject.transform);
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward * magicSpeed,ForceMode.Impulse);
        }
    }
}