using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class PlayerArrivalIndicator : MonoBehaviour, IPunInstantiateMagicCallback
{
    public TMP_Text _playername;
    public Canvas _mycanvas;
    public GameObject _myvoiceControlsPrefab;
    PhotonView _pv;
    private void Start()
    {
        Invoke(nameof(InItData),3f);
    }

    public void InItData()
    {
        _mycanvas.worldCamera = Camera.main;
        _playername.text = GetComponent<PhotonView>().Owner.NickName;
        //_pv = GetComponent<PhotonView>();
        //GameObject _pvf = Instantiate(_myvoiceControlsPrefab, PhotonController._instance._playerlist.content.transform);
        //_pvf.GetComponent<playermute>()._playername.text = GetComponent<PhotonView>().Owner.NickName;
        //_pvf.GetComponent<playermute>()._playerUID = GetComponent<PhotonView>().ViewID.ToString();
        //_pvf.GetComponent<playermute>()._pv = _pv;
   
    }



    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
       // print("----------------");
       // print(info.photonView.Owner.NickName);
       // GameObject _temp = Instantiate(PhotonController._instance._playeralertPrefab,PhotonController._instance._playerlist.content);
       //_temp.GetComponent<Text>().text = info.photonView.Owner.NickName + " Joined";
      

    }
}
