using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// Æô¶¯Á÷³Ì
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
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
