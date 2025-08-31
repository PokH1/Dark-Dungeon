using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AbstractionServer;

public class MenuManager : MonoBehaviour
{
    public LoginDataSO loginData;
    public GameObject signupPanel;
    public GameObject mainMenuPanel;
    public Button loginButton;

    private void Start()
    {
        // Opcional: si quieres también añadir el listener por código
        // loginButton.onClick.AddListener(OnLoginButtonPressed);
    }

    // Este es el método que asignas al botón desde el Inspector
    public void OnLoginButtonPressed()
    {
        StartCoroutine(SignupOrLogin());
    }

    private IEnumerator SignupOrLogin()
    {
        bool loginSuccess = false;

        yield return AuthService.RegisterAndLogin(
            loginData.nickname,
            loginData.password,
            onSuccess: (token) =>
            {
                loginSuccess = true;
            },
            onError: (error) =>
            {
                loginSuccess = false;
            }
        );

        if (loginSuccess)
        {
            SwitchPanels();
        }
    }

    public void OnNickNameChange(string nickname)
    {
        loginData.nickname = nickname;
    }

    public void OnPasswordChange(string password)
    {
        loginData.password = password;
    }

    private void SwitchPanels()
    {
        if (signupPanel != null && mainMenuPanel != null)
        {
            signupPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
}
