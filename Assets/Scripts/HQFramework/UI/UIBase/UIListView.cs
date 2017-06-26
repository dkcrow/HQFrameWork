namespace HQFrameWork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    //QKUGUIListView,isReUse为true可复用模式，只能添加一种prefab，为false是可以添加不同种类的prefab
    public class UIListView : UGUICtrlBase
    {
        #region 公用部分

        public bool isReUse = true;
        public ScrollRect scrollView;
        private RectTransform content;

        public enum Arrangement
        {
            Horizontal,   //水平滑动
            Vertical,       //垂直滑动
        }

        public Arrangement arrangement = Arrangement.Vertical;    //排序方式
        public Vector2 spacingSize = new Vector2(10, 10);   //网格间隙

        /// <summary>
        /// 用来存放当前选中状态的item 的Index，改变状态时候尽量少遍历,复用的时候用Index
        /// </summary>
        public int nowSelectedItemIndex = 0;


        /// <summary>
        /// 用来存放当前选中状态的ite ,改变状态时候尽量少遍历
        /// </summary>
        public UIListViewItem nowSelectedItem;

        #endregion 公用部分

        #region 复用式

        private Vector2 cellSize = new Vector2(150, 150);    //网格大小
        private int maxViewCount = 5; //最大可见行数
        private int perLineNum = 2;        //每行个数
        private List<UIListViewItem> listItemReUse = new List<UIListViewItem>();
        private Queue<UIListViewItem> unUseItemReUse = new Queue<UIListViewItem>();

        public delegate void OnUpdateItem(UIListViewItem go, int dataIndex);

        public OnUpdateItem onUpdateItem;
        private int nowDataCount = 0;
        private int curScrollPerLineIndex = -1;  //当前开始显示行或者列的index

        #endregion 复用式

        #region 非复用式

        private float contentLength = 0;        //网格最大长度，vetival 为width，hor为height
        private List<RectTransform> listItemNormal = new List<RectTransform>();   //存显示的item
        private Dictionary<string, List<RectTransform>> itemPool = new Dictionary<string, List<RectTransform>>();
        private RectTransform poolRectTrans;
        private float maxPosNum = 0;   //元素中最大坐标值

        #endregion 非复用式

        public override void Init()
        {
            scrollView = GetComponent<ScrollRect>();
            content = transform.FindChild("Viewport/Content").GetComponent<RectTransform>();
            if (!isReUse)
            {
                //非复用式，创建一个用来装移除的item的recttransform
                GameObject go = new GameObject();
                poolRectTrans = go.AddComponent<RectTransform>();
                poolRectTrans.SetParent(transform,false);
                poolRectTrans.gameObject.SetActive(false);
                poolRectTrans.name = "ItemPool";
                poolRectTrans.localScale = Vector3.one;
                poolRectTrans.anchoredPosition3D = Vector3.zero;
            }
            if (arrangement == Arrangement.Vertical)
            {
                scrollView.vertical = true;
                scrollView.horizontal = false;
                content.anchorMin = new Vector2(0, 1);
                content.anchorMax = new Vector2(0, 1);
                content.offsetMin = Vector2.zero;
                content.offsetMax = Vector2.zero;
                content.pivot = new Vector2(0, 1);
                content.anchoredPosition3D = Vector3.zero;
                if (!isReUse) contentLength = GetComponent<RectTransform>().sizeDelta.x;
            }
            else if (arrangement == Arrangement.Horizontal)
            {
                scrollView.vertical = false;
                scrollView.horizontal = true;
                content.anchorMin = new Vector2(0, 1);
                content.anchorMax = new Vector2(0, 1);
                content.offsetMin = Vector2.zero;
                content.offsetMax = Vector2.zero;
                content.pivot = new Vector2(0, 1);
                content.anchoredPosition3D = Vector3.zero;
                if (!isReUse) contentLength = GetComponent<RectTransform>().sizeDelta.y;
            }
            scrollView.onValueChanged.AddListener(OnValueChanged);
        }

        #region 公共方法

        private void OnValueChanged(Vector2 value)
        {
            if (!isReUse) return;
            int curIndex = 0;
            if (arrangement == Arrangement.Horizontal)
            {
                if (value.x <= 0)       //解决边界问题大于1一直也会刷新
                {
                    curIndex = 0;
                }
                else if (value.x >= 1)
                {
                    curIndex = Mathf.CeilToInt((float)nowDataCount / (float)perLineNum) - maxViewCount;
                }
                else
                {
                    curIndex = getCurScrollPerLineIndex();
                }
            }
            else if (arrangement == Arrangement.Vertical)
            {
                if (value.y >= 1)
                {
                    curIndex = 0;
                }
                else if (value.y <= 0)
                {
                    curIndex = Mathf.CeilToInt((float)nowDataCount / (float)perLineNum) - maxViewCount;
                }
                else
                {
                    curIndex = getCurScrollPerLineIndex();
                }
            }

            SetUpdateItemForUse(curIndex);
        }

        /// <summary>
        /// 设置添加物体的锚点和对齐锚点
        /// </summary>
        /// <param name="rectTrans"></param>
        private void SetAnchorAndPiovt(RectTransform rectTrans)
        {
            rectTrans.pivot = new Vector2(0, 1);
            rectTrans.anchorMin = new Vector2(0, 1);
            rectTrans.anchorMax = new Vector2(0, 1);
        }

        /// <summary>
        /// 删除当前数据索引下数据
        /// </summary>
        /// <param name="dataIndex"></param>
        public void DeleteItem(int index)
        {
            if (isReUse)
            {
                if (index < 0 || index >= nowDataCount)
                {
                    return;
                }
                setDataCount(nowDataCount - 1);

                for (int i = listItemReUse.Count - 1; i >= 0; i--)
                {
                    UIListViewItem item = listItemReUse[i];
                    int oldIndex = item.Index;
                    item.OnRemove();
                    if (oldIndex == index)
                    {
                        listItemReUse.Remove(item);
                        item.Index = -1;
                        unUseItemReUse.Enqueue(item);
                    }
                    if (oldIndex > index)
                    {
                        item.Index = oldIndex - 1;
                    }
                }
                SetUpdateItemForUse(getCurScrollPerLineIndex());
            }
            else
            {
                if (index >= listItemNormal.Count)
                {
                    Debug.Log("ListBox.cs: 删除元素数组越界请检查!!!!");
                    return;
                }
                RectTransform rect = listItemNormal[index];       
                listItemNormal.RemoveAt(index);
                rect.SetParent(poolRectTrans);
                UIListViewItem itemTemp = rect.GetComponent<UIListViewItem>();
                itemTemp.OnRemove();
                itemPool[GetGameObjectName(itemTemp.gameObject)].Add(rect);
            }
        }

        /// <summary>
        /// 移除所有元素
        /// </summary>
        public void RemoveAll()
        {
            if (null != nowSelectedItem) nowSelectedItem.SetChecked(false);
            nowSelectedItem = null;
            // content.anchoredPosition = new Vector2(0,0);
            if (isReUse)
            {
                setDataCount(0);
                for (int i = listItemReUse.Count - 1; i >= 0; i--)
                {
                    UIListViewItem temp = listItemReUse[i];
                    temp.OnRemove();
                    listItemReUse.RemoveAt(i);
                    unUseItemReUse.Enqueue(temp);
                    temp.gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = listItemNormal.Count - 1; i >= 0; i--)
                {
                    DeleteItem(i);
                }
            }
        }

        /// <summary>
        /// 设置ListView位置
        /// </summary>
        /// <param name="percent">0-1</param>
        public void SetViewPosition(float percent)
        {
            if (percent <= 0) percent = 0.000001f; if (percent >= 1) percent = 0.99999f;   //防止边界刷新问题
            if (arrangement == Arrangement.Vertical)
            {
                float posY = percent * (content.sizeDelta.y - GetComponent<RectTransform>().sizeDelta.y) - 1;
                if (posY <= 0) posY = 0.01f;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, posY);
                OnValueChanged(new Vector2(0, percent));
            }
            else if (arrangement == Arrangement.Horizontal)
            {
                float posX = percent * (content.sizeDelta.x - GetComponent<RectTransform>().sizeDelta.x) - 1;
                if (posX < 0) posX = 0.01f;
                content.anchoredPosition = new Vector2(posX, content.anchoredPosition.y);
                OnValueChanged(new Vector2(percent, 0));
            }
        }
        /// <summary>
        /// 获取listview百分比
        /// </summary>
        /// <seealso cref="SetViewPosition"/>
        /// <returns></returns>
        public float GetViewPosition()
        {
            if (arrangement == Arrangement.Vertical)
            {
                float deltaY = content.sizeDelta.y - GetComponent<RectTransform>().sizeDelta.y;
                if (deltaY == 0) return 0;
                return content.anchoredPosition.y/ deltaY;
            }
            else if (arrangement == Arrangement.Horizontal)
            {
                float deltaX = content.sizeDelta.x - GetComponent<RectTransform>().sizeDelta.x;
                if (deltaX == 0) return 0;
                return content.anchoredPosition.x/deltaX;
            }
            return 0;
        }
        /// <summary>
        /// 设置content位置
        /// </summary>
        /// <param name="anchoredPos"></param>
        public void SetViewPositionForUnResuse(Vector2 pos)
        {
            content.anchoredPosition = pos;
        }

        public RectTransform GetContent()
        {
            return content;
        }

        #endregion 公共方法

        #region 复用式

        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="curIndex">当前开始显示行</param>
        private void SetUpdateItemForUse(int curIndex)
        {
            if (curIndex < 0)
            {
                return;
            }
            curScrollPerLineIndex = curIndex;
            int startDataIndex = curScrollPerLineIndex * perLineNum;
            int endDataIndex = (curScrollPerLineIndex + maxViewCount) * perLineNum;
            //移除区域之外的item
            for (int i = listItemReUse.Count - 1; i >= 0; i--)
            {
                UIListViewItem item = listItemReUse[i];
                int index = item.Index;

                if (index < startDataIndex || index >= endDataIndex)
                {
                    item.Index = -1;
                    listItemReUse.Remove(item);
                    unUseItemReUse.Enqueue(item);
                    item.gameObject.SetActive(false);
                }
            }
            //显示需要显示的item
            for (int i = startDataIndex; i < endDataIndex; i++)
            {
                if (i >= nowDataCount)
                {
                    continue;
                }
                if (isExistingData(i))
                {
                    continue;
                }
                AddItemForReUse(i);
            }
        }

        public void UpdateListBoxForReUse()
        {
            for (int i = 0; i < listItemReUse.Count; i++)
            {
                if (null != listItemReUse[i])
                {
                    listItemReUse[i].Index = listItemReUse[i].Index;
                }
            }
        }

        /// <summary>
        /// 是否已经存在该条数据item
        /// </summary>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        private bool isExistingData(int dataIndex)
        {
            if (listItemReUse == null || listItemReUse.Count <= 0)
            {
                return false;
            }
            for (int i = 0; i < listItemReUse.Count; i++)
            {
                if (listItemReUse[i].Index == dataIndex)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 生成需要的最大数量物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public void InitItemForReUse<T>(GameObject go) where T : UIListViewItem
        {
            if (go.activeSelf) go.SetActive(false);
            //计算需要的最大可见数量，行数，网格大小等
            RectTransform rectT = GetComponent<RectTransform>();
            cellSize = go.GetComponent<RectTransform>().sizeDelta;
            if (arrangement == Arrangement.Vertical)
            {
                maxViewCount = Mathf.CeilToInt(rectT.sizeDelta.y / (spacingSize.y + cellSize.y)) + 1;  //最大可见行数
                perLineNum = (spacingSize.x + cellSize.x) > rectT.sizeDelta.x ? 1 : Mathf.FloorToInt(rectT.sizeDelta.x / (spacingSize.x + cellSize.x));        //每行个数
            }
            else if (arrangement == Arrangement.Horizontal)
            {
                maxViewCount = Mathf.CeilToInt(rectT.sizeDelta.x / (spacingSize.x + cellSize.x)) + 1;  //最大可见行数
                perLineNum = (spacingSize.y + cellSize.y) > rectT.sizeDelta.y ? 1 : Mathf.FloorToInt(rectT.sizeDelta.y / (spacingSize.y + cellSize.y));        //每行个数
            }
            //初始化需要的最大数量item
            for (int i = 0; i < maxViewCount * perLineNum; i++)
            {
                UIListViewItem temp = Instantiate(go).AddComponent<T>();
                temp.listBox = this;
                SetAnchorAndPiovt(temp.RectTrans);
                temp.RectTrans.SetParent(content, false);
                temp.gameObject.SetActive(false);
                temp.Init();
                unUseItemReUse.Enqueue(temp);
            }
        }

        private void AddItemForReUse(int index)
        {
            UIListViewItem temp = null;
            if (unUseItemReUse.Count > 0)
            {
                temp = unUseItemReUse.Dequeue();
            }
            listItemReUse.Add(temp);
            temp.Index = index;
            temp.gameObject.SetActive(true);
            SetContentSize();
        }

        public UIListViewItem AddItemForReUse()
        {
            if (unUseItemReUse.Count <= 0)
            {
                setDataCount(nowDataCount + 1);
                return null;
            }
            UIListViewItem temp = null;
            temp = unUseItemReUse.Dequeue();
            listItemReUse.Add(temp);
           // temp.listBox = this;
            temp.Index = nowDataCount;
            temp.OnAdd();
            temp.gameObject.SetActive(true);
            setDataCount(nowDataCount + 1);
            return temp;
        }

        /// <summary>
        /// 刷新复用对话框单个元素
        /// </summary>
        /// <param name="index"></param>
        public void UpdateItemForReUseSingle(int index)
        {
            if (index < 0 || index > nowDataCount)
            {
                return;
            }
            UIListViewItem item = listItemReUse[index];
            item.Index = item.Index;
        }

        public void InsterItemForReUse(int Index)
        {
            if (Index < 0 || Index > nowDataCount)
            {
                return;
            }
            //检测是否需添加gameObject
            bool isNeedAdd = false;
            for (int i = listItemReUse.Count - 1; i >= 0; i--)
            {
                UIListViewItem item = listItemReUse[i];
                if (item.Index >= (nowDataCount - 1))
                {
                    isNeedAdd = true;
                    break;
                }
            }
            setDataCount(nowDataCount + 1);
            if (isNeedAdd)
            {
                for (int i = 0; i < listItemReUse.Count; i++)
                {
                    UIListViewItem item = listItemReUse[i];
                    int oldIndex = item.Index;
                    if (oldIndex >= Index)
                    {
                        item.Index = oldIndex + 1;
                    }
                    item = null;
                }
                SetUpdateItemForUse(getCurScrollPerLineIndex());
            }
            else
            {
                //重新刷新数据
                for (int i = 0; i < listItemReUse.Count; i++)
                {
                    UIListViewItem item = listItemReUse[i];
                    int oldIndex = item.Index;
                    if (oldIndex >= Index)
                    {
                        item.Index = oldIndex;
                    }
                    item = null;
                }
            }
        }

        /// <summary>
        /// 设置数据数量
        /// </summary>
        /// <param name="count"></param>
        public void setDataCount(int count)
        {
            if (nowDataCount == count)
            {
                return;
            }
            nowDataCount = count;
            SetContentSize();
        }

        /// <summary>
        /// 设置content大小
        /// </summary>
        private void SetContentSize()
        {
            Vector2 sizeDelta = content.sizeDelta;
            sizeDelta.y = Mathf.CeilToInt((float)nowDataCount / (float)perLineNum) * (cellSize.y + spacingSize.y);
            content.sizeDelta = sizeDelta;

            int lineCount = Mathf.CeilToInt((float)nowDataCount / (float)perLineNum);
            switch (arrangement)
            {
                case Arrangement.Horizontal:
                    content.sizeDelta = new Vector2(cellSize.x * lineCount + spacingSize.x * (lineCount - 1), content.sizeDelta.y);
                    break;

                case Arrangement.Vertical:
                    content.sizeDelta = new Vector2(content.sizeDelta.x, cellSize.y * lineCount + spacingSize.y * (lineCount - 1));
                    break;
            }
        }

        public void SetContentSize(Vector2 size)
        {
            content.sizeDelta = size;
        }

        public Vector2 GetContentSize()
        {
            return content.sizeDelta;
        }

        /// <summary>
        ///  获得非复用item列表
        /// </summary>
        /// <returns></returns>
        public List<RectTransform> GetNormalItemList()
        {
            return listItemNormal;
        }

        /// <summary>
        /// 获取当前元素个数
        /// </summary>
        /// <returns></returns>
        public int GetNowItemNum()
        {
            if (isReUse)
            {
                return nowDataCount;
            }
            return listItemNormal.Count;
        }

        /// <summary>
        /// 设置元素坐标
        /// </summary>
        /// <param name="temp"></param>
        public void SetItemPostionForReUse(UIListViewItem temp)
        {
            float thisLineNum = temp.Index % perLineNum;
            float x = 0;
            float y = 0;
            switch (arrangement)
            {
                case Arrangement.Horizontal: //水平方向
                    x = Mathf.FloorToInt((float)temp.Index / (float)perLineNum) * (cellSize.x + spacingSize.x);
                    y = -thisLineNum * (cellSize.y + spacingSize.y);
                    break;

                case Arrangement.Vertical://垂着方向
                    x = thisLineNum * (cellSize.x + spacingSize.x);
                    y = -Mathf.FloorToInt((float)temp.Index / perLineNum) * (cellSize.y + spacingSize.y);
                    break;
            }
            temp.RectTrans.anchoredPosition3D = new Vector3(x, y, 0);
        }

        /// <summary>
        /// 获取当前元素坐标
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public Vector3 GetItemPostion(UIListViewItem temp)
        {
            return GetItemPositionWithIndexForReUse(temp.Index);
        }

        /// <summary>
        /// 通过数据id取得该条数据应该存在的位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetItemPositionWithIndexForReUse(int index)
        {
            float thisLineNum = index % perLineNum;
            float x = 0;
            float y = 0;
            switch (arrangement)
            {
                case Arrangement.Horizontal: //水平方向
                    x = Mathf.FloorToInt((float)index / (float)perLineNum) * (cellSize.x + spacingSize.x);
                    y = -thisLineNum * (cellSize.y + spacingSize.y);
                    break;

                case Arrangement.Vertical://垂着方向
                    x = thisLineNum * (cellSize.x + spacingSize.x);
                    y = -Mathf.FloorToInt((float)index / (float)perLineNum) * (cellSize.y + spacingSize.y);
                    break;
            }
            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// 根据Content偏移,计算当前开始显示所在数据列表中的行或列
        /// </summary>
        /// <returns></returns>
        private int getCurScrollPerLineIndex()
        {
            switch (arrangement)
            {
                case Arrangement.Horizontal: //水平方向
                    return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.x) / (cellSize.x + spacingSize.x));

                case Arrangement.Vertical://垂着方向
                    return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.y) / (cellSize.y + spacingSize.y));
            }
            return 0;
        }

        #endregion 复用式

        #region 非复用式
        public UIListViewItem AddItemForUnReUseForLua(GameObject prefab)
        {
           return AddItemForUnReUse<UIListViewItem>(prefab);
        }
        /// <summary>
        /// 增加item绑定脚本T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddItemForUnReUse<T>(GameObject prefab) where T : UIListViewItem
        {
            if (prefab.activeSelf) prefab.SetActive(false);
            if (isReUse)
            {
                //GameDebugLog.LogError("你的ListBox是Reuse类型，这个方式是UnReuse使用，请检查！！！！！");
            }
            T t = AddItemFromPoolForUnReUse<T>(GetGameObjectName(prefab.gameObject));
            if (null == t)
            {
                t = AddItemFromPrefabForUnReUse(prefab).gameObject.AddComponent<T>();
                t.listBox = this;
                t.Init();
            }
            //如果回收池中没有先初始化该类型
            if (!itemPool.ContainsKey(GetGameObjectName(t.gameObject)))
            {
                itemPool[GetGameObjectName(t.gameObject)] = new List<RectTransform>();
            }
            t.OnAdd();
            RectTransform go = t.GetComponent<RectTransform>();
            go.SetParent(content, false);
            SetAnchorAndPiovt(go);
            SetPosForUnReUse(go);
            AddItemToListForUnReUse(go);
            t.gameObject.SetActive(true);
            return t;
        }

        /// <summary>
        /// 克隆prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private GameObject AddItemFromPrefabForUnReUse(GameObject prefab)
        {
            RectTransform go = Instantiate(prefab).GetComponent<RectTransform>();

            return go.gameObject;
        }

        /// <summary>
        /// 从池中取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T AddItemFromPoolForUnReUse<T>(string t) where T : UIListViewItem
        {
            if (!itemPool.ContainsKey(t) || itemPool[t].Count == 0)
            {
                return null;
            }
            RectTransform go = null;
            if (itemPool[t].Count > 0)
            {
                go = itemPool[t][itemPool[t].Count - 1];
                itemPool[t].RemoveAt(itemPool[t].Count - 1);
            }
            return go.GetComponent<T>();
        }

        private void AddItemToListForUnReUse(RectTransform rectTrans)
        {
            listItemNormal.Add(rectTrans);
        }

        /// <summary>
        /// 设置当前元素位置
        /// </summary>
        /// <param name="rect"></param>
        private void SetPosForUnReUse(RectTransform rect)
        {
            if (arrangement == Arrangement.Vertical)
            {
                SetVertical(rect);
            }
            else
            {
                SetHorizontal(rect);
            }
        }

        /// <summary>
        /// 重新排列位置
        /// </summary>
        public void RePostionForUnReUse()
        {
            List<RectTransform> listTemp = new List<RectTransform>();
            for (int i = 0; i < listItemNormal.Count; i++)
            {
                listTemp.Add(listItemNormal[i]);
            }
            listItemNormal.Clear();
            for (int i = 0; i < listTemp.Count; i++)
            {
                SetPosForUnReUse(listTemp[i]);
                listItemNormal.Add(listTemp[i]);
            }
        }

        private void SetVertical(RectTransform rect)
        {
            if (listItemNormal.Count == 0)
            {
                rect.anchoredPosition3D = new Vector3(0, 0, 0);
                maxPosNum = -rect.sizeDelta.y;
                return;
            }
            if (contentLength - (listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.x + listItemNormal[listItemNormal.Count - 1].sizeDelta.x + spacingSize.x) > rect.sizeDelta.x)
            {
                //本行
                float posX = listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.x + listItemNormal[listItemNormal.Count - 1].sizeDelta.x + spacingSize.x;
                float posY = listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.y;
                rect.anchoredPosition3D = new Vector3(posX, posY, 0);
            }
            else
            {
                //下一行
                float posX = 0;
                float posY = listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.y - listItemNormal[listItemNormal.Count - 1].sizeDelta.y - spacingSize.y;
                rect.anchoredPosition3D = new Vector3(posX, posY < maxPosNum ? posY : maxPosNum - spacingSize.y, 0);
            }
            if (maxPosNum > rect.anchoredPosition3D.y - rect.sizeDelta.y)
            {
                maxPosNum = rect.anchoredPosition3D.y - rect.sizeDelta.y;
            }
            content.sizeDelta = new Vector2(content.sizeDelta.x, -maxPosNum);
        }

        private void SetHorizontal(RectTransform rect)
        {
            if (listItemNormal.Count == 0)
            {
                rect.anchoredPosition3D = new Vector3(0, 0, 0);
                maxPosNum = rect.sizeDelta.x;
                return;
            }
            if (contentLength - (Math.Abs(listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.y) + listItemNormal[listItemNormal.Count - 1].sizeDelta.y + spacingSize.y) > rect.sizeDelta.y)
            {
                //本列
                float posX = listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.x;
                float posY = listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.y - listItemNormal[listItemNormal.Count - 1].sizeDelta.y - spacingSize.y;
                rect.anchoredPosition3D = new Vector3(posX, posY, 0);
            }
            else
            {
                //下一列
                float posX = listItemNormal[listItemNormal.Count - 1].anchoredPosition3D.x + listItemNormal[listItemNormal.Count - 1].sizeDelta.x + spacingSize.x;
                float posY = 0;
                rect.anchoredPosition3D = new Vector3(posX < maxPosNum ? maxPosNum + spacingSize.x : posX, posY, 0);
            }
            if (maxPosNum < rect.anchoredPosition3D.x + rect.sizeDelta.x)
            {
                maxPosNum = rect.anchoredPosition3D.x + rect.sizeDelta.x;
            }
            content.sizeDelta = new Vector2(maxPosNum, content.sizeDelta.y);
        }

        #endregion 非复用式
        /// <summary>
        /// 获取gameobject名称，去除(Clone)
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private string GetGameObjectName(GameObject go)
        {
            return go.name.Replace("(Clone)", "");
        }
        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.LISTVIEW;
            }
        }
    }
}