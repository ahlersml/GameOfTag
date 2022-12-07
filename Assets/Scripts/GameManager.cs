using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _spawnPoint;
    private Vector3 _spawnLocation;
    float x;
    float z;
    float y = 9;

    // Start is called before the first frame update
    void Start()
    {
        //_spawnPoint.position,
        x = Random.Range(-15 ,5);
        z = Random.Range(-10, 8);
        _spawnLocation = new Vector3(x, y, z);
        var newPlayer = PhotonNetwork.Instantiate(_playerPrefab.name, _spawnLocation, Quaternion.identity);

        // If player is the host
        if(PhotonNetwork.IsMasterClient)
        {
            // Start as tagged
            newPlayer.GetComponent<PlayerController>().photonView.RPC("OnTagged", RpcTarget.AllBuffered);
        }
    }

   
}
