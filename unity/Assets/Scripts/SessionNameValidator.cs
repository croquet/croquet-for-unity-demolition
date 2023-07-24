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
            // Force Capital
            if (ch >= 'a' && ch <= 'z')
            {
                ch = (char)((int)ch - 32);
            }
            // Force Alphabetic
            if (ch >= 'A' && ch <= 'Z')
            {
                // Force Maximum Length
                if (text.Length < lengthLimit)
                {
                    text += ch;
                    pos += 1;
                }
                return ch;
            }
            // If invalid, do not enter character
            return (char)0;
        }
    }
    
}

