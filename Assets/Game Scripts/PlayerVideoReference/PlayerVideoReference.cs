using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVideoReference : MonoBehaviour
{
    public Transform Videos;
    public RawImage Raw_Image;

    private void Update()
    {
        if(Input.GetKey(KeyCode.P))
        {
            Debug.Log("**** ViewID: " + gameObject.GetComponent<PhotonView>().ViewID + " ****");
            Debug.Log("Videos: childs = ("+Videos.childCount + ") active = " + Videos.gameObject.activeInHierarchy);
            Debug.Log("Raw_Image: childs = (" + Raw_Image.transform.childCount + ")active = " + Raw_Image.gameObject.activeInHierarchy);
        }
    }

}
