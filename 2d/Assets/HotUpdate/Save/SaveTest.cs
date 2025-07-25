using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectX
{
    [Serializable]
    public class TestInnerData
    {
        public int type;
    }

    [Serializable]
    public class TestData
    {
        public int TestInt;
        public float TestFloat;
        public string TestString;
        public DateTimeOffset TestDateTimeOffset;
        public Dictionary<string, TestInnerData> TestDictionary;
        public List<TestInnerData> TestList;
    }

    public class SaveTest : MonoBehaviour
    {
        // [SerializeField]
        public TestData TestData;

        private void Start()
        {
            Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        }

        private void OnGUI()
        {
            // 创建自定义样式
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = 50; // 设置文字大小

            if (GUI.Button(new Rect(100, 100, 500, 300), "Save", style))
            {
                SerializeHelper.SerializeJson("TestData", TestData);
                Debug.Log($"save complete {JsonUtility.ToJson(TestData)}");
            }

            if (GUI.Button(new Rect(100, 500, 500, 300),"Load", style))
            {
                TestData = SerializeHelper.DeserializeJson<TestData>("TestData");
                Debug.Log($"load complete {JsonUtility.ToJson(TestData)}");
            }
        }
    }
}