using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ProjectX
{
    
    public class PersistDataMgr
    {

        public void SaveAll(bool bForce) 
        {
            Debug.Log($"SaveAll({bForce})");
            foreach(var data in m_dicAllData.Values)
            {
                Save(data);
            }
        }

        public void Save(object data)
        {
            var dataFilePath = GetFilePath(data.GetType().FullName);
            SerializeHelper.SerializeJson(dataFilePath, data);
        }


        // public T Load<T>() where T : new()
        // {
        //     T data = null;
        //     var dataFilePath = GetFilePath(typeof(T).FullName);
        //     if (File.Exists(dataFilePath))
        //     {
        //         data = SerializeHelper.DeserializeJson<T>(dataFilePath, ENCRY);
        //     }
        //
        //     if (data == null)
        //     {
        //         data = new T();
        //         data.SetDefaultValue();
        //     }
        //     m_dicAllData[data.GetType()] = data;
        //     
        //     return data;
        // }

        public static string GetFilePath(string fileName)
        {
            // if (ENCRY)
            // {
            fileName = fileName.GetHashCode().ToString();
            // }
            return string.Format("{0}{1}", persistentDataPath4Recorder, fileName);
        }

        public void OnApplicationQuit()
        {
            SaveAll(true);
        }

        public void OnApplicationPause(bool isPause)
        {
            if (isPause)
            {
                SaveAll(true);
            }
        }


        private Dictionary<Type, object> m_dicAllData = new Dictionary<Type, object>();


        private static string m_PersistentDataPath4Recorder;
        // 外部资源目录
        public static string persistentDataPath4Recorder
        {
            get
            {
                if (null == m_PersistentDataPath4Recorder)
                {
                    m_PersistentDataPath4Recorder = Application.persistentDataPath + "/cache/";

                    if (!Directory.Exists(m_PersistentDataPath4Recorder))
                    {
                        Directory.CreateDirectory(m_PersistentDataPath4Recorder);
#if UNITY_IPHONE && !UNITY_EDITOR
                        UnityEngine.iOS.Device.SetNoBackupFlag(m_PersistentDataPath4Recorder);
#endif
                    }
                }

                return m_PersistentDataPath4Recorder;
            }
        }

        // private static bool ENCRY = false;
    }
}
