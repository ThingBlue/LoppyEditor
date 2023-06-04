using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace LoppyEditor
{
    public class EditorManager : MonoBehaviour
    {
        public static EditorManager instance;

        #region Inspector members

        public float doubleClickMaxDelay = 0.5f;

        public GameObject nodePrefab;
        public GameObject connectorPrefab;

        #endregion

        public List<GameObject> nodes;
        public List<GameObject> connectors;

        private bool singleClick;
        private float doubleClickTimer;

        private bool creatingConnector;
        public GameObject newConnector;
        private EditorNode connectorStartNode = null;

        private void Awake()
        {
            // Singleton
            if (instance == null) instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            // Initialize storage lists
            nodes = new List<GameObject>();
            connectors = new List<GameObject>();
        }

        private void Update()
        {
            // Handle timers
            if (singleClick) doubleClickTimer += Time.deltaTime;
            if (doubleClickTimer > doubleClickMaxDelay)
            {
                singleClick = false;
                doubleClickTimer = 0;
            }

            // Handle node creation
            if (Input.GetMouseButtonDown(0) && !isMouseOverUIObject())
            {
                if (!singleClick)
                {
                    singleClick = true;
                    doubleClickTimer = 0;
                }
                else if (singleClick && doubleClickTimer < doubleClickMaxDelay)
                {
                    // Create new node
                    GameObject newNodeObject = Instantiate(nodePrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                    newNodeObject.transform.position = new Vector3(newNodeObject.transform.position.x, newNodeObject.transform.position.y, 0);
                    nodes.Add(newNodeObject);

                    singleClick = false;
                    doubleClickTimer = 0;
                }
            }

            // Handle node connecting
            if (!creatingConnector && Input.GetMouseButtonDown(1))
            {
                // Check if mouse is over any nodes
                foreach (GameObject nodeObject in nodes)
                {
                    EditorNode node = nodeObject.GetComponent<EditorNode>();
                    if (node.mouseHover)
                    {
                        // Create new connector at node position
                        creatingConnector = true;
                        newConnector = Instantiate(connectorPrefab);
                        newConnector.GetComponent<LineRenderer>().SetPosition(0, nodeObject.transform.position);
                        connectorStartNode = node;
                    }
                }
            }
            if (creatingConnector && Input.GetMouseButton(1))
            {
                // Follow mouse
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                newConnector.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
            }
            if (creatingConnector && Input.GetMouseButtonUp(1))
            {
                // Detect if mouse finishes on a node
                bool onNode = false;
                Vector3 nodePosition = Vector3.zero;
                EditorNode connectorEndNode = null;
                foreach (GameObject nodeObject in nodes)
                {
                    EditorNode node = nodeObject.GetComponent<EditorNode>();
                    if (node.mouseHover && node != connectorStartNode)
                    {
                        onNode = true;
                        nodePosition = nodeObject.transform.position;
                        connectorEndNode = node;
                    }
                }

                // Check that the connection does not already exist
                if (onNode && !connectorStartNode.hasConnection(connectorEndNode))
                {
                    // Create connector line
                    newConnector.GetComponent<Connector>().addConnectedNode(connectorStartNode.gameObject);
                    newConnector.GetComponent<Connector>().addConnectedNode(connectorEndNode.gameObject);
                    newConnector.GetComponent<Connector>().connected = true;

                    newConnector.GetComponent<LineRenderer>().SetPosition(1, nodePosition);
                    newConnector.GetComponent<Connector>().bakeMesh();
                    connectors.Add(newConnector);
                    newConnector = null;

                    // Add connection between nodes
                    connectorStartNode.addConnection(connectorEndNode);
                    connectorEndNode.addConnection(connectorStartNode);
                    connectorStartNode = null;
                }
                else
                {
                    Destroy(newConnector);
                }
                creatingConnector = false;
            }

            // Handle object deletion
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                List<GameObject> nodesToDestroy = new List<GameObject>();
                List<GameObject> connectorsToDestroy = new List<GameObject>();

                // Check if any nodes are selected
                foreach (GameObject nodeObject in nodes)
                {
                    EditorNode node = nodeObject.GetComponent<EditorNode>();
                    if (node.selected)
                    {
                        nodesToDestroy.Add(nodeObject);
                    }
                }
                // Check if any connectors are selected
                foreach (GameObject connectorObject in connectors)
                {
                    Connector connector = connectorObject.GetComponent<Connector>();
                    if (connector.selected || connector.connectedNodeObjects[0].GetComponent<EditorNode>().selected || connector.connectedNodeObjects[1].GetComponent<EditorNode>().selected)
                    {
                        connectorsToDestroy.Add(connectorObject);
                    }
                }

                // Destroy and remove selected nodes
                foreach (GameObject nodeToDestroy in nodesToDestroy)
                {
                    // Make sure to remove all connections
                    nodeToDestroy.GetComponent<EditorNode>().clearConnections();
                    nodes.Remove(nodeToDestroy);
                    Destroy(nodeToDestroy);
                }
                // Destroy and remove selected connectors
                foreach (GameObject connectorToDestroy in connectorsToDestroy)
                {
                    // Make sure to remove connections
                    connectorToDestroy.GetComponent<Connector>().clearConnections();
                    connectors.Remove(connectorToDestroy);
                    Destroy(connectorToDestroy);
                }
            }
        }

        public bool isMouseOverUIObject()
        {
            // Check for nodes and connectors
            foreach (GameObject nodeObject in nodes)
            {
                if (nodeObject.GetComponent<EditorNode>().mouseHover) return true;
            }
            foreach (GameObject connectorObject in connectors)
            {
                if (connectorObject.GetComponent<Connector>().mouseHover) return true;
            }

            // Check for UI
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
