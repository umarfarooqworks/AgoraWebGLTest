using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerAIFollowHelper : MonoBehaviourPunCallbacks
{
    public NavMeshAgent agent;
    public Animator anim;
    public Transform targetPlayer;
    public PhotonView view;

    public float minDistance = 1f;
    public bool follow;

    public UnityEvent OnStartFollowingEvents;
    public UnityEvent OnStopFollowingEvents;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PhotonController._instance.Players.Count);
        PhotonController._instance.Players.Add(view);
        Debug.Log(PhotonController._instance.Players.Count);

        if(view.Owner.IsLocal)
        {
            transform.gameObject.name = "Local";
        }


        PlayerDisplayController.instance.OnNewPlayerJoined(view);
    }

    // Update is called once per frame
    void Update()
    {
 


        if (!view.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.F) && !follow)
        {
            follow = true;
            OnFollowOtherPlayer();
        }
        else if (Input.GetKeyDown(KeyCode.F) && follow)
        {
            follow = false;
            OnStopFollow();
        }

        if (follow)
        {
            if (targetPlayer == null)
            {
                OnStopFollow();
                return;
            }

            agent.stoppingDistance = minDistance;
            if (Vector3.Distance(transform.position, targetPlayer.position) < minDistance)
            {
                //InputVertical
                anim.SetFloat("InputMagnitude", 0);
                agent.enabled = false;
                return;
            }
            else
            {
                agent.enabled = true;
                anim.SetFloat("InputMagnitude", 1);
            }

            agent.SetDestination(targetPlayer.position);
        }
    }

    public void OnFollowOtherPlayer()
    {
        foreach(PhotonView x in PhotonController._instance.Players)
        {
            if(x != view && x!=null)
            {
                targetPlayer = x.transform;
                Debug.Log("Target Set To : " + x.transform.name);
                OnStartFollow();
                return;
            }
        }

    }

    void OnStartFollow()
    {
        OnStartFollowingEvents.Invoke();
    }
    void OnStopFollow()
    {
        follow = false;
        OnStopFollowingEvents.Invoke();
    }

    public void StartFollowing(PhotonView _view)
    {
        //if (follow)
        //    return;
        follow = true;
        targetPlayer = _view.transform;
        Debug.Log("Target Set To : " + _view.transform.name);
        OnStartFollow();
    }

    public void StopFollowing()
    {
        follow = false;
        //InputVertical
        anim.SetFloat("InputMagnitude", 0);
        OnStopFollow();
    }

    private void OnDestroy()
    {
        if(PhotonController._instance.Players.Contains(view))
            PhotonController._instance.Players.Remove(view);

        PlayerDisplayController.instance?.ResetList();
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        //if(otherPlayer == targetPlayer.GetComponent<PhotonView>().Owner)
        //{
        //    OnStopFollow();
        //}
    }

}
