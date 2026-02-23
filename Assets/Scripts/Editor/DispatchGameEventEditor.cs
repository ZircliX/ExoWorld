using System;
using System.Collections.Generic;
using System.Reflection;
using OverBang.ExoWorld.Gameplay.Quests;
using UnityEditor;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Editor
{
    public class DispatchGameEventEditor : EditorWindow
    {
        private Type selectedEventType;
        private Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        private Vector2 scrollPosition;

        [MenuItem("Window/Game Event Dispatcher")]
        public static void ShowWindow()
        {
            GetWindow<DispatchGameEventEditor>("Event Dispatcher");
        }

        private void OnGUI()
        {
            GUILayout.Label("Game Event Dispatcher", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Event Type Selection
            GUILayout.Label("Select Event Type:", EditorStyles.boldLabel);
            Type[] eventTypes = GetIGameEventTypes();
            string[] typeNames = Array.ConvertAll(eventTypes, t => t.Name);
            
            int selectedIndex = selectedEventType == null ? -1 : System.Array.IndexOf(eventTypes, selectedEventType);
            selectedIndex = EditorGUILayout.Popup("Event Type", selectedIndex, typeNames);

            if (selectedIndex >= 0)
            {
                selectedEventType = eventTypes[selectedIndex];
                DrawEventParameterFields();
            }

            EditorGUILayout.Space();

            // Dispatch Button
            if (GUILayout.Button("Dispatch Event", GUILayout.Height(40)))
            {
                DispatchEvent();
            }
        }

        private void DrawEventParameterFields()
        {
            FieldInfo[] fields = selectedEventType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Length == 0)
            {
                EditorGUILayout.HelpBox("This event has no public fields.", MessageType.Info);
                return;
            }

            GUILayout.Label("Parameters:", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (FieldInfo field in fields)
            {
                string fieldName = field.Name;
                
                if (!eventParameters.ContainsKey(fieldName))
                {
                    eventParameters[fieldName] = GetDefaultValue(field.FieldType);
                }

                DrawFieldInput(field, fieldName);
            }

            GUILayout.EndScrollView();
        }

        private void DrawFieldInput(FieldInfo field, string fieldName)
        {
            Type fieldType = field.FieldType;
            object currentValue = eventParameters[fieldName];

            if (fieldType == typeof(int))
            {
                eventParameters[fieldName] = EditorGUILayout.IntField(fieldName, (int)currentValue);
            }
            else if (fieldType == typeof(float))
            {
                eventParameters[fieldName] = EditorGUILayout.FloatField(fieldName, (float)currentValue);
            }
            else if (fieldType == typeof(string))
            {
                eventParameters[fieldName] = EditorGUILayout.TextField(fieldName, (string)currentValue ?? "");
            }
            else if (fieldType == typeof(bool))
            {
                eventParameters[fieldName] = EditorGUILayout.Toggle(fieldName, (bool)currentValue);
            }
            else if (fieldType == typeof(Vector3))
            {
                eventParameters[fieldName] = EditorGUILayout.Vector3Field(fieldName, (Vector3)currentValue);
            }
            else if (fieldType == typeof(Color))
            {
                eventParameters[fieldName] = EditorGUILayout.ColorField(fieldName, (Color)currentValue);
            }
            else
            {
                EditorGUILayout.LabelField(fieldName, "(Unsupported type: " + fieldType.Name + ")");
            }
        }

        private void DispatchEvent()
        {
            if (selectedEventType == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an event type.", "OK");
                return;
            }

            try
            {
                // Get constructor parameters in order
                FieldInfo[] fields = selectedEventType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                object[] constructorParams = new object[fields.Length];

                for (int i = 0; i < fields.Length; i++)
                {
                    constructorParams[i] = eventParameters[fields[i].Name];
                }

                // Create event instance
                object eventInstance = Activator.CreateInstance(selectedEventType, constructorParams);

                // Dispatch through ObjectiveManager (static method)
                MethodInfo dispatchMethod = typeof(ObjectivesManager).GetMethod(
                    "DispatchGameEvent",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (dispatchMethod != null)
                {
                    dispatchMethod.Invoke(null, new[] { eventInstance });
                    //EditorUtility.DisplayDialog("Success", $"Event '{selectedEventType.Name}' dispatched!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "ObjectiveManager.DispatchGameEvent method not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to dispatch event: {ex.Message}", "OK");
            }
        }

        private Type[] GetIGameEventTypes()
        {
            Type gameEventInterface = typeof(IGameEvent);
            Assembly assembly = typeof(QuestManager).Assembly;

            Type[] types = assembly.GetTypes();
            List<Type> eventTypes = new List<Type>();

            foreach (Type type in types)
            {
                if (gameEventInterface.IsAssignableFrom(type) && type != gameEventInterface && !type.IsAbstract)
                {
                    eventTypes.Add(type);
                }
            }

            return eventTypes.ToArray();
        }

        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}