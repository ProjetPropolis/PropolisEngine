using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    public static class PropolisColors
    {
        public static Color Dark { get { return GetColorFromHTML("#302b49"); } }
        public static Color Red { get { return GetColorFromHTML("#ff5061"); } }
        public static Color Purple { get { return GetColorFromHTML("#887fd5"); } }
        public static Color Yellow { get { return GetColorFromHTML("#FDE981"); } }
        public static Color Blue { get { return GetColorFromHTML("#0bffe3"); } }
        public static Color Green { get { return GetColorFromHTML("#0baaFF"); } }
        public static Color White { get { return GetColorFromHTML("#FFFFFF"); } }
        public static Color Fushia { get { return GetColorFromHTML("#ff33bc"); } }
        public static Color DarkBlue { get { return GetColorFromHTML("#005af9"); } }
        public static Color Orange { get { return GetColorFromHTML("#ff7550"); } }
        public static Color SuperCleanTrigger = Color.green;
        public static Color SuperClean1 = Color.magenta;


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

