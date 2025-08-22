using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public LoginDataSO loginData;
    public GameObject signup;
    public GameObject mainMenu;
    public void OnNickNameChange(string nickname)
    {
        loginData.nickname = nickname;
    }

    public void OnPasswordChange(string password)
    {
        loginData.password = password;
    }

    public void SwitchPanels()
    {
        if (signup != null && mainMenu != null)
        {
            signup.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}
