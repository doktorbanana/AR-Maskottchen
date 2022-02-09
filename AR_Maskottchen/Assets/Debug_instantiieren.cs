using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_instantiieren : MonoBehaviour
{
    [SerializeField]
    GameObject mascot_prefab;
    
    public void MascotInstantiate(){
        Instantiate(mascot_prefab);
    }
}
