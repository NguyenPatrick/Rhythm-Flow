using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour {


    public void changeToHomeScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void changeToGameScene()
    {
        SceneManager.LoadScene("Game");
    }
}
