using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextToCurrentSessionName : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<TMP_Text>().text =  FindObjectOfType<CroquetBridge>().sessionName;
    }

}
