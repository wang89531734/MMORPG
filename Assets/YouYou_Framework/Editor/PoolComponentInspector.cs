using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YouYou
{
    [CustomEditor(typeof(PoolComponent), true)]
    public class PoolComponentInspector : Editor
    {
        //释放间隔 属性
        private SerializedProperty m_ClearInterval = null;

        //释放间隔 属性
        private SerializedProperty m_GameObjectPoolGroups = null;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PoolComponent component = base.target as PoolComponent;

            //绘制滑动条
            int clearInterval = (int)EditorGUILayout.Slider("清空类对象池间隔", m_ClearInterval.intValue, 10, 1800);
            if (clearInterval != m_ClearInterval.intValue)
            {
                component.m_ClearInterval = clearInterval;
            }
            else
            {
                m_ClearInterval.intValue = clearInterval;
            }

            //=====================类对象池开始===========================
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("类名");
            GUILayout.Label("池中数量", GUILayout.Width(50));
            GUILayout.Label("常驻数量", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            if (component != null && component.PoolManager != null)
            {
                foreach (var item in component.PoolManager.ClassObjectPool.InspectorDic)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(item.Key.Name);
                    GUILayout.Label(item.Value.ToString(), GUILayout.Width(50));

                    int key = item.Key.GetHashCode();
                    byte resideCount = 0;
                    component.PoolManager.ClassObjectPool.ClassObjectCount.TryGetValue(key, out resideCount);

                    GUILayout.Label(resideCount.ToString(), GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            //=====================类对象池结束===========================

            //=====================变量计数开始===========================
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("变量");
            GUILayout.Label("数量", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            if (component != null)
            {
                foreach (var item in component.VarObjectInspectorDic)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(item.Key.Name);
                    GUILayout.Label(item.Value.ToString(), GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            //=====================变量计数结束===========================

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(m_GameObjectPoolGroups, true);

            serializedObject.ApplyModifiedProperties();
            //重绘
            Repaint();
        }

        private void OnEnable()
        {
            //建立属性关系
            m_ClearInterval = serializedObject.FindProperty("m_ClearInterval");
            m_GameObjectPoolGroups = serializedObject.FindProperty("m_GameObjectPoolGroups");
            serializedObject.ApplyModifiedProperties();
        }
    }
}