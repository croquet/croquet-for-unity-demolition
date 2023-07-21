using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static void LoadLevel(int level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);

        Debug.Log($"Loading Level: {level}");
    }

    public static void TmpStartDefaultSession()
    {
        CroquetBridge bridge = FindObjectOfType<CroquetBridge>();
        if (bridge != null)
        {
            bridge.SetSessionName(""); // this will start the session using the default name
        }
    }
}
