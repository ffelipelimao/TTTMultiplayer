using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private Button _loginButton;
    private Button _sendButton;

    void Start()
    {
        _loginButton = transform.Find("Connect").GetComponent<Button>();
        _loginButton.onClick.AddListener(Connect);

        _sendButton = transform.Find("Send").GetComponent<Button>();
        _sendButton.onClick.AddListener(Send);
    }

    void Connect()
    {
        Debug.Log("Connecting to server...");
    }

    void Send()
    {
        Debug.Log("Sending msg to server...");
    }

}
