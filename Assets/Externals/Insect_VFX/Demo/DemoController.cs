using System;
using UnityEngine;
using Insect_VFX;
using UnityEngine.Serialization;

namespace Insect_VFX_DEMO
{
    [Serializable]
    public struct Section
    {
        public string sectionName;
        public Transform cameraPosition;
        [FormerlySerializedAs("emmiter")] public InsectEmitter emitter;
        public bool move;
        [HideInInspector] public Vector3 iPos;
    }

    public class DemoController : MonoBehaviour
    {
        [SerializeField] private Transform camTransform;
        [SerializeField] private Section[] sections;
        [SerializeField] private float cameraLerpSpeed = 2.0f;

        private int currentSectionIndex = 0;
        private int _numberOfEmissions = 0;
        private Vector3 targetCamPosition;
        private Quaternion targetCamRotation;

        private void Start()
        {
            SetSection(currentSectionIndex);
        }

        private void Update()
        {
            // Move sections
            if (sections[currentSectionIndex].move)
            {
                if (sections[currentSectionIndex].iPos == Vector3.zero)
                    sections[currentSectionIndex].iPos = sections[currentSectionIndex].emitter.transform.position;

                sections[currentSectionIndex].emitter.transform.position = sections[currentSectionIndex].iPos +
                                                                           Vector3.right * Mathf.Sin(Time.time * 0.25f);
            }

            // Lerp camera movement
            camTransform.position =
                Vector3.Lerp(camTransform.position, targetCamPosition, Time.deltaTime * cameraLerpSpeed);
            camTransform.rotation =
                Quaternion.Lerp(camTransform.rotation, targetCamRotation, Time.deltaTime * cameraLerpSpeed);
        }

        #region PUBLIC METHODS

        public void StartSim()
        {
            sections[currentSectionIndex].emitter.StartSimulation();
        }

        public void EndSim()
        {
            sections[currentSectionIndex].emitter.EndSimulation();
        }

        public void SetNumberOfEntities(int value)
        {
            sections[currentSectionIndex].emitter.SetNumberOfEmissions(value);
        }

        #endregion

        private void NextSection()
        {
            sections[currentSectionIndex].emitter.EndSimulation();

            currentSectionIndex++;
            currentSectionIndex %= sections.Length;

            SetSection(currentSectionIndex);
        }

        private void SetSection(int index)
        {
            sections[currentSectionIndex].emitter.StopSimulation();
            currentSectionIndex = index;

            _numberOfEmissions = sections[index].emitter.numberOfEmissions;

            SetCamPos(index);
        }

        private void SetCamPos(int index)
        {
            targetCamPosition = sections[index].cameraPosition.position;
            targetCamRotation = sections[index].cameraPosition.rotation;
        }

        private void OnGUI()
        {
            Rect titleRect = new Rect(0, Screen.height - 100, Screen.width, 50);
            GUI.Label(titleRect, sections[currentSectionIndex].sectionName, new GUIStyle(GUI.skin.label)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            });

            if (GUI.Button(new Rect(10, 40, 100, 30), "Next"))
            {
                NextSection();
            }

            if (GUI.Button(new Rect(110, 40, 100, 30), "Play"))
            {
                StartSim();
            }

            if (GUI.Button(new Rect(210, 40, 100, 30), "End"))
            {
                EndSim();
            }

            if (GUI.Button(new Rect(310, 40, 100, 30), "Stop"))
            {
                sections[currentSectionIndex].emitter.StopSimulation();
            }

            _numberOfEmissions = (int)GUI.HorizontalSlider(
                new Rect(10, 80, 150, 20),
                sections[currentSectionIndex].emitter.numberOfEmissions,
                0,
                InsectEmitter.MAX_NUMBER_OF_EMISSIONS);

            if (sections[currentSectionIndex].emitter.numberOfEmissions != _numberOfEmissions)
                sections[currentSectionIndex].emitter.SetNumberOfEmissions(_numberOfEmissions);

            GUI.Label(new Rect(170, 80, 50, 20), sections[currentSectionIndex].emitter.numberOfEmissions.ToString());

            // Mostrar la textura en la esquina superior derecha
            Texture navTexture = sections[currentSectionIndex].emitter.navigationTexture;
            if (navTexture != null)
            {
                Rect textureRect = new Rect(Screen.width - 138, 10, 128, 128);
                GUI.DrawTexture(textureRect, navTexture);
            }
        }
    }
}