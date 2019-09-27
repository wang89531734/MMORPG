using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YouYou
{
    /// <summary>
    /// 场景加载和卸载器
    /// </summary>
    public class SceneLoaderRoutine
    {
        private AsyncOperation m_CurrAsync = null;

        /// <summary>
        /// 进度更新
        /// </summary>
        private BaseAction<int, float> OnProgressUpdate;

        /// <summary>
        /// 加载场景完毕
        /// </summary>
        private BaseAction<SceneLoaderRoutine> OnLoadSceneComplete;

        /// <summary>
        /// 卸载场景完毕
        /// </summary>
        private BaseAction<SceneLoaderRoutine> OnUnLoadSceneComplete;

        /// <summary>
        /// 场景明细编号
        /// </summary>
        private int m_SceneDetailId;

        public void LoadScene(int sceneDetailId,string sceneName,BaseAction<int,float> onProgressUpdate,BaseAction<SceneLoaderRoutine> onLoadSceneComplete)
        {
            Reset();

            m_SceneDetailId = sceneDetailId;
            OnProgressUpdate = onProgressUpdate;
            OnLoadSceneComplete = onLoadSceneComplete;

            //GameEntry.Resource.res
        }

        private void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
