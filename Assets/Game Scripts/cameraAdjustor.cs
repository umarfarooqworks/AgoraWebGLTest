using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class cameraAdjustor : MonoBehaviourPunCallbacks
{
    public GameObject camera;
    void Start()
    {


        if (photonView.IsMine)
        {
            
            camera.SetActive(true);
            camera.transform.parent = null;
        }
        else
        {
            camera.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
