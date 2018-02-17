using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System;

namespace Propolis {

    public static class PropolisStatsExporter
    {
        public static PropolisStatsModel PropolisStatsModel { get; set; }

        public static void LoadStatsFromFile() {
            string path = Application.persistentDataPath + "/PropolisEventStats.json";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    PropolisStatsModel =JsonUtility.FromJson<PropolisStatsModel>( sr.ReadToEnd());
                }
            }
            else
            {
                PropolisStatsModel = new PropolisStatsModel();
                SaveStats();
            }

        }

        public static void SaveStats() {
            string path = Application.persistentDataPath + "/PropolisEventStats.json";
            if (PropolisStatsModel != null) {
                using (FileStream fs = File.Create(path))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string stringifiedJson = JsonUtility.ToJson(PropolisStatsModel);
                        sw.Write(stringifiedJson);
                    }
                }

            }
        }

        public static void IncrementStatValue(string propName) {

            try
            {
                Type type = PropolisStatsModel.GetType();
                PropertyInfo infos = type.GetProperty(propName);
                int current = (int)infos.GetValue(PropolisStatsModel, null);
                infos.SetValue(PropolisStatsModel, current + 1, null);
            }
            catch (System.Exception ex)
            {
                //Debug.Log(ex.Message);
            }
        }
    }

}

