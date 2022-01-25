using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurnOff : MonoBehaviour
{
    [SerializeField]
    int activeTime;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(TurnOff());
    }

    IEnumerator TurnOff(){
        yield return new WaitForSeconds(activeTime);
        gameObject.SetActive(false);
    }
}
