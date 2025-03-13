using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kroland.fantasymonsters
{
    public class SimpleAnimationPlayer : MonoBehaviour
    {
        private Animator anim;
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                nextAnim();
            }
        }

        public void nextAnim()
        {
            anim.SetTrigger("Next");
        }
    }
}