using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace YouYou
{
    public class DataTableManager : ManagerBase
    {
        public DataTableManager()
        {
            InitDBModel();
        }

        /// <summary>
        /// 章表
        /// </summary>
        public ChapterDBModel ChapterDBModel { get; private set; }

        /// <summary>
        /// 关卡表
        /// </summary>
        public GameLevelDBModel GameLevelDBModel { get; private set; }

        /// <summary>
        /// 初始化DBModel
        /// </summary>
        private void InitDBModel()
        {
            //每个表都new
            ChapterDBModel = new ChapterDBModel();
            GameLevelDBModel = new GameLevelDBModel();
        }

        public void LoadDataTable()
        {
            //每个表都LoadData
            ChapterDBModel.LoadData();

            //所有表Load完毕
            GameEntry.Event.CommonEvent.Dispatch(SysEventId.LoadDataTableComplete);
        }

        /// <summary>
        /// 异步加载表格
        /// </summary>
        public void LoadDataTableAsync()
        {
            Task.Factory.StartNew(LoadDataTable);
        }

        public void Clear()
        {
            //每个表都Clear
            ChapterDBModel.Clear();
        }
    }
}
