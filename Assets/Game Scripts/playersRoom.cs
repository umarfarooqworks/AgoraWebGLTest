using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class playersRoom : MonoBehaviourPunCallbacks
{
    public string _roomName;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=> RoomClicked());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RoomClicked()
    {
       PhotonController._instance._allroomspanel.SetActive(false);
        PhotonController._instance._loadingPanel.SetActive(true);
        PhotonNetwork.JoinRoom(_roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Can't Join Room right now");
    }

    public override void OnJoinedRoom()
    {
      
    }
}
