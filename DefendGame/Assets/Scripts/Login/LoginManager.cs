using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwdField;

    public GameController gameController;

    public string testUsername1;
    public string testPasswd1;
    public string testUsername2;
    public string testPasswd2;
    public string testUsername3;
    public string testPasswd3;


    public void onLogin()
    {
        // press login button
        string username = usernameField.text.Trim().ToString();
        string passwd = passwdField.text.Trim().ToString();
        
        gameController.Login(username, passwd);
    }

    public void onRegister()
    {
        // press register button
        string username = usernameField.text.Trim().ToString();
        string passwd = passwdField.text.Trim().ToString();

        gameController.Register(username, passwd);
    }

    public void onLogin1()
    {
        // press test1 button
        gameController.Login(testUsername1, testPasswd1);
    }

    public void onLogin2()
    {
        // press test2 button
        gameController.Login(testUsername2, testPasswd2);
    }

    public void onLogin3()
    {
        // press test3 button
        gameController.Login(testUsername3, testPasswd3);
    }
}
