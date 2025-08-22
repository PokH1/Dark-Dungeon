using System;
using System.Collections;
using UnityEngine;

namespace AbstractionServer
{
    public class AuthService
    {
        [Serializable]
        public class LoginRequest
        {
            public string username;
            public string password;

            public LoginRequest(string user, string pass)
            {
                username = user;
                password = pass;
            }
        }
        public static string userId;

        [Serializable]
        public class AuthResponse
        {
            public User user;

            [Serializable]
            public class User
            {
                public string access_token;
                public string name;
                public string id;
            }
        }
            public static IEnumerator RegisterAndLogin(string username, string password, Action<String> onSuccess, Action<String> onError)
            {
                var user = new LoginRequest(username, password);

                yield return AbstractionApiClient.Post<LoginRequest, string>(
                    "/auth/register",
                    user,
                    _ =>
                    {
                        Debug.Log("Registration successful");
                    },
                    err =>
                    {
                        Debug.LogWarning("Registration failed (possibly already exist): " + err);
                    }
                );

                yield return AbstractionApiClient.Post<LoginRequest, AuthResponse>(
                    "/auth/login",
                    user,
                    res =>
                    {
                        AbstractionApiClient.Token = res.user.access_token;
                        AbstractionApiClient.userId = res.user.id;
                        Debug.Log("Login successful, token: " + res.user.access_token);
                        Debug.Log("Login successful, Wallet: " + AbstractionApiClient.userId);
                        onSuccess?.Invoke(res.user.access_token);
                    },
                    onError
                );
            }
        }

    }