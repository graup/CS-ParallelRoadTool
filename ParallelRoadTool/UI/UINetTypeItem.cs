using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using ParallelRoadTool.Detours;
using ParallelRoadTool.Redirection;
using ParallelRoadTool.UI;
using ParallelRoadTool.UI.Base;
using UnityEngine;
using System;

namespace ParallelRoadTool
{
    public class UINetTypeItem : UIPanel
    {
        public UILabel Label;
        protected UITextField TextField { get; private set; }
        protected UIButton DeleteButton { get; private set; }
        protected UIButton AddButton { get; private set; }
        public NetInfo netInfo;
        public bool isCurrentItem { get; set; } = false;
        public float offset;
        public int index;

        public Action OnChangedCallback { private get; set; }
        public Action OnDeleteCallback { private get; set; }
        public Action OnAddCallback { private get; set; }

        public override void Start()
        {
            name = "PRT_NetTypeItem";
            atlas = ResourceLoader.GetAtlas("Ingame");
            backgroundSprite = "SubcategoriesPanel";
            color = new Color32(255, 255, 255, 255);
            size = new Vector2(450 - 8*2 - 4*2, 40);

            var TEXT_FIELD_WIDTH = 60;
            var LABEL_WIDTH = 300;
            var COLUMN_PADDING = 8f;

            TextField = UIUtil.CreateTextField(this);
            TextField.relativePosition = new Vector3(LABEL_WIDTH, 10);
            TextField.width = TEXT_FIELD_WIDTH;

            TextField.eventTextSubmitted += TextField_eventTextSubmitted;

            Label = AddUIComponent<UILabel>();
            Label.textScale = 1f;
            Label.text = "Select a network";
            Label.autoSize = false;
            Label.width = LABEL_WIDTH;
            Label.relativePosition = new Vector3(10, 12);

            DeleteButton = AddUIComponent<UIButton>();
            DeleteButton.text = "";
            DeleteButton.tooltip = "Remove network";
            DeleteButton.size = new Vector2(36, 36);
            DeleteButton.textVerticalAlignment = UIVerticalAlignment.Middle;
            DeleteButton.textHorizontalAlignment = UIHorizontalAlignment.Left;
            DeleteButton.atlas = ResourceLoader.GetAtlas("ParallelRoadTool");
            DeleteButton.normalFgSprite = "Remove";
            DeleteButton.hoveredFgSprite = "RemoveHovered";
            DeleteButton.pressedFgSprite = "RemovePressed";
            DeleteButton.focusedFgSprite = "RemoveFocussed";
            DeleteButton.disabledFgSprite = "RemoveDisabled";
            DeleteButton.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            DeleteButton.horizontalAlignment = UIHorizontalAlignment.Right;
            DeleteButton.verticalAlignment = UIVerticalAlignment.Middle;
            DeleteButton.zOrder = 0;
            DeleteButton.textScale = 0.8f;
            DeleteButton.relativePosition = new Vector3(TEXT_FIELD_WIDTH + LABEL_WIDTH + 3 * COLUMN_PADDING, 0);

            DeleteButton.eventClicked += DeleteButton_eventClicked;

            AddButton = AddUIComponent<UIButton>();
            AddButton.atlas = ResourceLoader.GetAtlas("ParallelRoadTool");
            AddButton.text = "";
            AddButton.tooltip = "Add network";
            AddButton.size = new Vector2(36, 36);
            AddButton.textVerticalAlignment = UIVerticalAlignment.Middle;
            AddButton.textHorizontalAlignment = UIHorizontalAlignment.Left;
            AddButton.normalFgSprite = "Add";
            AddButton.hoveredFgSprite = "AddHovered";
            AddButton.pressedFgSprite = "AddPressed";
            AddButton.focusedFgSprite = "AddFocussed";
            AddButton.disabledFgSprite = "AddDisabled";
            AddButton.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            AddButton.horizontalAlignment = UIHorizontalAlignment.Right;
            AddButton.verticalAlignment = UIVerticalAlignment.Middle;
            AddButton.zOrder = 1;
            AddButton.isVisible = false;
            AddButton.textScale = 0.8f;
            AddButton.relativePosition = new Vector3(TEXT_FIELD_WIDTH + LABEL_WIDTH + 3 * COLUMN_PADDING, 0);

            AddButton.eventClicked += AddButton_eventClicked;

            RenderItem();


        }

        public void RenderItem()
        {
            DebugUtils.Log($"RenderItem {netInfo} at {offset}");
            if (netInfo != null)
                Label.text = netInfo.GenerateBeautifiedNetName();

            TextField.text = offset.ToString();

            if (isCurrentItem)
            {
                DeleteButton.isVisible = false;
                TextField.isVisible = false;
                AddButton.isVisible = true;
                Label.text = $"Current: {Label.text}";
            }
            else
            {

            }
        }

        private void TextField_eventTextSubmitted(UIComponent component, string value)
        {
            if (!float.TryParse(value, out offset)) return;
            OnChangedCallback?.Invoke();
        }

        private void AddButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            DebugUtils.Log("UINetTypeItem.AddButton_eventClicked");
            OnAddCallback?.Invoke();
        }

        private void DeleteButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            DebugUtils.Log("UINetTypeItem.DeleteButton_eventClicked");
            OnDeleteCallback?.Invoke();
        }

    }
}
