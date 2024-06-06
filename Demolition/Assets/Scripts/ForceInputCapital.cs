using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForceInputCapital : MonoBehaviour
{
    private TMP_InputField inputField;
    
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(delegate(string arg0) { ForceCapital();  });
    }

    void ForceCapital()
    {
        inputField.text = inputField.text.ToUpper();
    }
}
