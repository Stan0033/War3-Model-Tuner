 
using System.Globalization;
 

namespace Wa3Tuner.Helper_Classes
{
    public static class StringHelper
    {
        
            public static string TitleCase(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return string.Empty;

                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return textInfo.ToTitleCase(text.ToLower());
            }
       

    }
}


