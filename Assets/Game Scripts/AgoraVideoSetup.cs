using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RawImageWithIDs
{
    public RawImageWithIDs(int id, RawImage raw)
    {
        ID = id;
        rawImage = raw;
    }
    public int ID;
    public RawImage rawImage;
}

public class AgoraVideoSetup : MonoBehaviour
{
    public static AgoraVideoSetup instace;

    public enum ChannelActions
    {
        JOIN,
        LEAVE
    }

    [SerializeField]
    string appid = "025d672b502942f8ad01e793885edabb";
    [SerializeField]
    string channelName;
    [SerializeField]
    string yourToken;

    private ArrayList permissionList = new ArrayList();

    public GameObject StartVideoButton;
    public GameObject StopVideoButton;
    void Awake()
    {
        instace = this;




        StartVideoButton.GetComponent<Button>().onClick.AddListener(StartVideo);
        StopVideoButton.GetComponent<Button>().onClick.AddListener(StopVideo);

        //joinChannelButtonText = joinChannelButton.GetComponentInChildren<TextMeshProUGUI>();
        //JoinChannelButtonImage = joinChannelButton.GetComponent<Image>();
        //        JoinChannelButtonImage.color = Color.green;

        //        permissionList.Add(Permission);
    }



    void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Invoke(nameof(EnableUIButtons), 1f);
    }
    void EnableUIButtons()
    {
        SetUIButtons();
    }


    bool videoStopped = true;
    private void Update()
    {
     
    }



    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        StartVideoButton.GetComponent<Button>().onClick.RemoveListener(StartVideo);
        StopVideoButton.GetComponent<Button>().onClick.RemoveListener(StopVideo);
        AgoraController.instance.mRtcEngine.OnJoinChannelSuccess -= onJoinChannelSuccess;
    }

    public uint GetAgoraUserID() => AgoraController.instance.LocalUserID;

    public void StartAgora(uint _viewid)
    {
        AgoraController.instance.loadEngine(appid, yourToken);
        AgoraController.instance.mRtcEngine.RegisterLocalUserAccount(appid, _viewid);
        AgoraController.instance.join(channelName, true, _viewid);
        AgoraController.instance.mRtcEngine.OnJoinChannelSuccess += onJoinChannelSuccess;
    }

    public void LeaveAgora()
    {
        AgoraController.instance.leave();
        AgoraController.instance.unloadEngine();

        //CHange button texts
    }

    public GameObject GetPlayerVideosChild(string viewID)
    {
        foreach(PhotonView x in PhotonController._instance.Players)
        {
            if(string.Equals(x.ViewID.ToString(), viewID))
            {
                return x.GetComponent<PlayerVideoReference>().Videos.gameObject;
            }
        }
        return null;
    }

    public RawImage GetPlayerVideosRawImage(string viewID)
    {
        foreach (PhotonView x in PhotonController._instance.Players)
        {
            if (string.Equals(x.ViewID.ToString(), viewID))
            {
                return x.GetComponent<PlayerVideoReference>().Raw_Image;
            }
        }
        return null;
    }


    public void StartVideo()
    {
        Debug.Log("StartVideo");
//        AgoraController.instance.mRtcEngine.MuteLocalVideoStream(false);
        AgoraController.instance.EnableVideo(true);
        AgoraController.instance.mRtcEngine.EnableLocalVideo(true);
        //AgoraController.instance.DisplayLocalCameraOutputOnTopOfUser();
        videoStopped = false;
        SetUIButtons();
    }
    public void StopVideo()
    {
        Debug.Log("StopVideo");
//        AgoraController.instance.mRtcEngine.MuteLocalVideoStream(true);
        AgoraController.instance.EnableVideo(false);
        AgoraController.instance.mRtcEngine.EnableLocalVideo(false);
//        AgoraController.instance.HideLocalCameraOutputOnTopOfUser();
        videoStopped = true;
        SetUIButtons();
    }

    void SetUIButtons()
    {
        StartVideoButton.SetActive(videoStopped);
        StopVideoButton.SetActive(!videoStopped);
    }
}
