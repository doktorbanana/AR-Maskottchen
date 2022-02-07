using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons_wiggle : MonoBehaviour
{

    private Animator anim;
    
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(Start_Wiggle());
    }

    IEnumerator Start_Wiggle() {

        yield return new WaitForSeconds(10);

        anim.SetTrigger("wiggleTime");


        StartCoroutine(Start_Wiggle());
        yield return null;
    }
}
