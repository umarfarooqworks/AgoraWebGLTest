using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayController : MonoBehaviourPunCallbacks
{
    public Transform content;
    public PlayerDisplayDetails PlayerDisplayDetails;

    public List<PlayerDisplayDetails> AllPlayersList;

    public static PlayerDisplayController instance;

    private void Awake()
    {
        instance = this;        
    }

    public void OnNewPlayerJoined(PhotonView view)
    {
        if (view.Owner.IsLocal)
            return;

        PlayerDisplayDetails temp = GameObject.Instantiate(PlayerDisplayDetails, content);

        AllPlayersList.Add(temp);
        temp.init(view);
    }

    public void OnClickStopFollowing()
    {
        PhotonController._instance.MakeLocalPlayerUnFollow();
    }

    public void ResetList()
    {
        Invoke(nameof(DelayedResetList), 0.1f);
    }

    private void DelayedResetList()
    {
        Debug.Log("ResetList");
        for (int i = AllPlayersList.Count - 1; i > -1; i--)
        {
            Debug.Log("Checking for :" + i);
            if (AllPlayersList[i].referencePlayer == null)
            {
                PlayerDisplayDetails Todestroy = AllPlayersList[i];
                Debug.Log("Destroy Element at :" + i);
                AllPlayersList.RemoveAt(i);
                Destroy(Todestroy.gameObject);
            }
            else
            {
                Debug.Log("NOT NULL at " + i);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        ResetList();
    }
}
