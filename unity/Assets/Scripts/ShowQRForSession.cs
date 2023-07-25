using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowQRForSession : MonoBehaviour
{
    private ShowQRCode qrShower;

    void Start()
    {
        qrShower = GameObject.FindObjectOfType<ShowQRCode>();
        if (qrShower == null) enabled = false;
    }

    void Update()
    {
        if (CroquetBridge.Instance.croquetSessionState == "running") { // @@ provide static Croquet accessor
            string localReflector = PlayerPrefs.GetString("sessionIP", "");
            string sessionNameValue = CroquetBridge.Instance.sessionName;
            string url;
            if (localReflector == "")
            {
                // Debug.Log("local reflector session ip setting empty, using live croquet network");
                url = $"https://croquet.io/demolition-multi/?q={sessionNameValue}";
            }
            else
            {
                // Debug.Log("local reflector session ip setting found, using set ip");
                url = $"http://{localReflector}/demolition-multi?q={sessionNameValue}&reflector=ws://{localReflector}/reflector&files=http://{localReflector}/files";
            }

            string reflectorMsg = localReflector == "" ? "" : $" on reflector {localReflector}";
            Debug.Log($"Displaying QR code for session {sessionNameValue}{reflectorMsg}");
            qrShower.DisplayQR(url);

            enabled = false;
        }
    }
}
