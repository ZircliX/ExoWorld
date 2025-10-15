using System.Collections.Generic;
using Helteix.Tools.Editor.Serialisation;
using OverBang.Pooling.Resource;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OverBang.Pooling.Editor
{
    [CustomEditor(typeof(PoolResource))]
    public class PoolResourceEditor : UnityEditor.Editor
    {
        private const string PREFAB_CHOICE = "Prefab";
        private const string RESOURCE_CHOICE = "Resource";
        private const string ADDRESSABLES_CHOICE = "Addressable";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty poolEmptyBehavior = serializedObject.FindBackingFieldProperty(nameof(PoolResource.PoolEmptyBehavior));
            SerializedProperty asset = serializedObject.FindBackingFieldProperty(nameof(PoolResource.Asset));
            root.Add(new PropertyField(poolEmptyBehavior));           
            
            VisualElement assetRow = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                }
            };
            
            root.Add(assetRow);
            asset.managedReferenceValue ??= new PrefabPoolAsset();

            string defaultValue = asset.managedReferenceValue switch
            {
                PrefabPoolAsset => PREFAB_CHOICE,
                ResourcePoolAsset => RESOURCE_CHOICE,
                AddressablePoolAsset => ADDRESSABLES_CHOICE,
                _ => PREFAB_CHOICE,
            };
            
            DropdownField assetType = new DropdownField(new List<string>() 
            {
                PREFAB_CHOICE,
                RESOURCE_CHOICE,
                ADDRESSABLES_CHOICE,
            }, defaultValue)
            {
                style =
                {
                    paddingRight = 5,
                    paddingLeft = 5,
                    width = 115,
                }
            };
            
            assetType.RegisterValueChangedCallback(ctx =>
            {
                switch (ctx.newValue)
                {
                    case PREFAB_CHOICE:
                        asset.managedReferenceValue = new PrefabPoolAsset();
                        break;
                    case RESOURCE_CHOICE:
                        asset.managedReferenceValue = new ResourcePoolAsset();
                        break;
                    case ADDRESSABLES_CHOICE:
                        asset.managedReferenceValue = new AddressablePoolAsset();
                        break;
                }
                //Debug.Log(ctx.newValue);

                serializedObject.ApplyModifiedProperties();
            });
            
            assetRow.Add(assetType);
            
            assetRow.Add(new PropertyField(asset)
            {
                label = string.Empty,
                style =
                {
                    flexGrow = 1,
                }
            });
            
            return root;
        }
    }
}