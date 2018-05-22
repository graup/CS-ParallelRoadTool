using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using ParallelRoadTool.Detours;
using ParallelRoadTool.Redirection;
using ParallelRoadTool.UI;
using ParallelRoadTool.UI.Base;
using UnityEngine;

namespace ParallelRoadTool
{
    public class UINetList : UIPanel
    {
        public List<NetTypeItem> list;
        private List<UINetTypeItem> m_items;
        private UINetTypeItem m_currentTool;

        public Action OnChangedCallback { private get; set; }

        private UIPanel m_space;

        public override void Start()
        {
            name = "PRT_NetList";
            padding = new RectOffset(4, 4, 4, 0);
            size = new Vector2(450 - 8*2, 200);
            autoLayoutPadding = new RectOffset(0, 0, 0, 4);
            autoFitChildrenVertically = true;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            backgroundSprite = "GenericPanel";
            color = Color.black;

            m_items = new List<UINetTypeItem>();

            m_currentTool = AddUIComponent<UINetTypeItem>();
            m_currentTool.isCurrentItem = true;
            m_currentTool.OnAddCallback = () =>
            {
                DebugUtils.Log("Adding item to list");

                // get sum of current offsets
                float prevOffset = 0;
                if (list.Count() > 0)
                    prevOffset = list.Last().offset;

                NetInfo netInfo = PrefabCollection<NetInfo>.FindLoaded(m_currentTool.netInfo.name);
                DebugUtils.Log($"{m_currentTool.netInfo} halfWidth: {m_currentTool.netInfo.m_halfWidth}");
                NetTypeItem item = new NetTypeItem(netInfo, prevOffset + netInfo.m_halfWidth * 2);
                list.Add(item);

                RenderList();

                Changed();
            };

            m_space = AddUIComponent<UIPanel>();
            m_space.size = new Vector2(1, 1);
        }

        public void UpdateCurrrentTool(NetInfo tool)
        {
            m_currentTool.netInfo = tool;
            m_currentTool.RenderItem();
        }

        public void Changed()
        {
            OnChangedCallback?.Invoke();
        }

        public void RenderList()
        {
            // Remove items
            foreach (UINetTypeItem child in m_items)
            {
                Destroy(child);
            }

            m_items.Clear();

            // Add items
            int index = 0;
            foreach (NetTypeItem item in list)
            {
                DebugUtils.Log($"rendering item {index} {item.netInfo} at {item.offset}");
                var comp = AddUIComponent<UINetTypeItem>();
                comp.netInfo = item.netInfo;
                comp.offset = item.offset;
                comp.index = index++;
                //comp.RenderItem();

                comp.OnDeleteCallback = () =>
                {
                    // remove item from list
                    list.RemoveAt(comp.index);
                    RenderList();
                    Changed();
                };

                comp.OnChangedCallback = () =>
                {
                    //item.offset = comp.offset;
                    NetTypeItem i = list[comp.index];
                    i.offset = comp.offset;
                    DebugUtils.Message($"OnChangedCallback {comp.index} now at {item.offset}");
                    Changed();
                };

                m_items.Add(comp);
            }

            m_space.BringToFront();


        }
    }
}
