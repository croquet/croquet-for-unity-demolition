using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public TMP_InputField sessionNameInputField;
    public TMP_Text joinCodeIssueText;

    public static void LoadNextCroquetLevel()
    {
        // Debug.Log($"number of scenes active in build settings: {SceneManager.sceneCountInBuildSettings}");
        // Debug.Log($"current sceneBuildIndex is {SceneManager.GetActiveScene().buildIndex}");

        int demolitionLevelToLoad = (SceneManager.GetActiveScene().buildIndex + 1)%(SceneManager.sceneCountInBuildSettings);

        // skip the main menu
        if (demolitionLevelToLoad == 0)
            demolitionLevelToLoad = 1;

        Debug.Log($"Next Level button requesting scene with buildIndex {demolitionLevelToLoad}");
        Croquet.RequestToLoadScene(demolitionLevelToLoad, forceReload: false);
    }

    public static void LoadPreviousCroquetLevel()
    {
        // Debug.Log($"number of scenes active in build settings: {SceneManager.sceneCountInBuildSettings}");
        // Debug.Log($"current sceneBuildIndex is {SceneManager.GetActiveScene().buildIndex}");

        int demolitionLevelToLoad = (SceneManager.GetActiveScene().buildIndex - 1)%(SceneManager.sceneCountInBuildSettings);

        // skip the main menu
        if (demolitionLevelToLoad == 0)
            demolitionLevelToLoad = SceneManager.sceneCountInBuildSettings-1;

        Debug.Log($"Previous Level button requesting scene with buildIndex {demolitionLevelToLoad}");
        Croquet.RequestToLoadScene(demolitionLevelToLoad, forceReload: false);
    }

    public static void ResetCroquetLevel()
    {
        int demolitionLevelToLoad = SceneManager.GetActiveScene().buildIndex;
        Croquet.RequestToLoadScene(demolitionLevelToLoad, true);
    }

    public static void StartDefaultSession()
    {
        Croquet.SetSessionName(""); // this will start the session using the default name
    }

    public void StartRandomSession()
    {
        Croquet.SetSessionName(GenerateValidSessionName()); // start the session using a random name
    }

    public void StartSessionWithName()
    {
        string sessionName = sessionNameInputField.text;
        if (sessionName.Length == 5)
        {
            Croquet.SetSessionName(sessionName); // start the session using the chosen name
        }
        else
        {
            joinCodeIssueText.text = "Join Code should be 5 characters!";
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
