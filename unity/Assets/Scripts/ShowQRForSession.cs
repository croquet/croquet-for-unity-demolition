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
            CroquetRunner runner = CroquetBridge.Instance.GetComponent<CroquetRunner>();
            string localReflector = runner.localReflector;
            int sessionNameValue = PlayerPrefs.GetInt("sessionNameValue", 1);
            string url;
            if (localReflector == "")
            {
                url = $"https://croquet.io/demolition-multi/?q={sessionNameValue}";
            }
            else
            {
                url = $"http://{localReflector}/demolition-multi?q={sessionNameValue}&reflector=ws://{localReflector}/reflector&files=http://{localReflector}/files";
            }
            qrShower.DisplayQR(url);
        }
    }
}
