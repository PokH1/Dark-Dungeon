using AbstractionServer;
using UnityEngine;

public class ConnectionBridge : MonoBehaviour
{
    public LoginDataSO loginData;

    public void SignupAndLogin()
    {
        StartCoroutine(AuthService.RegisterAndLogin(loginData.nickname, loginData.password,
            token =>
            {
                Debug.Log("Login successful, token: " + token);
            },
            err => Debug.LogError("Login failed: " + err)
        ));
    }
}
