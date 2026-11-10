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
    [SerializeField] private Button _findOpponentButton;
    [SerializeField] private Transform _loadingContainer;
    [SerializeField] private Button _cancelButton;

    private const string LoginScene = "00_Main";

    void Start()
    {
        _findOpponentButton.onClick.AddListener(FindOpponent);
        _cancelButton.onClick.AddListener(CancelFindOpponent);
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

    void FindOpponent()
    {
        //hide the find opponent button
        // show loading UI
        // send net find opponent request message

        _findOpponentButton.gameObject.SetActive(false);
        _loadingContainer.gameObject.SetActive(true);

        var msg = new Net_FindOpponentRequest();
        Client.Instance.SendServer(msg);
    }

    void CancelFindOpponent()
    {
        //hide loading UI
        // show the find opponent button
        // send net find opponent request message


        _findOpponentButton.gameObject.SetActive(true);
        _loadingContainer.gameObject.SetActive(false);

        var msg = new Net_CancelFindOpponentRequest();
        Client.Instance.SendServer(msg);
    }

}
