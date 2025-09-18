using OverBang.GameName.Editor.EditorStyle;
using OverBang.GameName.Managers;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SceneManager = OverBang.GameName.Core.Scene.SceneManager;

namespace OverBang.GameName.Editor
{
    public class SceneManagerWindow : EditorWindow
    {
        [MenuItem("Tools/Networking/Scene Manager")]
        public static void ShowWindow() => GetWindow<SceneManagerWindow>("Scene Manager");

        private int selectedSceneIndex = 0;
        private bool guiBuilt = false;

        private void OnEnable()
        {
            EditorApplication.update += UpdateWindow;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateWindow;
        }

        private bool CheckForValidation() =>
            EditorApplication.isPlaying &&
            NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening;

        private void UpdateWindow()
        {
            // Si network actif et UI pas encore construite → build GUI
            if (CheckForValidation() && !guiBuilt)
            {
                rootVisualElement.Clear();
                BuildGUI();
                guiBuilt = true;
            }
            // Si network inactif et UI construite → afficher warning
            else if (!CheckForValidation() && guiBuilt)
            {
                rootVisualElement.Clear();
                BuildWarning();
                guiBuilt = false;
            }
            // Si network inactif et GUI pas construite → warning
            else if (!CheckForValidation() && !guiBuilt)
            {
                rootVisualElement.Clear();
                BuildWarning();
            }
        }

        private void BuildWarning()
        {
            Label warningLabel = new Label("No active network connections found. Cannot fetch data.");
            warningLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            warningLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            warningLabel.style.color = new Color(1f, 0.65f, 0f);
            warningLabel.style.paddingTop = 10;
            warningLabel.style.paddingBottom = 10;

            rootVisualElement.Add(warningLabel);
        }

        private void BuildGUI()
        {
            ScrollView scrollView = new ScrollView();
            rootVisualElement.Add(scrollView);

            // Header
            Label header = new Label("Scenes in Build");
            EditorCustomStyle.Create(header).Bold().AlignCenter().FontSize(14).Build();
            scrollView.Add(header);
            scrollView.Add(new VisualElement() { style = { height = 5 } });

            // Active scene
            string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;// ton SceneManager custom
            Label activeSceneLabel = new Label($"\u23ed\ufe0f Active Scene : {activeScene}");
            EditorCustomStyle.Create(activeSceneLabel).AlignCenter().FontSize(12).Build();
            scrollView.Add(activeSceneLabel);
            scrollView.Add(new VisualElement() { style = { height = 10 } });

            // Scenes dropdown
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            var sceneList = new System.Collections.Generic.List<string>(sceneCount);
            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                sceneList.Add(System.IO.Path.GetFileNameWithoutExtension(scenePath));
            }

            DropdownField dropdown = new DropdownField("Select Scene", sceneList, selectedSceneIndex);
            dropdown.RegisterValueChangedCallback(evt =>
            {
                selectedSceneIndex = sceneList.IndexOf(evt.newValue);
            });
            scrollView.Add(dropdown);
            scrollView.Add(new VisualElement() { style = { height = 10 } });

            // Teleport button
            Button button = new Button(() =>
            {
                string sceneName = sceneList[selectedSceneIndex];
                TeleportPlayersToScene(sceneName);
            })
            {
                text = "\u2708\ufe0f Teleport Players \u2708\ufe0f"
            };
            EditorCustomStyle.Create(button).FontSize(12).Bold().Build();
            scrollView.Add(button);
        }

        private void TeleportPlayersToScene(string sceneName)
        {
            Debug.Log($"Teleporting all players to scene '{sceneName}'");
            SceneManager.Instance.ChangeScene(sceneName);
        }
    }
}
