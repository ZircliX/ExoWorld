using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Insect_VFX
{
    public class InsectEmitter : MonoBehaviour
    {
        [Header("Emission Settings")] [HideInInspector]
        public EmitterState emitterState = EmitterState.Idle;

        public bool playOnAwake = true;

        [Range(1, MAX_NUMBER_OF_EMISSIONS)]
        public int numberOfEmissions = 5;
        [SerializeField] private GameObject[] emissionObjects;
        [Range(0.1f, 10f)] public float simulationSize = 1f;
        [Range(0.5f, 5f)] public float simulationHeight = 1f;
        [Space] public bool loopMode;

        public SettingsPreset settingsPreset;
        public float moveSpeed = 1f;
        public float rotationSpeed = 15f;
        [Range(0.25f, 1.5f)]
        public float sizeVariation = 0.5f;
        [SerializeField] public float pauseTime = 1f;

        [Header("PathFinding Settings")] public LayerMask surface_LayerMask;
        public SimulationMode simulationMode;
        public InsectNavigationBake.BakeResolutions bakeResolutions;
        public PathFindingQuality pathfindingResolution = PathFindingQuality.Medium;
        [HideInInspector] public int pathfindingResolutionSteps = 10;
        [SerializeField] public Texture2D navigationTexture;

        private Entity[] _entityData = new Entity[0];

        [HideInInspector] public Vector3 spawnOrigin;

        /*      POOLING         */
        private Queue<GameObject> _objectPool;
        private Transform _entityParent;
        
        /*      DEBUG MODE      */ 
        private bool _debugMode;

        #region Events

        public Action<int> OnEmittedObjectStart;
        public Action<int> OnEmittedObjectPaused;
        public Action<int> OnEmittedObjectResume;
        public Action<int> OnEmittedObjectEnd;
        public Action<int> OnEmittedObjectDestroyed;

        #endregion

        #region Constant

        private const float Y_OFFSET = 0.005f;
        public const int MAX_NUMBER_OF_EMISSIONS = 100;

        #endregion

        #region Public Methods

        public void StartSimulation()
        {
            if (emitterState != EmitterState.Idle)
            {
                RestartSimulation();
                return;
            }

            _entityData = new Entity[numberOfEmissions];

            for (int i = 0; i < _entityData.Length; i++)
            {
                _entityData[i] = new Entity();
                _entityData[i].dieOnFinishPath = loopMode;
            }

            switch (simulationMode)
            {
                case SimulationMode.Runtime:
                    RaycastHit hit;
                    Physics.Raycast(transform.position, -transform.up, out hit, 100, surface_LayerMask);
                    spawnOrigin = hit.point + Vector3.down * 0.25f;
                    break;
                case SimulationMode.Baked:
                    spawnOrigin.y = InsectNavigationBake.GetHeightFromTexture(spawnOrigin, this, InsectNavigationBake.GetBakeResolution(bakeResolutions)) - 1 * 0.25f;
                    break;
            }

            for (int i = 0; i < numberOfEmissions; i++)
                SpawnEntity(i, spawnOrigin, Quaternion.FromToRotation(transform.up, Vector3.up));

            if (loopMode)
            {
                OnEmittedObjectDestroyed -= RecreateEntity;
                OnEmittedObjectDestroyed += RecreateEntity;
            }

            emitterState = EmitterState.Playing;
        }

        public void StartSimulation(float simulationTime)
        {
            StartSimulation();
            Invoke("EndSimulation", simulationTime);
        }

        public void RestartSimulation()
        {
            emitterState = EmitterState.Idle;

            for (int i = 0; i < _entityData.Length; i++) DespawnEntity(i);

            StartSimulation();
        }

        public void EndSimulation()
        {
            for (int i = 0; i < _entityData.Length; i++) DespawnEntityDynamic(i);

            OnEmittedObjectDestroyed -= RecreateEntity;

            emitterState = EmitterState.Ending;
        }

        public void StopSimulation()
        {
            emitterState = EmitterState.Idle;

            for (int i = 0; i < _entityData.Length; i++) DespawnEntity(i);
        }

        public void SetNumberOfEmissions(int newNumber)
        {
            if (newNumber == numberOfEmissions)
                return;

            StopSimulation();
            numberOfEmissions = newNumber;
            RestartSimulation();
        }

        public void SetEntitiesSettings(float _moveSpeed, float _rotationSpeed, float _pauseTime)
        {
            this.moveSpeed = _moveSpeed;
            this.rotationSpeed = _rotationSpeed;
            this.pauseTime = _pauseTime;
        }

        public GameObject GetEntityByIndex(int index)
        {
            return _entityData[index].objectEmmited;
        }

        public Vector3 GetEmissionOrigin()
        {
            Vector3 origin = Vector3.zero;
            switch (simulationMode)
            {
                case SimulationMode.Runtime:
                    origin = transform.position;
                    break;
                case SimulationMode.Baked:
                    origin = spawnOrigin;
                    break;
            }

            return origin;
        }

        #endregion

        private void Awake()
        {
            spawnOrigin = transform.position;

            if (navigationTexture == null && simulationMode == SimulationMode.Baked)
            {
                Debug.LogWarning($"[Insect-VFX] Navigation texture is not set, please bake map on {gameObject.name} object.");
                this.enabled = false;
            }
                
            
            if (_entityParent == null)
            {
                if (GameObject.Find("_EntityParent") != null)
                    _entityParent = GameObject.Find("_EntityParent").transform;
                else
                    _entityParent = new GameObject("_EntityParent").transform;
            }
            
            InitializePooling();
            
            if (playOnAwake)
            {
                StartSimulation();
                /*
                Debug.Log("Start Simulation");
                */
            }
        }

        private void Update()
        {
            if (_entityData.Length == 0)
                return;

            if (emitterState == EmitterState.Idle)
                return;

            for (int i = 0; i < _entityData.Length; i++)
            {
                if (_entityData[i] == null || _entityData[i].pathPoints == null || _entityData[i].pathPoints.Length == 0)
                    continue;

                if (_entityData[i].pauseTimer > 0)
                {
                    _entityData[i].pauseTimer -= Time.deltaTime;

                    if (_entityData[i].pauseTimer <= 0) OnEmittedObjectResume?.Invoke(i);
                    continue;
                }
                
                if(_entityData[i].objectEmmited)
                    _entityData[i].objectEmmited.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0,Mathf.Abs(Mathf.Sin(Time.time * 20f) * 100));
                
                MoveEntityAlongPath(i);

                CheckEntityFinishedPath(i);
            }
        }

        private void CheckEntityFinishedPath(int index)
        {
            ref var entity = ref _entityData[index];

            if (!_entityData[index].objectEmmited)
                return;

            Vector3 a = entity.objectEmmited.transform.position;
            Vector3 b = entity.pathPoints[0];
            
            a.y = 0;
            b.y = 0;
            
            if ((a - b).sqrMagnitude < 0.000625f)            
            {
                entity.pathPoints = RemoveReachedPoint(entity.pathPoints);
                if (entity.pathPoints.Length == 0)
                {
                    OnEmittedObjectEnd?.Invoke(index);

                    if (entity.dieOnFinishPath)
                    {
                        DespawnEntity(index);
                        OnEmittedObjectDestroyed?.Invoke(index);
                    }
                    else
                    {
                        entity.targetPosition = GenerateRandomTargetPosition(GetEmissionOrigin());
                        entity.pauseTimer = GetPauseTime();
                        OnEmittedObjectPaused?.Invoke(index);

                        entity.pathPoints = GeneratePathPoints(entity.objectEmmited.transform.position, entity.targetPosition);
                    }
                }
                
            }
        }

        private void MoveEntityAlongPath(int index)
        {
            if(!_entityData[index].objectEmmited)
                return;
            
            Transform entityTransform = _entityData[index].objectEmmited.transform;
            Vector3 currentPosition = entityTransform.position;
            Vector3 targetPosition = _entityData[index].pathPoints[0];

            Vector3 direction = (targetPosition - currentPosition);
            float distance = direction.magnitude;

            if (distance > 0.001f)
            {
                direction /= distance;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                entityTransform.rotation = Quaternion.Slerp(entityTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                float angleDifference = Vector3.Angle(entityTransform.forward, direction);
                float velocityResistance = Mathf.Clamp01(1 - (angleDifference / 25));

                entityTransform.position += direction * (moveSpeed * velocityResistance * Time.deltaTime);
            }
        }

        private void InitializePooling()
        {
            _objectPool = new Queue<GameObject>();

            for (int i = 0; i < numberOfEmissions; i++)
            {
                GameObject obj = Instantiate(emissionObjects[i % emissionObjects.Length], _entityParent);
                obj.transform.position = spawnOrigin;
                obj.transform.localScale *= Random.Range(1f, sizeVariation);
                obj.SetActive(false);
                _objectPool.Enqueue(obj);
            }
        }

        private void SpawnEntity(int index, Vector3 position, Quaternion rotation)
        {
            if (_entityData[index] == null) _entityData[index] = new Entity();

            GameObject entity;

            // Intenta obtener un objeto del pool
            if (_objectPool.Count > 0)
            {
                entity = _objectPool.Dequeue();
            }
            else
            {
                // Si el pool est� vac�o, muestra una advertencia y det�n el proceso
                Debug.LogWarning("Pool is empty! Consider increasing its size.");
                return;
            }

            // Configurar el objeto para su uso
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            entity.SetActive(true);

            _entityData[index].objectEmmited = entity;
            _entityData[index].targetPosition = GenerateRandomTargetPosition(position);
            _entityData[index].pauseTimer = GetPauseTime();
            _entityData[index].pathPoints =
                GeneratePathPoints(entity.transform.position, _entityData[index].targetPosition);

            OnEmittedObjectStart?.Invoke(index);
        }

        private void DespawnEntityDynamic(int index)
        {
            if (index < 0 || index >= _entityData.Length)
            {
                Debug.LogWarning($"Index {index} is out of range on DespawnEntityDynamic.");
                return;
            }

            _entityData[index].dieOnFinishPath = true;

            if (_entityData[index].pathPoints != null && _entityData[index].pathPoints.Length > 0)
                _entityData[index].pathPoints[_entityData[index].pathPoints.Length - 1] += Vector3.down;
            else
                RecreateEntity(index);
        }

        private void DespawnEntity(int index)
        {
            if (_entityData[index]?.objectEmmited)
            {
                _entityData[index].objectEmmited.transform.position = spawnOrigin + Vector3.down;
                _entityData[index].objectEmmited.SetActive(false);
                _objectPool.Enqueue(_entityData[index].objectEmmited);

                _entityData[index].objectEmmited = null;
            }
        }

        private void RecreateEntity(int index)
        {
            SpawnEntity(index, spawnOrigin, Quaternion.identity);
        }

        private Vector3 GenerateRandomTargetPosition(Vector3 origin)
        {
            Vector3 simulationCenter = GetEmissionOrigin();

            float range = simulationSize * 0.5f;
            Vector3 newPos = origin + new Vector3(
                Random.Range(-range, range),
                0,
                Random.Range(-range, range)
            );

            Vector3 minPos = simulationCenter - Vector3.right * (simulationSize * 0.5f) -
                             Vector3.forward * (simulationSize * 0.5f);
            Vector3 maxPos = simulationCenter + Vector3.right * (simulationSize * 0.5f) +
                             Vector3.forward * (simulationSize * 0.5f);

            newPos = new Vector3(
                Mathf.Clamp(newPos.x, minPos.x, maxPos.x),
                0,
                Mathf.Clamp(newPos.z, minPos.z, maxPos.z)
            );

            switch (simulationMode)
            {
                case SimulationMode.Runtime:
                    Vector3 rayOrigin = newPos;
                    rayOrigin.y = transform.position.y;

                    Ray ray = new(rayOrigin, -Vector3.up);
                    Physics.Raycast(ray, out RaycastHit hitInfo, 100, surface_LayerMask);
                    newPos.y = hitInfo.point.y + 0.01f;
                    break;
                case SimulationMode.Baked:
                    newPos.y = InsectNavigationBake.GetHeightFromTexture(newPos, this, InsectNavigationBake.GetBakeResolution(bakeResolutions));
                    break;
            }

            return newPos;
        }

        private Vector3[] GeneratePathPoints(Vector3 start, Vector3 end)
        {
            var points = new Vector3[pathfindingResolutionSteps];
            Vector3 direction = (end - start) / pathfindingResolutionSteps;

            for (int i = 0; i < pathfindingResolutionSteps; i++)
            {
                Vector3 point = start + direction * i;

                switch (simulationMode)
                {
                    case SimulationMode.Runtime:
                        if (Physics.Raycast(point + transform.up, -transform.up, out RaycastHit hitInfo, 100,
                                surface_LayerMask)) point.y = hitInfo.point.y + Y_OFFSET;
                        break;
                    case SimulationMode.Baked:
                        point.y = InsectNavigationBake.GetHeightFromTexture(point, this, InsectNavigationBake.GetBakeResolution(bakeResolutions)) + Y_OFFSET;
                        break;
                }

                if (loopMode)
                {
                    if (i == pathfindingResolutionSteps - 1)
                        point.y -= .5f;
                    else if (i == 1)
                        point.y += .1f;
                }

                points[i] = point;
            }

            return points;
        }

        private Vector3[] RemoveReachedPoint(Vector3[] path)
        {
            if (path.Length <= 1) return Array.Empty<Vector3>();

            var newPath = new Vector3[path.Length - 1];
            for (int i = 1; i < path.Length; i++) newPath[i - 1] = path[i];
            return newPath;
        }

        private float GetPauseTime()
        {
            return pauseTime * Random.Range(0.1f, 1f);
        }

        [ContextMenu("Toggle DebugMode")]
        private void ToggleSimulationDebugMode()
        {
            _debugMode = !_debugMode;
        }

        private void OnDrawGizmos()
        {
            Vector3 originPosition = transform.position;

            if (Application.isPlaying)
                switch (simulationMode)
                {
                    case SimulationMode.Baked:
                        originPosition = spawnOrigin;
                        break;
                }

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(originPosition, new Vector3(simulationSize, simulationHeight, simulationSize));

            Gizmos.color = Color.blue;
            if (Physics.Raycast(originPosition + Vector3.up * (simulationHeight * 0.5f), Vector3.down,
                    out RaycastHit hit, simulationHeight, surface_LayerMask))
            {
                Gizmos.DrawSphere(hit.point, 0.01f);
            }

            if (!_debugMode)
                return;

            if (_entityData.Length > 0)
            {
                for (int i = 0; i < _entityData.Length; i++)
                {
                    Gizmos.color = Color.blue;
                    for (int j = 0; j < _entityData[i].pathPoints.Length - 1; j++)
                    {
                        Vector3 a = _entityData[i].pathPoints[j];
                        Vector3 b = _entityData[i].pathPoints[j + 1];
                        Gizmos.DrawLine(a, b);
                    }
                }
            }
        }
    }

    public enum SettingsPreset
    {
        Custom,
        Default
    }

    public enum SimulationMode
    {
        Runtime,
        Baked
    }

    public enum PathFindingQuality
    {
        Low,
        Medium,
        High,
        MaxQuality
    }

    public enum EmitterState
    {
        Idle,
        Playing,
        Ending
    }

}