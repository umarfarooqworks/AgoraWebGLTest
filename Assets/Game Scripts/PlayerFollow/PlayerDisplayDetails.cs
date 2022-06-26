using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplayDetails : MonoBehaviour
{
    public PhotonView referencePlayer;
    public Text PlayerNameText;
    public Button FollowButton;
    public Button UnFollowbutton;

    public static Action<Button> OnPalayerClicksFollow;

    public void init(PhotonView view)
    {
        referencePlayer = view;
        string gender = "";

        if(PhotonHelper.GetPlayerCustomProperty<int>(view, PropertiesData.Gender, 1) == PropertiesData.MaleCode)
        {
            gender = "(M)";
        }
        else if (PhotonHelper.GetPlayerCustomProperty<int>(view, PropertiesData.Gender, 1) == PropertiesData.FeMaleCode)
        {
            gender = "(F)";
        }

        PlayerNameText.text = view.Owner.NickName;// + gender;
        FollowButton.onClick.AddListener(OnClickFollowButton);
        UnFollowbutton.onClick.AddListener(OnClickUnFollow);
        OnPalayerClicksFollow += OnFollowClicked;
    }

    public void OnClickFollowButton()
    {
        PhotonController._instance.MakeLocalPlayerFollow(referencePlayer);
        OnPalayerClicksFollow.Invoke(FollowButton);
    }


    void OnFollowClicked(Button btn)
    {
        if(btn == FollowButton)
        {
            FollowButton.gameObject.SetActive(false);
            UnFollowbutton.gameObject.SetActive(true);
        }
        else
        {
            FollowButton.gameObject.SetActive(true);
            UnFollowbutton.gameObject.SetActive(false);
        }
    }


    public void OnClickUnFollow()
    {
        PhotonController._instance.MakeLocalPlayerUnFollow();
        FollowButton.gameObject.SetActive(true);
        UnFollowbutton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        FollowButton.onClick.RemoveListener(OnClickFollowButton);
        UnFollowbutton.onClick.RemoveListener(OnClickUnFollow);
        OnPalayerClicksFollow -= OnFollowClicked;
    }
}
