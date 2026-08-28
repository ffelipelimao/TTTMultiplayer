using UnityEngine;

public class Lobby : MonoBehaviour
{
    void Start()
    {

    }

    void RequestSeverStatus()
    {
        var msg = new Net_ServerStatusRequest();
        Client.Instance.SendServer(msg);
    }

}
