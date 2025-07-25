using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ProjectX
{
    
    public class PersistDataMgr
    {

        // public void SaveAll(bool bForce) 
        // {
        //     Debug.Log($"SaveAll({bForce})");
        //     foreach(var data in m_dicAllData.Values)
        //     {
        //         Save(data);
        //     }
        // }

        // public void Save(object data)
        // {
        //     var dataFilePath = GetFilePath(data.GetType().FullName);
        //     SerializeHelper.SerializeJson(dataFilePath, data);
        // }
        //
        //
        // public T Load<T>() where T : new()
        // {
        //     T data = default;
        //     var dataFilePath = GetFilePath(typeof(T).FullName);
        //     if (File.Exists(dataFilePath))
        //     {
        //         data = SerializeHelper.DeserializeJson<T>(dataFilePath);
        //     }
        //
        //     if (data == null)
        //     {
        //         data = new T();
        //     }
        //     m_dicAllData[data.GetType()] = data;
        //     
        //     return data;
        // }

        // public void OnApplicationQuit()
        // {
        //     SaveAll(true);
        // }
        //
        // public void OnApplicationPause(bool isPause)
        // {
        //     if (isPause)
        //     {
        //         SaveAll(true);
        //     }
        // }


        private Dictionary<Type, object> m_dicAllData = new Dictionary<Type, object>();



        // private static bool ENCRY = false;
    }
}
