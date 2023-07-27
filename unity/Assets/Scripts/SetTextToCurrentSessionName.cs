using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextToCurrentSessionName : MonoBehaviour
{
    void Update()
    {
        string sessionName = CroquetBridge.Instance.sessionName;
        if (sessionName != "")
        {
            gameObject.GetComponent<TMP_Text>().text = sessionName;
            enabled = false;
        }
    }

}
