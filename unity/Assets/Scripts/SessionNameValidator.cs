using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
    
    [CreateAssetMenu(fileName = "InputValidator - SessionNameValidator.asset", menuName = "TextMeshPro/Input Validators/SessionNameValidator", order = 100)]
    public class SessionNameValidator : TMP_InputValidator
    {
        public int lengthLimit = 5;
        // Custom text input validation function
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (ch >= 'A' && ch <= 'Z')
            {
                if (text.Length < 5)
                {
                    text += ch;
                    pos += 1;
                }
                return ch;
            }
            return (char)0;
        }
    }
    
}

