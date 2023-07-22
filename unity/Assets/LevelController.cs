using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    
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
}
