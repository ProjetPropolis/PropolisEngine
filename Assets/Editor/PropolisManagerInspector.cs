using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Propolis
{
    [CustomEditor(typeof(PropolisManager))]
    public class PropolisManagerInspector : Editor
    {
        string command ="";

        public override void OnInspectorGUI()
        {
            
            DrawDefaultInspector();

            PropolisManager manager = (PropolisManager)target;
            GUILayout.Box(manager.ConsoleLog);
            command = GUILayout.TextField(command, 100);
            if (GUILayout.Button("Send"))
            {
                manager.SendCommand(command);
            }
            if (GUILayout.Button("Clear"))
            {
                manager.ClearConsole();
            }
        }
    }
}
