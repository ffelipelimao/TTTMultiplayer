using TMPro;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] private Transform _topPlayersContainer;
    [SerializeField] private TextMeshProUGUI _playersOnlineLbl;
    [SerializeField] private GameObject _playerRowPrefab;

    void Start()
    {
        OnServerStatusRequestHandler.OnServerStatus += Refresh;
        RequestSeverStatus();
    }

    void OnDestroy()
    {
        OnServerStatusRequestHandler.OnServerStatus -= Refresh;
    }

    void Refresh(Net_OnServerStatus msg)
    {
        while (_topPlayersContainer.childCount > 0)
        {
            DestroyImmediate(_topPlayersContainer.GetChild(0).gameObject);
        }

        _playersOnlineLbl.text = $"{msg.PlayersCount} players online";

        for (int i = 0; i < msg.Players.Length; i++)
        {
            var player = msg.Players[i];
            var instance = Instantiate(_playerRowPrefab, _topPlayersContainer).GetComponent<PlayerRow>();
            instance.Init(player);
        }
    }

    void RequestSeverStatus()
    {
        var msg = new Net_ServerStatusRequest();
        Client.Instance.SendServer(msg);
    }

}
