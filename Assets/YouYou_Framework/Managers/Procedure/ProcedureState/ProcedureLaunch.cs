using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YouYou
{
    /// <summary>
    /// 启动流程
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)//先是片头场景然后加载标题场景
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
            //GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
            Debug.Log("执行启动流程");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
