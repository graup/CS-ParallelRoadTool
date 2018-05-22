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
    public class UIOptionsPanel : UIPanel
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
            size = new Vector2(450 - 8*2, 36 + 2*8);

            padding = new RectOffset(8, 8, 8, 8);
            autoLayoutPadding = new RectOffset(0, 4, 0, 0);
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayout = true;
            autoSize = false;
            //autoFitChildrenVertically = true;

            m_parallel = AddCheckBox("Parallel", "Toggle parallel road tool", false);
            AddCheckBox("Anarchy", "Toggle parallel road tool", false);
            AddCheckBox("Anarchy", "Toggle parallel road tool", false);

            UpdateOptions();

            /*
m_nettoolSelection = AddUIComponent<UILabel>();
m_nettoolSelection.name = "PRT_NetToolSelectionLabel";
m_nettoolSelection.textAlignment = UIHorizontalAlignment.Center;
m_nettoolSelection.textScale = 1.2f;
m_nettoolSelection.text = "Select a network";
m_nettoolSelection.autoSize = false;
m_nettoolSelection.width = 400 + 8 * 4;
m_nettoolSelection.autoHeight = false;
m_nettoolSelection.height = 60;
*/


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
                m_nettoolSelection.text = netToolSelection.GenerateBeautifiedNetName();
                //m_nettoolSelection.
                //m_addMoreNetworks.relativePosition = new Vector2(-20, m_nettoolSelection.width);
            }
            size = new Vector2(400 + 4 * 8, 80 + 40 * m_networks.Count);
            base.Update();
            */
        }


        private UIButton AddButton(string spriteName, string toolTip, MouseEventHandler clickAction)
        {
            var button = AddUIComponent<UIButton>();
            button.name = "PRT_" + spriteName;
            button.atlas = m_atlas;
            button.tooltip = toolTip;
            button.size = new Vector2(36, 36);

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

        private UICheckBox AddCheckBox(string spriteName, string toolTip, bool value)
        {
            var checkBox = AddUIComponent<UICheckBox>();
            checkBox.size = new Vector2(36, 36);

            var button = checkBox.AddUIComponent<UIButton>();
            button.name = "PRT_" + spriteName;
            button.atlas = m_atlas;
            button.tooltip = toolTip;
            button.relativePosition = new Vector2(0, 0);

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
            DebugUtils.Log("UIOptionsPanel.UpdateOptions()");

            ParallelRoadTool.IsParallelEnabled = m_parallel.isChecked;
            
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
                "Remove",
                "RemoveDisabled",
                "RemoveFocused",
                "RemoveHovered",
                "RemovePressed",
                "Bending",
                "BendingDisabled",
                "BendingFocused",
                "BendingHovered",
                "BendingPressed",
                "Parallel",
                "ParallelDisabled",
                "ParallelFocused",
                "ParallelHovered",
                "ParallelPressed"
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