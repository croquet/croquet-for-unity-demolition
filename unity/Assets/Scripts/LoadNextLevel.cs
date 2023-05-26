using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    // instead of having to queue off the build scenes
    // lets provide a listing
    public int currentLevelIndex = 0;
    public List<string> levelNames = new List<string>();
    
    
    public void LoadNextScene()
    {
        currentLevelIndex += 1;
        if (currentLevelIndex >= levelNames.Count)
        {
            currentLevelIndex %= levelNames.Count;
        }
        SceneManager.LoadScene(levelNames[currentLevelIndex]);
    }
}
