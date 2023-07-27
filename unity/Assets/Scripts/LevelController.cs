using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public TMP_InputField sessionNameInputField;

    public static void LoadNextCroquetLevel()
    {
        // Debug.Log($"number of scenes active in build settings: {SceneManager.sceneCountInBuildSettings}");
        // Debug.Log($"current sceneBuildIndex is {SceneManager.GetActiveScene().buildIndex}");

        int demolitionLevelToLoad = (SceneManager.GetActiveScene().buildIndex + 1)%(SceneManager.sceneCountInBuildSettings);

        // $$$ TEST SETUP: RETURN TO MAIN MENU AFTER LAST SCENE
        if (demolitionLevelToLoad == 0)
        {
            ReturnToMainMenu();
            return;
        }

        // skip the main menu
        if (demolitionLevelToLoad == 0)
            demolitionLevelToLoad = 1;

        Debug.Log($"Next Level button requesting scene with buildIndex {demolitionLevelToLoad}");
        Croquet.RequestToLoadScene(demolitionLevelToLoad, forceReload: false);
    }

    public static void StartDefaultSession()
    {
        CroquetBridge bridge = FindObjectOfType<CroquetBridge>();
        if (bridge != null)
        {
            bridge.SetSessionName(""); // this will start the session using the default name
        }
    }

    public void StartRandomSession()
    {
        CroquetBridge bridge = FindObjectOfType<CroquetBridge>();
        if (bridge != null)
        {
            bridge.SetSessionName(GenerateValidSessionName()); // this will start the session using the default name
        }
    }

    public void StartSessionWithName()
    {
        CroquetBridge bridge = FindObjectOfType<CroquetBridge>();
        if (bridge != null)
        {
            string sessionName = sessionNameInputField.text;
            if (sessionName.Length == 5)
            {
                bridge.SetSessionName(sessionName); // this will start the session using the default name
            }
            else
            {
                Debug.Log("Provided Session Name is not of full length 5!");
            }
        }
    }

    public string GenerateValidSessionName()
    {
        StringBuilder sessionName = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            int randomNumber = Random.Range(65, 91); // CAPITAL ALPHABETIC
            sessionName.Append((char)randomNumber);
        }

        return sessionName.ToString();
    }

    public static void ReturnToMainMenu()
    {
        // tell Croquet to tell us to shut down then load scene 0
        // NB: currently a static method
        CroquetBridge.Instance.SendToCroquet("shutdown", "0");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
