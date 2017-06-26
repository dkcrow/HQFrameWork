//=====================================================================================/
///<summary>
///wending
///主题 有多少个basedata类型 就会有多少主题
///<summary>
//=====================================================================================/
namespace HQFrameWork
{
    using System.Collections.Generic;

    public class DataSubjectManager
    {
        protected class Subject : ObservableSubjectTemplate<BaseData, int, object>
        { }

        private Dictionary<DataType, Entry> m_subjectDic = new Dictionary<DataType, Entry>();
        private static DataSubjectManager instance;
        private DataSubjectManager() { }
        public static DataSubjectManager Instance
        {
            get
            {

                if (null == instance) instance = new DataSubjectManager();
                return instance;
            }
        }
        private class Entry
        {
            public DataType dataType;
            public Subject subject = new Subject();

            public Entry(DataType type)
            {
                dataType = type;
            }
        }

        /// <summary>
        /// 增加数据监听,一定记得销毁时候移除监听！！！！！！！！！！！！！！！！！！！！！
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="observer"></param>
        public void AddOnChangedCallback(DataType dataType, IObserver observer)
        {
            Entry en = null;
            if (!m_subjectDic.ContainsKey(dataType))
            {
                en = new Entry(dataType);
                m_subjectDic[dataType] = en;
            }
            m_subjectDic[dataType].subject.Attach(observer.OnDataChange);
        }

        public void RemoveOnChangedCallback(DataType dataType, IObserver observer)
        {
            if (m_subjectDic.ContainsKey(dataType))
            {
                m_subjectDic[dataType].subject.Detach(observer.OnDataChange);
            }
        }
        public static BaseData GetData(string dataName)
        {
            return BaseData.GetData(dataName);
        }
        public static T GetData<T>() where T : BaseData, new()
        {
            return BaseData.GetData<T>();
        }

        public void Notify(BaseData data, int type = 0, object obj = null)
        {
            if (m_subjectDic.ContainsKey(data.dataType))
            {
                m_subjectDic[data.dataType].subject.Notify(data, type, obj);
            }
        }
        public void ClearAllData()
        {
            BaseData.ClearAllData();
        }
    }
}