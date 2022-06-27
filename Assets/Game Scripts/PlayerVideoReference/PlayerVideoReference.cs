using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerVideoReference : MonoBehaviourPunCallbacks
{
    [SerializeField]
    PhotonView view;
    public Transform Videos;
    public RawImage Raw_Image;

    private void Start()
    {
        UpdateVideoOnOffView();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("**** ViewID: " + gameObject.GetComponent<PhotonView>().ViewID + " ****");
            Debug.Log("Videos: childs = (" + Videos.childCount + ") active = " + Videos.gameObject.activeInHierarchy);
            Debug.Log("Raw_Image: childs = (" + Raw_Image.transform.childCount + ")active = " + Raw_Image.gameObject.activeInHierarchy);
        }
    }

    public void setVideoEnable(bool val)
    {
        Videos.gameObject.SetActive(val);
        Raw_Image.gameObject.SetActive(val);
    }

    void UpdateVideoOnOffView()
    {
        if (PhotonHelper.GetPlayerCustomProperty<int>(view, PropertiesData.VideoOnOff, 0) == 1)
        {
            setVideoEnable(true);
        }
        else
        {
            setVideoEnable(false);
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (changedProps.ContainsKey(PropertiesData.VideoOnOff))
        {
            if (targetPlayer == view.Owner)
            {
                UpdateVideoOnOffView();
            }
        }

    }






}
