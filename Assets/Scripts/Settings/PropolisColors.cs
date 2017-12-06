using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    public static class PropolisColors
    {
        public static Color Dark { get { return GetColorFromHTML("#222222"); } }
        public static Color Red { get { return GetColorFromHTML("#EF5572"); } }
        public static Color Purple { get { return GetColorFromHTML("#3A3459"); } }
        public static Color Yellow { get { return GetColorFromHTML("#FDE981"); } }
        public static Color Blue { get { return GetColorFromHTML("#0BFFE2"); } }
        public static Color White { get { return GetColorFromHTML("#FFFFFF"); } }
        public static Color Fushia { get { return GetColorFromHTML("#FF0080"); } }
        public static Color DarkBlue { get { return GetColorFromHTML("#0000FF"); } }


        private static Color GetColorFromHTML(string hex)
        {
            Color color;

            if (ColorUtility.TryParseHtmlString(hex, out color))
            {
                return color;
            }
            return color;

        }  

    }
}

