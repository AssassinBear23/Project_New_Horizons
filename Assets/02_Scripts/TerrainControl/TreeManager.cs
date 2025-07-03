
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum IncreaseTypes { Linearly, Exponentially }
namespace Managers.Terrain
{
    /// <summary>
    /// Moves the tree segment upwards, destroys it once it's out of screen and spawns a new one
    /// </summary>
    public class TreeManager : MonoBehaviour
    {
        [Header("Stats")]
        /// <summary>
        /// The speed at which the terrain moves upwards
        /// </summary>
        [SerializeField] public float MovementSpeed = 5;

        /// <summary>
        /// The way in which the speed is increased over time
        /// </summary>
        [SerializeField] private IncreaseTypes speedIncreaseType = IncreaseTypes.Linearly;

        [SerializeField] private float speedIncrease = 0.001f;
        [SerializeField, Min(1)] private float speedMultiplier = 1.01f;

        [Header("References")]
        [SerializeField] private List<TreeTrunkController> m_treeSegments = new();
        /// <summary>
        /// A reference to the parent object in the scene to keep the hierarchy structured
        /// </summary>
        public GameObject _Parent;
        /// <summary>
        /// The prefab of the tree segment
        /// </summary>
        [SerializeField] private List<GameObject> m_prefabs = new();
        private float m_prefabHeight;
        private GameManager m_Gm;
        private CameraController m_Controller;

        private void Start()
        {
            m_Gm = GameManager.Instance;
            m_prefabHeight = GetPrefabSize().y;
        }
        public void InitializeCameraController(CameraController controller)
        {
            m_Controller = controller;
        }
        private Vector3 GetPrefabSize()
        {
            if (m_prefabs.Count > 0)
            {
                if (m_prefabs[0].TryGetComponent<Renderer>(out var renderer))
                    return renderer.bounds.size;
                else
                {
                    Debug.LogWarning("Background prefab does not have a Renderer component.", this);
                    return Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("Background prefab is not assigned.", this);
                return Vector3.zero;
            }
        }

        public void SetupTreeManager()
        {
            if (GameManager.Instance != null && GameManager.Instance.TreeManager == null)
                GameManager.Instance.TreeManager = this;
            else
                Debug.LogError("GameManager instance is null or TreeManager is already set.");
        }

        private void FixedUpdate()
        {
            if (m_Gm.IsPaused) return;

            m_Controller.UpdatePosition(MovementSpeed);

            IncreaseSpeed(); // Increases movement speed over time
        }

        private void IncreaseSpeed()
        {
            switch (speedIncreaseType)
            {
                case IncreaseTypes.Linearly:
                    MovementSpeed += speedIncrease;
                    break;
                case IncreaseTypes.Exponentially:
                    MovementSpeed *= speedMultiplier;
                    break;
            }
        }

        /// <summary>
        /// Adds a tree segment to the managed list.
        /// </summary>
        /// <param name="treeSegment">The tree segment to add.</param>
        public void AddTreeSegment(TreeTrunkController treeSegment)
        {
            if (!m_treeSegments.Contains(treeSegment)) m_treeSegments.Add(treeSegment);
        }

        /// <summary>
        /// Removes a tree segment from the managed list.
        /// </summary>
        /// <param name="treeSegment">The tree segment to remove.</param>
        public void RemoveTreeSegment(TreeTrunkController treeSegment)
        {
            if (m_treeSegments.Contains(treeSegment)) m_treeSegments.Remove(treeSegment);
        }

        /// <summary>
        /// Places a new tree segment under the specified tree trunk controller.
        /// </summary>
        /// <param name="callerSegment">The segment that called the method, asking for a segment to be placed underneath it.</param>
        public void SpawnNewTreeSegment(TreeTrunkController callerSegment)
        {
            Vector3 pos = callerSegment.transform.position;
            pos.y -= m_prefabHeight - 0.1f;

            GameObject toInstantiate = m_prefabs[Random.Range(0, m_prefabs.Count)];

            Instantiate(toInstantiate, pos, Quaternion.identity, _Parent.transform);
        }

        /// <summary>
        /// Returns a list of all the branches on the last layer of the last placed & finished tree segment
        /// </summary>
        /// <returns></returns>
        public (List<Transform>, bool) GetLastBranchList()
        {
            BranchPlacingAlgorithm reference = m_treeSegments[^1].GetComponent<BranchPlacingAlgorithm>();
            List<Transform> transforms = reference.lastBranches;
            bool isBird = reference.lastWasBird;
            return (transforms, isBird);
        }
    }
}