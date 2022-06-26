using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using agora_gaming_rtc;

public class PhotonController : MonoBehaviourPunCallbacks 
{
    private string _roomName = "WebGlMetaverseTest";
    public byte _maxPLayers = 10;

    public GameObject _MenuPanel;
    public GameObject _InGamePanel;



    public GameObject _playerPrefab;
    public GameObject _playerFemalePrefab;


    public GameObject _PlayPanel;
    public GameObject _loadingPanel;
    public GameObject _roomPanel;
    public GameObject _namePanel;
    public InputField _PlayerNick;
    public ScrollRect _playerlist;
    public GameObject _allroomspanel;

    [Header("AvailablePlayers")]
    public List<PhotonView> Players;


    int SelectedGener = PropertiesData.MaleCode;
    public GameObject MaleButton;
    public GameObject FemaleButton;

    public static PhotonController _instance;
    private void Awake()
    {
       if (_instance == null)
        {
            {
                _instance = this;
            }
            if(_instance!=this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int i = 0;
            foreach (PhotonView x in Players)
                Debug.Log(i + " : with view" + x.ViewID);
            Debug.Log("Players: " + Players.Count);

        }
    }

    private void Start()
    {
        _MenuPanel.SetActive(true);
        _InGamePanel.SetActive(false);
    }

    public void _Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        _PlayPanel.SetActive(false);
        _loadingPanel.SetActive(true);
    }    

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        _loadingPanel.SetActive(false);
        _namePanel.SetActive(true);

    }
    public void CreateJoinRoom()
    {

        if (! string.IsNullOrEmpty( _PlayerNick.text))
        {
            RoomOptions _roomoptions = new RoomOptions();
            _roomoptions.MaxPlayers = _maxPLayers;
            _roomoptions.IsVisible = true;
            _roomoptions.IsOpen = true;
            PhotonNetwork.JoinRandomRoom();

            _namePanel.SetActive(false);
            _loadingPanel.SetActive(true);
        }
    }
    public GameObject _VoiceCOntrols;
    public GameObject _LocalPlayer;

    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = _PlayerNick.text;

        _loadingPanel.SetActive(false);

        if(SelectedGener == PropertiesData.MaleCode)
        {
            _LocalPlayer = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
        }
        else
        {
            _LocalPlayer = PhotonNetwork.Instantiate(_playerFemalePrefab.name, Vector3.zero, Quaternion.identity);
        }


        //  _VoiceCOntrols.SetActive(true);
        int _viewid = _LocalPlayer.GetComponent<PhotonView>().ViewID;
        print(_viewid + "  my view id");

        _InGamePanel.SetActive(true);

        AgoraVideoSetup.instace.StartAgora((uint) _viewid);

//        OnClickMaleButton();
        PhotonHelper.SetPlayerCustomProperty(PhotonNetwork.LocalPlayer, PropertiesData.Gender, SelectedGener);

    }





    public void StartGame()
    {
        if (!string.IsNullOrEmpty(_PlayerNick.text))
        {
            PhotonNetwork.NickName = _PlayerNick.text;
            _namePanel.SetActive(false);
            _roomPanel.SetActive(true);
        }
    }
    public GameObject _roomPrefab;
    public GameObject _roomList;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

       

       foreach(RoomInfo _r in roomList)
        {
            GameObject _gb = Instantiate(_roomPrefab, _roomList.transform);
            _gb.GetComponent<playersRoom>()._roomName = _r.Name;
            _gb.transform.GetChild(0).GetComponent<Text>().text = _r.Name;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Joining new room failed");
        RoomOptions _roomoptions = new RoomOptions();
        _roomoptions.MaxPlayers = _maxPLayers;
        _roomoptions.IsVisible = true;
        _roomoptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(_roomName, _roomoptions, TypedLobby.Default);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected for server due to " + cause);
        AgoraVideoSetup.instace.LeaveAgora();
    }
    public void _allrooms()
    {
        if (!string.IsNullOrEmpty(_PlayerNick.text))
        {
            _namePanel.SetActive(false);
            _allroomspanel.SetActive(true);
        }
    }
    public InputField _PrivateRoomIF;
    public void createprivateroom()
    {
        if (!string.IsNullOrEmpty(_PrivateRoomIF.text))
        {

            RoomOptions _roomoptions = new RoomOptions();
            _roomoptions.MaxPlayers = 5;
            _roomoptions.IsVisible = true;
            _roomoptions.IsOpen = true;

            PhotonNetwork.JoinOrCreateRoom(_PrivateRoomIF.text, _roomoptions, TypedLobby.Default);

            _allroomspanel.SetActive(false);
            _loadingPanel.SetActive(true);
        }
    }

    public void OnClickMaleButton()
    {
        MaleButton.GetComponent<Image>().color = Color.red;
        FemaleButton.GetComponent<Image>().color = Color.white;
        SelectedGener = PropertiesData.MaleCode;
//        PhotonHelper.SetPlayerCustomProperty(PhotonNetwork.LocalPlayer, PropertiesData.Gender, PropertiesData.MaleCode);
    }
    public void OnClickFeMaleButton()
    {
        MaleButton.GetComponent<Image>().color = Color.white;
        FemaleButton.GetComponent<Image>().color = Color.red;
        SelectedGener = PropertiesData.FeMaleCode;
//        PhotonHelper.SetPlayerCustomProperty(PhotonNetwork.LocalPlayer, PropertiesData.Gender, PropertiesData.FeMaleCode);
    }

    public void MakeLocalPlayerFollow(PhotonView __view)
    {
        _LocalPlayer.GetComponent<PlayerAIFollowHelper>().StartFollowing(__view);
    }
    public void MakeLocalPlayerUnFollow()
    {
        _LocalPlayer.GetComponent<PlayerAIFollowHelper>().StopFollowing();
    }

    public VideoSurface GetPlayerReferenceWithAgoraVideoSurface(int View_id)
    {
        foreach(PhotonView x in Players)
        {
            if(x.ViewID == View_id)
            {
                return x.gameObject.GetComponentInChildren<VideoSurface>();
            }
        }
        return null;
    }
}
