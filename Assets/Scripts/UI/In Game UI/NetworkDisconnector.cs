using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDisconnector : MonoBehaviour
{
    public void Disconnect()
    {
        NetworkClient.Disconnect();

        NetworkManager.singleton.StopHost();
    }
}
