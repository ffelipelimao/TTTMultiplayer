using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerRow : MonoBehaviour
{
    [SerializeField] GameObject _onlineImage, _offlineImage;
    [SerializeField] TextMeshProUGUI _username, _score;

    public void Init(Net_PlayerStatus player)
    {
        if (player.IsOnline)
        {
            _onlineImage.SetActive(true);
        }
        else
        {
            _offlineImage.SetActive(true);
        }

        _username.text = player.Username;
        _score.text = player.Score.ToString();
    }
}