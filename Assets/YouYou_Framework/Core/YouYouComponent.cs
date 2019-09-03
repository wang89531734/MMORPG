using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouYou
{
    /// <summary>
    /// YouYou的组件基类
    /// </summary>
    public class YouYouComponent : MonoBehaviour
    {
        /// <summary>
        /// 组件实例编号
        /// </summary>
        private int m_InstanceId;

        private void Awake()
        {
            m_InstanceId = GetInstanceID();

            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        public int InstanceId
        {
            get { return m_InstanceId; }
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
    }
}
