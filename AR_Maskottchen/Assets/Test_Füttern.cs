using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maskottchen.Manager;

public class Test_Füttern : MonoBehaviour
{
    [SerializeField]
    Maskottchen_Manager maskottchenManager;
    public void Click(){
        maskottchenManager.Feed();
    } 
}
