using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    public void SetJoinAddress(string address)
    {
        NetworkManager.singleton.networkAddress = address;
    }
}
