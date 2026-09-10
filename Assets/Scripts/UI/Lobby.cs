using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private Transform _topPlayersContainer;
    [SerializeField] private Transform _logoutButton;
    [SerializeField] private TextMeshProUGUI _playersOnlineLbl;
    [SerializeField] private GameObject _playerRowPrefab;

    private const string LoginScene = "00_Main";

    void Start()
    {
        _logoutButton.GetComponent<Button>().onClick.AddListener(LogoutAction);

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

    void LogoutAction()
    {
        Debug.Log("Logging out...");
        Client.Instance.Disconnect();
        SceneManager.LoadScene(LoginScene);
    }

}
