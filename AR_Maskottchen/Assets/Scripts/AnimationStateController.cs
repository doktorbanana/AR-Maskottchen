using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private readonly string[] animations = { "Pickup", "Wave", "Sleep", "Catch", "Laugh" };
    private int counter = 0;
    private bool isIdle = true;
    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (isIdle)
                {
                    if (counter >= animations.Length)
                    {
                        counter = 0;
                    }
                    mAnimator.SetTrigger(animations[counter]);
                    Debug.Log(animations[counter]);
                    counter++;
                    isIdle = false;
                } else {
                    mAnimator.SetTrigger("Idle");
                    isIdle = true;
                }

            }
        }
    }
}
