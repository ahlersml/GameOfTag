using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField] private Camera _camera;
    [SerializeField] private TMP_Text _nameText;

    [SerializeField] private Color _taggedColor;
    private Color _initialColor;

    private bool _isTagged;

    private float _touchbackCountdown;
    [SerializeField] private float _touchbackDuration;

    private float _timeSpentTagged;
    

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(_camera.gameObject);
        }
        // Update the player's name display
        UpdateNameDisplay();

        //store initial color
        _initialColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
    }

    [PunRPC]
    public void OnTagged()
    {
        // Flag the player as tagged
        _isTagged = true;

        //start countdown for no touchback
        _touchbackCountdown = _touchbackDuration;

        //Change color of player who is it
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = _taggedColor;
    }

    [PunRPC]
    public void OnUnTagged()
    {
        // Flag the player as tagged
        _isTagged = false;

           //Change color of player back to initial color
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = _initialColor;
    }

    private void OnCollisionEnter(Collision other)
    {
        // check if bumped into player
        var otherPlayer = other.collider.GetComponent<PlayerController>();
        if(otherPlayer != null)
        {
            // if we are the tagged player and havent recently been tagged
            if (_isTagged && _touchbackCountdown <= 0f)
            {
                //Untag ourself
                photonView.RPC("OnUnTagged", RpcTarget.AllBuffered);

                //Tag other player
                otherPlayer.photonView.RPC("OnTagged", RpcTarget.AllBuffered);
            }
        }
    }


    private void Update()
    {
        UpdateNameDisplay();

        if (photonView.IsMine)
        {
            //Reduce our touchbacks timer
            if (_touchbackCountdown > 0f)
            {
                _touchbackCountdown -= Time.deltaTime;
            }

            //if we are tagged
            if(_isTagged)
            {
                //increase time we are tagged
                _timeSpentTagged += Time.deltaTime;
            }
            
        }

        //endGame();

    }
    private void UpdateNameDisplay()
    {
        //if the player is winning change their name color
        var isWinning = FindObjectsOfType<PlayerController>().OrderBy(p => p._timeSpentTagged).First() == this;
        if(isWinning)
        {
            _nameText.text = $"<color=green>{photonView.Owner.NickName}\n<size=50%>{_timeSpentTagged:F1} sec</size>";
        }
        else
        {
            _nameText.text = $"{photonView.Owner.NickName}\n<size=50%>{_timeSpentTagged:F1} sec</size>";
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log($"Writing for {info.Sender.NickName}");

            stream.SendNext(_timeSpentTagged);
            
        }
        if(stream.IsReading)
        {
            Debug.Log($"Writing for {info.Sender.NickName}");

            _timeSpentTagged = (float)stream.ReceiveNext();

            UpdateNameDisplay();
        }
    }

    /*public void endGame()
    {
        if (photonView.IsMine)
        {
            if(_timeSpentTagged >= 60f)
            {
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }*/
}
