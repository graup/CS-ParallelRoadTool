using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using ICities;
using ParallelRoadTool.Detours;
using ParallelRoadTool.Redirection;
using UnityEngine;

namespace ParallelRoadTool
{
    public class NetTypeItem
    {
        public NetInfo netInfo;
        public float offset;

        public NetTypeItem(NetInfo _netInfo, float _offset)
        {
            netInfo = _netInfo;
            offset = _offset;
        }
    }


    public class ParallelRoadTool : MonoBehaviour
    {
        public const string settingsFileName = "ParallelRoadTool";

        public static ParallelRoadTool instance;

        public static List<NetTypeItem> SelectedRoadTypes = new List<NetTypeItem>();

        public NetTool m_netTool;

        private UIOptionsPanel m_panel;
        private UINetList m_netlist;

        public NetInfo netToolSelection;

        public static bool IsParallelEnabled
        {
            get => NetManagerDetour.IsDeployed();

            set
            {
                if (IsParallelEnabled != value)
                {
                    if (value)
                    {
                        DebugUtils.Log("Enabling parallel road support");
                        NetManagerDetour.Deploy();
                    }
                    else
                    {
                        DebugUtils.Log("Disabling parallel road support");
                        NetManagerDetour.Revert();
                    }
                }
            }
        }

        public void Start()
        {
            var view = UIView.GetAView();
            UIMainWindow window = view.FindUIComponent<UIMainWindow>("PRT_MainWindow");
            if (window != null)
                Destroy(window);

            DebugUtils.Log("Adding UI components");
            window = view.AddUIComponent(typeof(UIMainWindow)) as UIMainWindow;
            m_panel = window.AddUIComponent(typeof(UIOptionsPanel)) as UIOptionsPanel;
            m_netlist = window.AddUIComponent(typeof(UINetList)) as UINetList;
            m_netlist.list = SelectedRoadTypes;
            m_netlist.OnChangedCallback = () =>
            {
                DebugUtils.Log($"m_netlist.OnChangedCallback (selected {SelectedRoadTypes.Count})");
                NetManagerDetour.NetworksCount = SelectedRoadTypes.Count;
            };
            //m_netlist.RenderList();

            UIPanel space = window.AddUIComponent<UIPanel>();
            space.size = new Vector2(1, 1);

            // Find NetTool and deploy
            try
            {
                m_netTool = FindObjectOfType<NetTool>();
                if (m_netTool == null)
                {
                    DebugUtils.Log("Net Tool not found");
                    enabled = false;
                    return;
                }

                NetManagerDetour.Deploy();

                DebugUtils.Log("Initialized");
            }
            catch (Exception e)
            {
                DebugUtils.Log("Start failed");
                DebugUtils.LogException(e);
                enabled = false;
            }
        }

        public void OnDestroy()
        {
            NetManagerDetour.Revert();
            IsParallelEnabled = false;
        }


        public bool IsToolActive()
        {
            return m_panel.m_parallel.isChecked && m_netTool.enabled;
        }

        private void AdjustNetOffset(float step)
        {
            // Adjust all offsets on keypress
            int index = 0;
            foreach (NetTypeItem item in SelectedRoadTypes)
            {
                item.offset += (1+index) * step;
                index++;
            }
            m_netlist.RenderList();
        }

        public void OnGUI()
        {
            try
            {
                if (UIView.HasModalInput() || UIView.HasInputFocus()) return;
                var e = Event.current;

                // Checking key presses
                if (OptionsKeymapping.toggleParallelRoads.IsPressed(e))
                {
                    m_panel.m_parallel.isChecked = !m_panel.m_parallel.isChecked;
                }

                if (OptionsKeymapping.decreaseOffset.IsPressed(e))
                {
                    AdjustNetOffset(-1f);
                }
                if (OptionsKeymapping.increaseOffset.IsPressed(e))
                {
                    AdjustNetOffset(1f);
                }

                var sel = m_netTool.m_prefab;
                if (netToolSelection != sel)
                {
                    netToolSelection = sel;
                    m_netlist.UpdateCurrrentTool(netToolSelection);
                }

            }
            catch (Exception e)
            {
                DebugUtils.Log("OnGUI failed");
                DebugUtils.LogException(e);
            }
        }
    }

    public class ParallelRoadToolLoader : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            // Reload mod if re-created after level has been loaded. For development
            /*if (loading.loadingComplete)
            {
                ParallelRoadTool.instance = new GameObject("ParallelRoadTool").AddComponent<ParallelRoadTool>();
            }*/
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (ParallelRoadTool.instance == null)
                ParallelRoadTool.instance = new GameObject("ParallelRoadTool").AddComponent<ParallelRoadTool>();
            else
                ParallelRoadTool.instance.Start();
        }
    }
}