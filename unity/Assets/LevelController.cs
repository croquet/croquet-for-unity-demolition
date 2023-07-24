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
        Debug.Log($"number of scenes active in build settings: ${SceneManager.sceneCountInBuildSettings}");
        Debug.Log($"current sceneBuildIndex is {SceneManager.GetActiveScene().buildIndex}");
            
        int demolitionLevelToLoad = (SceneManager.GetActiveScene().buildIndex + 1)%(SceneManager.sceneCountInBuildSettings);
        
        // skip the main menu
        if (demolitionLevelToLoad == 0)
            demolitionLevelToLoad = 1;
        
        Debug.Log($"Next Level Button Attempting to load scene with sceneBuildIndex {demolitionLevelToLoad}");
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
            bridge.SetSessionName(sessionName); // this will start the session using the default name
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
    
}
