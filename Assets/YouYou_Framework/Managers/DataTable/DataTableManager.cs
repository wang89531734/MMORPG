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

        public Sys_CodeDBModel Sys_CodeDBModel { get; private set; }
        public Sys_EffectDBModel Sys_EffectDBModel { get; private set; }
        public Sys_PrefabDBModel Sys_PrefabDBModel { get; private set; }
        public Sys_SoundDBModel Sys_SoundDBModel { get; private set; }
        public Sys_StorySoundDBModel Sys_StorySoundDBModel { get; private set; }
        public Sys_UIFormDBModel Sys_UIFormDBModel { get; private set; }
        public LocalizationBModel LocalizationBModel { get; private set; }

        /// <summary>
        /// 章表
        /// </summary>
        public ChapterDBModel ChapterDBModel { get; private set; }

        /// <summary>
        /// 关卡表
        /// </summary>
        public GameLevelDBModel GameLevelDBModel { get; private set; }

        /// <summary>
        /// 任务表
        /// </summary>
        public TaskDBModel TaskDBModel { get; private set; }

        /// <summary>
        /// 初始化DBModel
        /// </summary>
        private void InitDBModel()
        {
            //每个表都new
            Sys_CodeDBModel = new Sys_CodeDBModel();
            Sys_EffectDBModel = new Sys_EffectDBModel();
            Sys_PrefabDBModel = new Sys_PrefabDBModel();
            Sys_SoundDBModel = new Sys_SoundDBModel();
            Sys_StorySoundDBModel = new Sys_StorySoundDBModel();
            Sys_UIFormDBModel = new Sys_UIFormDBModel();
            LocalizationBModel = new LocalizationBModel();

            ChapterDBModel = new ChapterDBModel();
            GameLevelDBModel = new GameLevelDBModel();
            TaskDBModel = new TaskDBModel();
        }

        public void LoadDataTable()
        {
            //每个表都LoadData
            Sys_UIFormDBModel.LoadData();
            Sys_EffectDBModel.LoadData();
            Sys_PrefabDBModel.LoadData();
            Sys_SoundDBModel.LoadData();
            Sys_StorySoundDBModel.LoadData();
            Sys_UIFormDBModel.LoadData();
            LocalizationBModel.LoadData();

            ChapterDBModel.LoadData();
            GameLevelDBModel.LoadData();
            TaskDBModel.LoadData();

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
            Sys_UIFormDBModel.Clear();
            Sys_EffectDBModel.Clear();
            Sys_PrefabDBModel.Clear();
            Sys_SoundDBModel.Clear();
            Sys_StorySoundDBModel.Clear();
            Sys_UIFormDBModel.Clear();
            LocalizationBModel.Clear();

            ChapterDBModel.Clear();
            GameLevelDBModel.Clear();
            TaskDBModel.Clear();
        }
    }
}
