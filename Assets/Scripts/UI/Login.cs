using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using TTT.PacketHandlers;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private Transform _loginButton;
    private TMP_InputField _usernameInput;
    private TMP_InputField _passwordInput;
    private TextMeshProUGUI _loginText;
    private TextMeshProUGUI _usernameErrorMessage;
    private TextMeshProUGUI _loginErrorMessage;
    private string _username = string.Empty;
    private string _password = string.Empty;
    public Transform _loadingUI;
    private bool _isConnected;

    private bool _isValidPassword;

    [SerializeField] private int _MaxPasswordLength = 10;
    [SerializeField] private int _MaxUsernameLength = 10;


    void Start()
    {
        _loginButton = transform.Find("LoginButton");
        _loginButton.GetComponent<Button>().onClick.AddListener(LoginAction);
        _loginText = _loginButton.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        _usernameInput = transform.Find("UsernameInput").GetComponent<TMP_InputField>();
        _usernameInput.onValueChanged.AddListener(ChangeUsername);

        _passwordInput = transform.Find("PasswordInput").GetComponent<TMP_InputField>();
        _passwordInput.onValueChanged.AddListener(ChangePassword);

        _usernameErrorMessage = _usernameInput.transform.Find("UsernameErrorMessage").GetComponent<TextMeshProUGUI>();
        _loginErrorMessage = _loginButton.Find("LoginErrorMessage").GetComponent<TextMeshProUGUI>();

        _usernameErrorMessage.text = string.Empty;
        _loginErrorMessage.text = string.Empty;

        EnableLoginButton(false);

        Client.Instance.OnServerConnected += SetIsConnected;

        OnAuthFailedHandler.OnAuthFailed += HandleAuthFailed;
    }

    void HandleAuthFailed(Net_OnAuthFailed msg)
    {
        _loadingUI.gameObject.SetActive(false);
        _loginErrorMessage.text = "Invalid username or password.";

        EnableLoginButton(_isValidPassword);
    }

    void SetIsConnected()
    {
        _isConnected = true;
    }

    void OnDestroy()
    {
        OnAuthFailedHandler.OnAuthFailed -= HandleAuthFailed;

        Client.Instance.OnServerConnected -= SetIsConnected;
    }

    void LoginAction()
    {
        StartCoroutine(LoginRoutine());
    }

    void ChangeUsername(string value)
    {
        _username = value;
        ValidateUsernameAndUpdateUI();
    }

    void ChangePassword(string value)
    {
        _password = value;
        _isValidPassword = ValidatePasswordAndUpdateUI();

        EnableLoginButton(_isValidPassword);
    }

    bool ValidateUsernameAndUpdateUI()
    {
        string error = string.Empty;

        if (string.IsNullOrWhiteSpace(_username))
            error = "Empty username";
        else if (_username.Length > _MaxUsernameLength)
            error = $"Max {_MaxUsernameLength} chars.";
        else if (!Regex.IsMatch(_username, "^[a-zA-Z0-9]+$"))
            error = "Only letters and numbers to username.";

        _usernameErrorMessage.text = error;

        return string.IsNullOrEmpty(error);
    }

    bool ValidatePasswordAndUpdateUI()
    {
        string error = string.Empty;

        if (string.IsNullOrWhiteSpace(_password))
            error = "Empty password.";
        else if (_password.Length > _MaxPasswordLength)
            error = $"Max {_MaxUsernameLength} chars.";

        _loginErrorMessage.text = error;

        return string.IsNullOrEmpty(error);
    }


    void EnableLoginButton(bool enable)
    {
        var button = _loginButton.GetComponent<Button>();
        button.interactable = enable;
        _loginText.color = button.interactable ? Color.white : Color.grey;
    }

    IEnumerator LoginRoutine()
    {
        Debug.Log("Logging in...");
        EnableLoginButton(false);
        _loadingUI.gameObject.SetActive(true);

        Client.Instance.Connect();

        while (!_isConnected)
        {
            Debug.Log("Connecting...");
            yield return null;
        }

        Debug.Log("Connected!");

        var authRequest = new Net_AuthRequest()
        {
            Username = _username,
            Password = _password
        };

        Client.Instance.SendServer(authRequest);
    }
}
