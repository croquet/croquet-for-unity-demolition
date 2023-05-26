using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SessionNameChooser : MonoBehaviour
{
    // public reference to the text to update
    public TMPro.TMP_Text sessionNameText;
    public TMPro.TMP_Text sessionIPText;

    private static int SessionNameValue
    {
        get {
            return _sessionNameValue;
        }
        set {
            _sessionNameValue = Mathf.Clamp(value, 0, 100);
            if (OnSessionNameChange != null)
                OnSessionNameChange(_sessionNameValue);

            PlayerPrefs.SetInt("sessionNameValue", _sessionNameValue);
            PlayerPrefs.Save();
        }
    }

    private static int _sessionNameValue = 0;
    public delegate void OnSessionNameChangeDelegate(int newVal);
    public static event OnSessionNameChangeDelegate OnSessionNameChange;

    private static string _sessionIPValue = "";
    //public delegate void 

    // Start is called before the first frame update
    void Start()
    {
        OnSessionNameChange += SessionNameChangeHandler;

        // recover the session name from save data
        SessionNameValue = PlayerPrefs.GetInt("sessionNameValue");
        
    }

    private void SessionNameChangeHandler(int newVal)
    {
        sessionNameText.text = newVal.ToString();
    }


    // allow increment and decrement
    public static void increment(int amount)
    {
        SessionNameValue+=amount;
    }

    public static void decrement(int amount)
    {
        SessionNameValue-=amount;
    }
}
