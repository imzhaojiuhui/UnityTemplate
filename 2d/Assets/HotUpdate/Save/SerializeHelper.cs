using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace ProjectX
{
    public class SerializeHelper
    {
        private static readonly string AesKey = "weoizkxjkfs";
        private static readonly string AesIv = "asjkdyweucn";
        private static readonly bool IsEncry = true;

        public static bool SerializeJson(string fileName, object obj)
        {
            var path = GetFilePath(fileName);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("SerializeJson Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Debug.LogError("SerializeJson obj is Null.");
                return false;
            }

            string jsonValue = null;
            try
            {
                jsonValue = JsonUtility.ToJson(obj);
                if (IsEncry)
                {
                    jsonValue = EncryptUtil.AesStr(jsonValue, AesKey, AesIv);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            string tmpPath = $"{path}.tmp";
            
            using (FileStream fs = new FileStream(tmpPath, FileMode.Create))
            {
                byte[] writeDataArray = UTF8Encoding.UTF8.GetBytes(jsonValue);
                fs.Write(writeDataArray, 0, writeDataArray.Length);
                fs.Flush();
            }
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            var tmpFileInfo = new FileInfo(tmpPath);
            tmpFileInfo.MoveTo(path);

            return true;
        }

        public static T DeserializeJson<T>(string fileName)
        {
            var path = GetFilePath(fileName);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("DeserializeJson Without Valid Path.");
                return default(T);
            }

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                return default(T);
            }

            using (FileStream stream = fileInfo.OpenRead())
            {
                try
                {
                    if (stream.Length <= 0)
                    {
                        stream.Close();
                        return default(T);
                    }

                    byte[] byteData = new byte[stream.Length];

                    stream.Read(byteData, 0, byteData.Length);

                    string context = UTF8Encoding.UTF8.GetString(byteData);

                    stream.Close();

                    if (string.IsNullOrEmpty(context))
                    {
                        return default(T);
                    }

                    if (IsEncry)
                    {
                        context = EncryptUtil.UnAesStr(context, AesKey, AesIv);
                    }

                    return JsonUtility.FromJson<T>(context);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }

            Debug.LogError("DeserializeJson Failed!");
            return default(T);
        }
        
        public static string GetFilePath(string fileName)
        {
            // if (ENCRY)
            // {
            fileName = fileName.GetHashCode().ToString();
            // }
            return string.Format("{0}{1}", persistentDataPath4Recorder, fileName);
        }
        
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
        
    }
}
