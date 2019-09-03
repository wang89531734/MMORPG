using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YouYou
{
    /// <summary>
    /// 类对象池
    /// </summary>
    public class ClassObjectPool:IDisposable
    {
        /// <summary>
        /// 类对象在池中的常驻数量
        /// </summary>
        public Dictionary<int, byte> ClassObjectCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 类对象池字典
        /// </summary>
        private Dictionary<int, Queue<object>> m_ClassObjectPoolDic;

#if UNITY_EDITOR
        /// <summary>
        /// 在监视面板显示的信息
        /// </summary>
        public Dictionary<Type,int> InspectorDic=new Dictionary<Type, int>();
#endif

        public ClassObjectPool()
        {
            m_ClassObjectPoolDic = new Dictionary<int, Queue<object>>();
            ClassObjectCount = new Dictionary<int, byte>();
        }

        #region SetResideCount 设置类常驻数量
        /// <summary>
        /// 设置类常驻数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public void SetResideCount<T>(byte count) where T : class
        {
            //先找到这个类的哈希码
            int key = typeof(T).GetHashCode();
            ClassObjectCount[key] = count;
        }
        #endregion

        #region Dequeue 取出一个对象
        /// <summary>
        /// 取出一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Dequeue<T>() where T : class, new()
        {
            lock (m_ClassObjectPoolDic)
            {
                //先找到这个类的哈希码
                int key = typeof(T).GetHashCode();

                Queue<object> queue = null;
                m_ClassObjectPoolDic.TryGetValue(key, out queue);

                if (queue == null)
                {
                    queue = new Queue<object>();
                    m_ClassObjectPoolDic[key] = queue;
                }

                //开始获取对象
                if (queue.Count > 0)
                {
                    //说明队列中 有闲置的
                    object obj = queue.Dequeue();
#if UNITY_EDITOR
                    Type t = obj.GetType();
                    if (InspectorDic.ContainsKey(t))
                    {
                        InspectorDic[t]--;
                    }
                    else
                    {
                        InspectorDic[t] = 0;
                    }
#endif
                    return (T)obj;
                }
                else
                {
                    //如果队列中没有 才实例化一个
                    return new T();
                }
            }
        }     
        #endregion

        #region Enqueue 对象回池
        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="obj"></param>
        public void Enqueue(object obj)
        {
            lock (m_ClassObjectPoolDic)
            {
                int key = obj.GetType().GetHashCode();

                Queue<object> queue = null;
                m_ClassObjectPoolDic.TryGetValue(key, out queue);

#if UNITY_EDITOR
                Type t = obj.GetType();
                if (InspectorDic.ContainsKey(t))
                {
                    InspectorDic[t]++;
                }
                else
                {
                    InspectorDic[t] = 1;
                }
#endif


                if (queue != null)
                {
                    queue.Enqueue(obj);
                }
            }
        }
        #endregion

        /// <summary>
        /// 释放类对象池
        /// </summary>
        public void Clear()
        {
            lock (m_ClassObjectPoolDic)
            {
                int queueCount = 0;

                //1.定义迭代器
                var enumerator = m_ClassObjectPoolDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int key = enumerator.Current.Key;

                    Queue<object> queue = m_ClassObjectPoolDic[key];
#if UNITY_EDITOR
                    Type t = null;
#endif
                    queueCount = queue.Count;

                    byte resideCount = 0;
                    ClassObjectCount.TryGetValue(key, out resideCount);
                    while (queueCount > resideCount)
                    {
                        queueCount--;
                        object obj = queue.Dequeue();
#if UNITY_EDITOR
                        t = obj.GetType();
                        InspectorDic[t]--;
#endif
                    }

                    if (queueCount == 0)
                    {
#if UNITY_EDITOR
                        if (t != null)
                        {
                            InspectorDic.Remove(t);
                        }
#endif
                    }
                }
                GC.Collect();
            }           
        }

        public void Dispose()
        {
            m_ClassObjectPoolDic.Clear();
        }
    }
}
