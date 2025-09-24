using System.Collections.Generic;
using System.Linq;
using LTX.Tools.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.ChanneledProperties.Editor.Core
{
    public abstract class ChanneledPropertyElement<T> : VisualElement, IChanneledPropertyElement
    {
        private readonly IChanneledPropertyEditor<T> channeledProperty;
        private readonly SerializedProperty property;

        private List<ChannelElement<T>> channelElements = new();
        private VisualElement channelsContainer;
        private VisualElement infosContainer;

        private IntegerField capacityField;
        private IntegerField channelCountField;
        private Toggle hasFlexibleSizeField;
        private Button infosTabButton;
        private Button channelsTabButton;

        private bool isShowingChannels;

        public ChanneledPropertyElement(IChanneledPropertyEditor<T> channeledProperty, SerializedProperty property)
        {
            this.channeledProperty = channeledProperty;
            this.property = property;
        }

        void IChanneledPropertyElement.Draw()
        {
            schedule.Execute(Update).Until(() => false);

            VisualElement root = new VisualElement();
            root.AddToClassList("channeled-property-root");

            Add(root);
            channelsContainer = new VisualElement()
            {
                name = "channelsContainer",
                style =
                {
                    flexShrink = 1,
                }
            };
            channelsContainer.AddToClassList("unity-disabled");

            infosContainer = new VisualElement()
            {
                name = "infosContainer"
            };
            infosContainer.AddToClassList("unity-disabled");

            VisualElement toolbar = new VisualElement();
            toolbar.AddToClassList("channeled-property-toolbar");

            infosTabButton = new ToolbarButton(ShowInfos)
            {
                text = "Infos"
            };
            channelsTabButton = new ToolbarButton(ShowChannels)
            {
                text = "Channels"
            };

            channelsTabButton.AddToClassList("channeled-property-toggle");
            infosTabButton.AddToClassList("channeled-property-toggle");

            toolbar.Add(infosTabButton);
            toolbar.Add(channelsTabButton);
            root.Add(toolbar);

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                elasticity = 1,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden,
                style =
                {
                    maxHeight = 250
                }
            };

            root.Add(scrollView);

            scrollView.Add(channelsContainer);
            scrollView.Add(infosContainer);
            ShowChannels();
        }



        private void ShowInfos()
        {
            infosTabButton.AddToClassList("selected-button");
            channelsTabButton.RemoveFromClassList("selected-button");

            infosContainer.ShowManually();
            channelsContainer.HideManually();
            isShowingChannels = false;
        }

        private void ShowChannels()
        {
            channelsTabButton.AddToClassList("selected-button");
            infosTabButton.RemoveFromClassList("selected-button");

            channelsContainer.ShowManually();
            infosContainer.HideManually();
            isShowingChannels = true;
        }

        private void Update()
        {
            if(isShowingChannels)
                UpdateChannels();
            else
                UpdateInfos();
        }

        private void UpdateChannels()
        {
            var channels = channeledProperty.GetChannels();
            HelpBox helpBox = channelsContainer.Q<HelpBox>();

            var tuples = channels as (ChannelKey key, IChannel<T> channel)[] ?? channels.ToArray();
            if (!tuples.Any())
            {
                if (helpBox == null)
                {
                    helpBox = new HelpBox("No channels to show yet.", HelpBoxMessageType.None);
                    channelsContainer.Add(helpBox);
                }
                return;
            }
            if(helpBox != null)
                channelsContainer.Remove(helpBox);

            int idx = 0;
            foreach (var (channelKey, channel) in tuples)
            {
                if (channelElements.Count <= idx)
                {
                    ChannelElement<T> element = CreateChannelElement();
                    element.AddToClassList("channeled-property-element");
                    element.Init(this, channel);

                    channelsContainer.Add(element);
                    channelElements.Add(element);
                }

                channelElements[idx].Update(channelKey, channel);
                idx++;
            }

            for (int i = idx; i < channelElements.Count; i++)
            {
                ChannelElement<T> element = channelElements[idx];
                element.Dispose(this);

                channelsContainer.Remove(element);
                channelElements.Remove(element);
            }
        }

        protected abstract ChannelElement<T> CreateChannelElement();


        protected virtual void UpdateInfos()
        {
            if (capacityField == null)
            {
                capacityField = new IntegerField("Capacity");
                infosContainer.Add(capacityField);
            }

            if (channelCountField == null)
            {
                channelCountField = new IntegerField("Channel Count");
                infosContainer.Add(channelCountField);
            }

            if (hasFlexibleSizeField == null)
            {
                hasFlexibleSizeField = new Toggle("Flexible size");
                infosContainer.Add(hasFlexibleSizeField);
            }

            capacityField.value = channeledProperty.Capacity;
            channelCountField.value = channeledProperty.ChannelCount;
            hasFlexibleSizeField.value = channeledProperty.HasFlexibleSize;

        }

        void IChanneledPropertyElement.Dispose()
        {
            EditorApplication.update -= Update;
        }
    }
}