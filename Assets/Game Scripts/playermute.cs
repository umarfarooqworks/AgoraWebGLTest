using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using agora_gaming_rtc;
public class playermute : MonoBehaviour
{
    public Image _playerVOiceImg;
    public Text _playername;
    public Sprite _muteimg;
    public Sprite _unmuteImg;
    bool _ismuted=false;
    public string _playerUID;
    public PhotonView _pv;
   
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=> _MUterplayer());


     
    }


    public void _MUterplayer()
    {

      
       

        if (!_pv.IsMine)
        {

           UserInfo _UI = AgoraController.instance.mRtcEngine.GetUserInfoByUserAccount(_playerUID);
            print(_UI.uid + " is uid");
            if (!_ismuted)
            {
                _playerVOiceImg.sprite = _muteimg;
                AgoraController.instance.mRtcEngine.MuteRemoteAudioStream(_UI.uid, true);
                _ismuted = true;
            }
            else
            {
                _playerVOiceImg.sprite = _unmuteImg;
                AgoraController.instance.mRtcEngine.MuteRemoteAudioStream(_UI.uid, false);
                _ismuted = false;
            }
    }
        else
        {
            if (!_ismuted)
            {
                _playerVOiceImg.sprite = _muteimg;
                AgoraController.instance.mRtcEngine.MuteLocalAudioStream( true);
                _ismuted = true;
            }
            else
            {
                _playerVOiceImg.sprite = _unmuteImg;
                AgoraController.instance.mRtcEngine.MuteLocalAudioStream( false);
                _ismuted = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
