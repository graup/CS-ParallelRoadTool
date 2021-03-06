﻿using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using ParallelRoadTool.Detours;
using ParallelRoadTool.Redirection;
using ParallelRoadTool.UI;
using ParallelRoadTool.UI.Base;
using UnityEngine;

namespace ParallelRoadTool
{
    public class OptionsPanel : UIPanel
    {
        public UIButton m_addMoreNetworks;
        private UITextureAtlas m_atlas;

        public UIMainWindow m_window;

        public List<UINetTypeItem> m_networks;

        public UICheckBox m_parallel;

        public NetInfo netToolSelection;
        private UILabel m_nettoolSelection;

        public override void Start()
        {
            LoadResources();

            name = "PRT_OptionsPanel";
            atlas = ResourceLoader.GetAtlas("Ingame");
            backgroundSprite = "GenericPanel";
            color = new Color32(206, 206, 206, 255);
            size = new Vector2(400, 52);
            relativePosition = new Vector2(8, 92);

            padding = new RectOffset(8, 8, 8, 8);
            autoLayoutPadding = new RectOffset(0, 4, 0, 0);
            autoLayoutDirection = LayoutDirection.Vertical;

            m_parallel = AddTool("Anarchy", "Toggle parallel road tool test", false);

            m_nettoolSelection = AddUIComponent<UILabel>();
            m_nettoolSelection.name = "PRT_NetToolSelectionLabel";
            m_nettoolSelection.textAlignment = UIHorizontalAlignment.Center;
            m_nettoolSelection.textScale = 1.2f;
            m_nettoolSelection.text = "Select a network";
            m_nettoolSelection.autoSize = false;
            m_nettoolSelection.width = 400 + 8*4;
            m_nettoolSelection.autoHeight = false;
            m_nettoolSelection.height = 60;

            m_addMoreNetworks = CreateAddButton(m_nettoolSelection, "Add", "Add this network to the selection", (c, p) =>
            {
                // TODO: added networks should also be removable or, as for now, once you add another network you can't removed it anymore
                // TODO: UI MUST be fixed before release, current one sucks.
                var selected = new NetTypeItem(netToolSelection, netToolSelection.m_halfWidth);
                ParallelRoadTool.SelectedRoadTypes.Add(selected);
                // TODO: update UI
            });

            m_networks = new List<UINetTypeItem>{
                // TODO: add current selection item
            };

            UpdateOptions();

            autoLayout = true;
        }

        public UICheckBox AddTool(string spriteName, string toolTip, bool value)
        {
            var roadsOptionPanel = UIUtil.FindComponent<UIComponent>("RoadsOptionPanel", null, UIUtil.FindOptions.NameContains);
            if (roadsOptionPanel == null || !roadsOptionPanel.gameObject.activeInHierarchy) return null;

            var checkBox = roadsOptionPanel.AddUIComponent<UICheckBox>();
            checkBox.size = new Vector2(36, 36);

            var button = checkBox.AddUIComponent<UIButton>();
            button.name = "PRT_Tool";
            button.atlas = m_atlas;
            button.tooltip = toolTip;
            button.relativePosition = new Vector3(0, 65);

            button.normalBgSprite = "OptionBase";
            button.hoveredBgSprite = "OptionBaseHovered";
            button.pressedBgSprite = "OptionBasePressed";
            button.disabledBgSprite = "OptionBaseDisabled";

            button.normalFgSprite = spriteName;
            button.hoveredFgSprite = spriteName + "Hovered";
            button.pressedFgSprite = spriteName + "Pressed";
            button.disabledFgSprite = spriteName + "Disabled";

            checkBox.isChecked = value;
            if (value)
            {
                button.normalBgSprite = "OptionBaseFocused";
                button.normalFgSprite = spriteName + "Focused";
            }

            checkBox.eventCheckChanged += (c, s) =>
            {
                if (s)
                {
                    button.normalBgSprite = "OptionBaseFocused";
                    button.normalFgSprite = spriteName + "Focused";
                }
                else
                {
                    button.normalBgSprite = "OptionBase";
                    button.normalFgSprite = spriteName;
                }

                UpdateOptions();
            };

            return checkBox;
        }

        public override void Update()
        {
            /*var sel = ParallelRoadTool.instance.m_netTool.m_prefab;
            if (netToolSelection != sel)
            {
                netToolSelection = sel;
                //m_nettoolSelection.text = netToolSelection.GenerateBeautifiedNetName();
                //m_nettoolSelection.
                //m_addMoreNetworks.relativePosition = new Vector2(-20, m_nettoolSelection.width);
            }
            size = new Vector2(400+4*8, 80 + 40* m_networks.Count);
            */
            base.Update();
        }

        private UINetTypeOption CreateNetworksDropdown()
        {
            var dropdown = AddUIComponent<UINetTypeOption>();
            dropdown.relativePosition = Vector2.zero;
            dropdown.Populate();

            dropdown.OnChangedCallback = () =>
            {
                DebugUtils.Log(
                    $"OptionsPanel.SelectionChangedCallback() - Selected {dropdown.SelectedNetInfo?.name} with offset {dropdown.Offset}");
                UpdateOptions();
            };

            dropdown.OnDeleteButtonCallback = () =>
            {
                DebugUtils.Log(
                   $"OptionsPanel.OnDeleteButtonCallback() - Selected {dropdown.SelectedNetInfo?.name} with offset {dropdown.Offset}");
                UpdateOptions();
            };

            return dropdown;
        }

        private UIButton CreateAddButton(UIComponent parent, string spriteName, string toolTip, MouseEventHandler clickAction)
        {
            var button = parent.AddUIComponent<UIButton>();
            button.name = "PRT_" + spriteName;
            button.atlas = m_atlas;
            button.tooltip = toolTip;
            button.relativePosition = new Vector2(200, 25);
            button.size = new Vector2(30, 30);

            button.normalBgSprite = "OptionBase";
            button.hoveredBgSprite = "OptionBaseHovered";
            button.pressedBgSprite = "OptionBasePressed";
            button.disabledBgSprite = "OptionBaseDisabled";

            button.normalFgSprite = spriteName;
            button.hoveredFgSprite = spriteName + "Hovered";
            button.pressedFgSprite = spriteName + "Pressed";
            button.disabledFgSprite = spriteName + "Disabled";

            button.eventClicked += clickAction;

            return button;
        }

        private UICheckBox CreateCheckBox(UIComponent parent, string spriteName, string toolTip, bool value)
        {
            var checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.size = new Vector2(36, 36);

            var button = checkBox.AddUIComponent<UIButton>();
            button.name = "PRT_" + spriteName;
            button.atlas = m_atlas;
            button.tooltip = toolTip;
            button.relativePosition = new Vector2(150, -36);

            button.normalBgSprite = "OptionBase";
            button.hoveredBgSprite = "OptionBaseHovered";
            button.pressedBgSprite = "OptionBasePressed";
            button.disabledBgSprite = "OptionBaseDisabled";

            button.normalFgSprite = spriteName;
            button.hoveredFgSprite = spriteName + "Hovered";
            button.pressedFgSprite = spriteName + "Pressed";
            button.disabledFgSprite = spriteName + "Disabled";

            checkBox.isChecked = value;
            if (value)
            {
                button.normalBgSprite = "OptionBaseFocused";
                button.normalFgSprite = spriteName + "Focused";
            }

            checkBox.eventCheckChanged += (c, s) =>
            {
                if (s)
                {
                    button.normalBgSprite = "OptionBaseFocused";
                    button.normalFgSprite = spriteName + "Focused";
                }
                else
                {
                    button.normalBgSprite = "OptionBase";
                    button.normalFgSprite = spriteName;
                }

                UpdateOptions();
            };

            return checkBox;
        }

        private void UpdateOptions()
        {
            DebugUtils.Log("OptionsPanel.UpdateOptions()");

            ParallelRoadTool.IsParallelEnabled = m_parallel.isChecked;
            //ParallelRoadTool.SelectedRoadTypes =
            //    m_networks.Select(n => new Tuple<NetInfo, float>(n.SelectedNetInfo, n.Offset)).ToList();
            NetManagerDetour.NetworksCount = ParallelRoadTool.SelectedRoadTypes.Count;

            foreach (var item in m_networks) item.enabled = m_parallel.isChecked;
        }

        private void LoadResources()
        {
            string[] spriteNames = 
            {
                "Anarchy",
                "AnarchyDisabled",
                "AnarchyFocused",
                "AnarchyHovered",
                "AnarchyPressed",
                "Add",
                "AddDisabled",
                "AddFocused",
                "AddHovered",
                "AddPressed",
                "Bending",
                "BendingDisabled",
                "BendingFocused",
                "BendingHovered",
                "BendingPressed"
            };

            m_atlas = ResourceLoader.CreateTextureAtlas("ParallelRoadTool", spriteNames, "ParallelRoadTool.Icons.");

            var defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures =
            {
                defaultAtlas["OptionBase"].texture,
                defaultAtlas["OptionBaseFocused"].texture,
                defaultAtlas["OptionBaseHovered"].texture,
                defaultAtlas["OptionBasePressed"].texture,
                defaultAtlas["OptionBaseDisabled"].texture
            };

            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);
        }
    }
}