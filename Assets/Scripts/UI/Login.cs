using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private Transform _loginButton;
    private TMP_InputField _usernameInput;
    private TMP_InputField _passwordInput;
    private TextMeshProUGUI _loginText;

    private string _username = string.Empty;
    private string _password = string.Empty;

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
    }

    void LoginAction()
    {
        Debug.Log("Logging in...");
    }
    void ChangeUsername(string value)
    {
        _username = value;
        ValidateAndUpdateUI();
    }
    void ChangePassword(string value)
    {
        _password = value;
        ValidateAndUpdateUI();
    }

    void ValidateAndUpdateUI()
    {
        var usernameRegex = Regex.Match(_username, "^[a-zA-Z0-9]+$");

        var interactable =
            !string.IsNullOrWhiteSpace(_username) &&
            !string.IsNullOrWhiteSpace(_password) &&
            _username.Length <= _MaxUsernameLength &&
            _password.Length <= _MaxPasswordLength &&
            usernameRegex.Success;

        EnableLoginButton(interactable);
    }

    void EnableLoginButton(bool interactable)
    {
        _loginButton.GetComponent<Button>().interactable = interactable;
        var color = _loginButton.GetComponent<Button>().interactable ? Color.white : Color.grey;
        _loginText.color = color;
    }
}
