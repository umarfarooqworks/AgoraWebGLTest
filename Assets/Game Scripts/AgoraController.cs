using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using agora_utilities;
using UnityEngine.UI;
using TMPro;

public class AgoraUserData
{
    public uint userID;
    public bool VideoEnabled = false;
    public bool AudioEnabled = false;
    public VideoSurface videoSurface;

    public AgoraUserData(uint userID, bool VideoEnabled, bool AudioEnabled)
    {
        this.userID = userID;
        this.VideoEnabled = VideoEnabled;
        this.AudioEnabled = AudioEnabled;
    }

    public void SetVideoSurface(VideoSurface videoSurface)
    {
        this.videoSurface = videoSurface;
    }
    public VideoSurface GetVideoSurface(VideoSurface videoSurface)
    {
        return this.videoSurface;
    }
    public void VideoSurfaceSetEnable(bool b)
    {
        //videoSurface.SetEnable(b);
    }
}

public class AgoraUserDataHandler
{
    List<AgoraUserData> agoraUserData = new List<AgoraUserData>();

    public void AddNewUser(AgoraUserData userData)
    {
        agoraUserData.Add(userData);
    }

    public void AssignVideoSurface(uint userID, VideoSurface videoSurface)
    {
        foreach(AgoraUserData x in agoraUserData)
        {
            if(x.userID == userID)
            {
                x.SetVideoSurface(videoSurface);
            }
        }
    }

    public VideoSurface GetVideoSurface(uint userID)
    {
        foreach (AgoraUserData x in agoraUserData)
        {
            if (x.userID == userID)
            {
                return x.videoSurface;
            }
        }
        return null;
    }

    public bool isVideoEnabled(uint userID)
    {
        foreach (AgoraUserData x in agoraUserData)
        {
            if (x.userID == userID)
            {
                return x.VideoEnabled;
            }
        }
        return false;
    }

    public void SetVideoEnabled(uint userID, bool val)
    {
        foreach (AgoraUserData x in agoraUserData)
        {
            if (x.userID == userID)
            {
                x.VideoEnabled = val;
            }
        }
    }

    public void SetVideoSurfaceEnableDisable(uint userID, bool val)
    {
        foreach (AgoraUserData x in agoraUserData)
        {
            if (x.userID == userID)
            {
                x.VideoSurfaceSetEnable(val);
            }
        }
    }
}



public class AgoraController : MonoBehaviour
{
    public IRtcEngine mRtcEngine { get; set; }
    private AudioVideoStates AudioVideoState = new AudioVideoStates();
    private ToggleStateButton MuteAudioButton { get; set; }
    private ToggleStateButton MuteVideoButton { get; set; }
    private ToggleStateButton RoleButton { get; set; }
    private ToggleStateButton ChannelButton { get; set; }
    private CLIENT_ROLE_TYPE ClientRole { get; set; }
    private string mChannelName { get; set; }
    public static AgoraController instance;
    public static string token = "";

    bool TestStopEnableVideo = false;

    private uint localUserID;
    public uint LocalUserID
    {
        get
        {
            return localUserID;
        }
    }

    public AgoraUserDataHandler AgoraUserDataHandler = new AgoraUserDataHandler();


    private void Awake()
    {
        if(instance== null)
        {
            instance = this;
        }
        if(instance!= this)
        {
            Destroy(this.gameObject);
        }
        
    }

    // load agora engine
    public void loadEngine(string appId, string _token)
    {
        token = _token;
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);

        mRtcEngine.OnJoinChannelSuccess += onJoinChannelSuccess;
        mRtcEngine.OnUserJoined += onUserJoined;
        mRtcEngine.OnUserOffline += onUserOffline;
        mRtcEngine.OnLeaveChannel += OnLeaveChannelHandler;
    }
    public void SetupInitState()
    {
        AudioVideoState.pubAudio =true;
        AudioVideoState.pubVideo = true;
        AudioVideoState.subAudio = true;
        AudioVideoState.subVideo = true;
    }


    // Testing Volume Indication
    private bool TestVolumeIndication = false;
    public void join(string channel, bool enableVideoOrNot, uint _useraccount, bool muted = false )
    {
        Debug.Log("**join**");
        if (mRtcEngine == null)
            return;

        mRtcEngine.OnUserMutedAudio += OnUserMutedAudio;
        mRtcEngine.OnUserMuteVideo += OnUserMutedVideo;
        mRtcEngine.OnVolumeIndication += OnVolumeIndicationHandler;
        mRtcEngine.OnUserEnableLocalVideo += OnLocalVideoStateChanged;


        SetupInitState();

        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
        };
        mRtcEngine.OnError = HandleError;

        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        // Turn this on to receive volumenIndication
        if (TestVolumeIndication)
        {
            mRtcEngine.EnableAudioVolumeIndication(500, 8, report_vad: true);
        }

        var _orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_LANDSCAPE;

        VideoEncoderConfiguration config = new VideoEncoderConfiguration
        {
            orientationMode = _orientationMode,
            degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_FRAMERATE,
            mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED
            // note: mirrorMode is not effective for WebGL
        };

        mRtcEngine.SetVideoEncoderConfiguration(config);

        mRtcEngine.EnableAudio();
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();

        mRtcEngine.JoinChannelByKey(channelKey: token, channelName: channel, info: "", uid: _useraccount);
        Debug.Log("**join - Ends**");

    }

    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("** onJoinChannelSuccess **");

        localUserID = uid;
        CreateNewVideoSurface(uid);
        AgoraUserDataHandler.AddNewUser(new AgoraUserData(uid, false, true));
        AgoraUserDataHandler.SetVideoSurfaceEnableDisable(uid, false);
        //        mRtcEngine.StopPreview();

        //mRtcEngine.StartPreview();
    }
    private void onUserJoined(uint uid, int elapsed)
    {

        Debug.Log("***onUserJoined***");
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);

        CreateNewVideoSurface(uid);
        AgoraUserDataHandler.AddNewUser(new AgoraUserData(uid, false, true));
    }


    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();

        GameObject go = GameObject.Find($"{localUserID}");
        if (go != null)
            Destroy(go);

    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (mRtcEngine != null)
        {
            if (!pauseVideo)
            {
                Debug.Log("Local: Stop Video");
                mRtcEngine.EnableVideo();
            }
            else
            {
                Debug.Log("Local: Start Video");
//                mRtcEngine.StopPreview();
                mRtcEngine.DisableVideo();
            }
        }
    }

    RawImage LocalUserRawImage = null;
    public void DisplayLocalCameraOutputOnTopOfUser()
    {
        LocalUserRawImage = GetChildRawImage(LocalUserID);
        if (LocalUserRawImage != null)
        {
            LocalUserRawImage.gameObject.SetActive(true);
            InitCameraDevice(LocalUserRawImage);
        }
    }
    public void HideLocalCameraOutputOnTopOfUser()
    {
        if(LocalUserRawImage == null)
        LocalUserRawImage = GetChildRawImage(LocalUserID);
        LocalUserRawImage.gameObject.SetActive(false);
    }



    void DelayedCamera()
    {
//        return;
//        InitCameraDevice(_rawImage);
    }

    private Rect _rect;
    private Texture2D _texture;
    private Vector2 _cameraSize = new Vector2(1920, 1080);
    private int _cameraFPS = 15;
    private WebCamTexture _webCameraTexture;
    RawImage _rawImage;

    void InitTexture()
    {
        _rect = new Rect(0, 0, Screen.width, Screen.height);
        _texture = new Texture2D((int)_rect.width, (int)_rect.height, TextureFormat.RGBA32, false);
    }

    public void InitCameraDevice(RawImage _rawImage)
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        _webCameraTexture = new WebCamTexture(devices[0].name, (int)_cameraSize.x, (int)_cameraSize.y, _cameraFPS);
        _rawImage.texture = _webCameraTexture;
        _webCameraTexture.Play();
    }


    private void Update()
    {
    }

    GameObject GetChildVideoLocation(uint userUID)
    {
        Debug.Log("**GetChildVideoLocation**");
        GameObject go;
//        return AgoraVideoSetup.instace.GetPlayerVideosChild(userUID.ToString());
        go = GameObject.Find("Videoss");
        return go;
    }

    RawImage GetChildRawImage(uint userUID)
    {
        return AgoraVideoSetup.instace.GetPlayerVideosRawImage(userUID.ToString());
    }

    GameObject GetChildVideoLocation(string userUID)
    {
        GameObject go = GameObject.Find("Videos");
        GameObject childVideo = go.transform.Find($"{userUID}")?.gameObject;

        if(childVideo == null)
        {
            childVideo = new GameObject($"{userUID}");
            childVideo.transform.parent = go.transform;
        }
        return childVideo;
    }


    VideoSurface GetPlayerVideoSurfaceID(int userUID)
    {
        VideoSurface vs = PhotonController._instance.GetPlayerReferenceWithAgoraVideoSurface(userUID);

        if (vs == null)
            Debug.Log("Video Surface not found");
        else
        {
            Debug.Log("Video Surface found");
        }

        return vs;
    }



    VideoSurface MakeImageVideoSurface(GameObject go, string userUID, float deltax = 250, float deltay = 250)
    {
        //if (go.GetComponent<VideoSurface>())
        //    return go.GetComponent<VideoSurface>();

        GameObject newVideoobj = new GameObject($"{userUID}");
        newVideoobj.transform.SetParent(go.transform);
        newVideoobj.AddComponent<RawImage>();
        newVideoobj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        var rectTransform = newVideoobj.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(deltax, deltay);
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);

        rectTransform.localRotation = new Quaternion(0, rectTransform.localRotation.y, -180f,
            rectTransform.localRotation.w);

        //ADD Text with user name
        GameObject newGO = new GameObject("-- " + $"{userUID}");
        newGO.transform.parent = newVideoobj.transform;
        newGO.AddComponent<TextMeshProUGUI>().text = "-" + userUID;

        newGO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        var rectTransform2 = newGO.GetComponent<RectTransform>();

        rectTransform2.sizeDelta = new Vector2(deltax, deltay);
        rectTransform2.localPosition = new Vector3(rectTransform2.localPosition.x, rectTransform2.localPosition.y, 0);

        rectTransform2.localRotation = new Quaternion(0, rectTransform2.localRotation.y, -180f,
            rectTransform2.localRotation.w);

        return newVideoobj.AddComponent<VideoSurface>();
    }

    [SerializeField]
    private List<GameObject> remoteUserDisplays = new List<GameObject>();

    void CreateNewVideoSurface(uint uid)
    {
        Debug.Log("**CreateNewVideoSurface**");
        GameObject childVideo = GetChildVideoLocation(uid);
        VideoSurface videoSurface = /*GetPlayerVideoSurfaceID((int)uid);//*/ MakeImageVideoSurface(childVideo, uid.ToString(), 200f, 200f);

        if (videoSurface != null)
        {
            SetVideoSurfaceForUserID(uid, videoSurface);
            AgoraUserDataHandler.AssignVideoSurface(uid, videoSurface);
        }
    }

    void SetVideoSurfaceForUserID(uint uid, VideoSurface videoSurface)
    {
        videoSurface.SetForUser(uid);
        videoSurface.SetEnable(true);
        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
    }



    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onSceneHelloVideoLoaded()
    {
        Debug.Log("onSceneHelloVideoLoaded");
        // Attach the SDK Script VideoSurface for video rendering
        GameObject quad = GameObject.Find("Quad");
        if (ReferenceEquals(quad, null))
        {
            Debug.Log("failed to find Quad");
            return;
        }
        else
        {
            quad.AddComponent<VideoSurface>();
        }

        GameObject cube = GameObject.Find("Cube");
        if (ReferenceEquals(cube, null))
        {
            Debug.Log("failed to find Cube");
            return;
        }
        else
        {
            cube.AddComponent<VideoSurface>();
        }
        SetButtons();
    }

    private void SetButtons()
    {
        MuteAudioButton = GameObject.Find("MuteButton").GetComponent<ToggleStateButton>();
        if (MuteAudioButton != null)
        {
            MuteAudioButton.Setup(initOnOff: false,
                onStateText: "Mute Local Audio", offStateText: "Unmute Local Audio",
                callOnAction: () =>
                {
                    mRtcEngine.MuteLocalAudioStream(true);
                },
                callOffAction: () =>
                {
                    mRtcEngine.MuteLocalAudioStream(false);
                }
            );
        }

        MuteVideoButton = GameObject.Find("CamButton").GetComponent<ToggleStateButton>();
        if (MuteVideoButton != null)
        {
            MuteVideoButton.Setup(initOnOff: false,
                onStateText: "Mute Local video", offStateText: "Unmute Local video",
                callOnAction: () =>
                {
                    mRtcEngine.MuteLocalVideoStream(true);
                },
                callOffAction: () =>
                {
                    mRtcEngine.MuteLocalVideoStream(false);
                }
            );
        }

        ChannelButton = GameObject.Find("ChannelButton").GetComponent<ToggleStateButton>();
        if (ChannelButton != null)
        {
            ChannelButton.Setup(initOnOff: false,
                onStateText: mChannelName + "2", offStateText: mChannelName,
                callOnAction: () =>
                {
                    mRtcEngine.SwitchChannel(null, mChannelName + "2");
                },
                callOffAction: () =>
                {
                    mRtcEngine.SwitchChannel(null, mChannelName);
                }
                );
        }
        SetupRoleButton(isHost: ClientRole == CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    private void SetupRoleButton(bool isHost)
    {
        if (RoleButton != null)
        {
            RoleButton.Setup(initOnOff: isHost,
                 onStateText: "Host", offStateText: "Audience",
                 callOnAction: () =>
                 {
                     Debug.Log("Switching role to Broadcaster");
                     mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
                     ChannelButton.GetComponent<Button>().interactable = false;
                     MuteAudioButton.Reset();
                     MuteVideoButton.Reset();
                     MuteVideoButton.GetComponent<Button>().interactable = true;
                     MuteAudioButton.GetComponent<Button>().interactable = true;
                 },
                 callOffAction: () =>
                 {
                     Debug.Log("Switching role to Audience");
                     mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
                     ChannelButton.GetComponent<Button>().interactable = true;
                     MuteVideoButton.GetComponent<Button>().interactable = false;
                     MuteAudioButton.GetComponent<Button>().interactable = false;
                 }
                 );

        }
    }
    private void OnUserMutedAudio(uint uid, bool muted)
    {
        Debug.LogFormat("user {0} muted audio:{1}", uid, muted);
    }

    private void OnLocalVideoStateChanged(uint uid, bool muted)
    {
        Debug.LogFormat("user {0} videostate: {1}", uid, muted);
    }

    private void OnUserMutedVideo(uint uid, bool muted)
    {
        Debug.Log("Client: " + uid + ", MutedVideo:" + muted);
        AgoraUserDataHandler.SetVideoSurfaceEnableDisable(uid, !muted);
    }

    #region ZombieCode

    void OnVolumeIndicationHandler(AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume)
    {
        Debug.Log("OnVolumeIndicationHandler speakerNumber:" + speakerNumber + " totalvolume:" + totalVolume);
        foreach (var sp in speakers)
        {
            Debug.LogFormat("Speaker:{0} level:{1} channel:{2}", sp.uid, sp.volume, sp.channelId);
        }
    }
    private void OnLeaveChannelHandler(RtcStats stats)
    {
        Debug.LogFormat("OnLeaveChannelSuccess ---- duration = {0} txVideoBytes:{1} ", stats.duration, stats.txVideoBytes);
        // Clean up the displays
        foreach (var go in remoteUserDisplays)
        {
           Destroy(go);
        }
        remoteUserDisplays.Clear();
    }


    VideoSurface makePlaneSurface(string goName)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (go == null)
        {
            return null;
        }
        go.name = goName;
        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        float yPos = Random.Range(3.0f, 5.0f);
        float xPos = Random.Range(-2.0f, 2.0f);
        go.transform.position = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(0.25f, 0.5f, .5f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }
    private const float Offset = 100;
    public VideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }
        go.name = goName;
        // to be renderered onto
        go.AddComponent<RawImage>();

        // make the object draggable
        go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            go.transform.SetParent(canvas.transform);
        }
        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        go.transform.localPosition = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(5 * 1.6666f, 5f, 1f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }


    public void _muteLocal(Toggle _tg)
    {
        if(_tg.isOn)
        mRtcEngine.MuteLocalAudioStream( true);
        else
        {
            mRtcEngine.MuteLocalAudioStream(false);
        }
    }

    public void _muteRemote(Toggle _tg)
    {
        if (_tg.isOn)
            mRtcEngine.MuteAllRemoteAudioStreams(true);
        else
        {
            mRtcEngine.MuteAllRemoteAudioStreams(false);
        }
    }

    #endregion ZombieCode

    #region Error Handling
    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }
        msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));
        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }
        Debug.LogError(msg);
        LastError = error;
    }

    #endregion
}
