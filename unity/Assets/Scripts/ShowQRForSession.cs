using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowQRForSession : MonoBehaviour
{
    void Start()
    {
        ShowQRCode qrShower = GameObject.FindObjectOfType<ShowQRCode>();
        if (qrShower != null)
        {
            int sessionNameValue = PlayerPrefs.GetInt("sessionNameValue", 1);
            qrShower.DisplayQR($"https://croquet.io/demolition-multi/?q={sessionNameValue}");
        }
    }
}
