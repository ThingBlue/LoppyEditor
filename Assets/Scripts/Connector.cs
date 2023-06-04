using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoppyEditor
{
    public class Connector : MonoBehaviour
    {
        #region Inspector members

        public LineRenderer lineRenderer;
        public MeshCollider meshCollider;

        public Color defaultColour;
        public Color selectedColour;
        public Color hoverColour;
        public Color pressedColour;

        #endregion

        public bool connected = false;
        public List<GameObject> connectedNodeObjects;

        private bool mouseDown = false;
        public bool selected = false;
        public bool mouseHover = false;

        public void addConnectedNode(GameObject node) { connectedNodeObjects.Add(node); }
        public void clearConnections()
        {
            // Remove connections from connected nodes
            connectedNodeObjects[0].GetComponent<EditorNode>().removeConnection(connectedNodeObjects[1].GetComponent<EditorNode>());
            connectedNodeObjects[1].GetComponent<EditorNode>().removeConnection(connectedNodeObjects[0].GetComponent<EditorNode>());
        }

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.objectSelectedEvent.AddListener(onObjectSelected);
        }

        private void Update()
        {
            // Detect mouse up outside of element
            if (Input.GetMouseButtonUp(0) && !mouseHover) mouseDown = false;

            // Detect deselection
            if (selected && Input.GetMouseButtonDown(0) && !mouseHover && !EditorManager.instance.isMouseOverUIObject()) onDeselect();
        }

        private void FixedUpdate()
        {
            // Set colour
            Color colourToSet = defaultColour;

            if (selected) colourToSet = selectedColour;
            else if (mouseHover && mouseDown) colourToSet = pressedColour;
            else if (mouseHover) colourToSet = hoverColour;
            else colourToSet = defaultColour;

            lineRenderer.startColor = colourToSet;
            lineRenderer.endColor = colourToSet;

            if (connected)
            {
                // Destroy connector if either of its connected nodes are destroyed
                bool destroy = false;
                foreach (GameObject nodeObject in connectedNodeObjects)
                {
                    if (!nodeObject)
                    {
                        Destroy(gameObject);
                        destroy = true;
                    }
                }
                if (destroy) return;

                // Follow movement of nodes
                if (lineRenderer.GetPosition(0) != connectedNodeObjects[0].transform.position || lineRenderer.GetPosition(1) != connectedNodeObjects[1].transform.position)
                {
                    lineRenderer.SetPosition(0, connectedNodeObjects[0].transform.position);
                    lineRenderer.SetPosition(1, connectedNodeObjects[1].transform.position);

                    bakeMesh();
                }
            }
        }

        public void bakeMesh()
        {
            // Create new mesh collider to match movement
            Mesh lineBakedMesh = new Mesh();
            lineRenderer.BakeMesh(lineBakedMesh, true);
            meshCollider.sharedMesh = lineBakedMesh;
        }

        private void OnMouseEnter()
        {
            mouseHover = true;
        }

        private void OnMouseExit()
        {
            mouseHover = false;
        }

        private void OnMouseDown()
        {
            mouseDown = true;
        }

        private void OnMouseUp()
        {
            if (!selected && mouseDown && mouseHover)
            {
                // Check that mouse is not over and nodes
                foreach (GameObject node in EditorManager.instance.nodes)
                {
                    if (node.GetComponent<EditorNode>().mouseHover) return;
                }

                onSelect();
            }
        }

        private void onSelect()
        {
            if (selected) return;

            EventManager.instance.objectSelectedEvent.Invoke();

            mouseDown = false;
            selected = true;
        }

        private void onDeselect()
        {
            if (!selected) return;

            selected = false;
        }

        #region Event system callbacks

        private void onObjectSelected()
        {
            // Don't deselect when ctrl is held
            if (Input.GetKey(KeyCode.LeftControl)) return;

            onDeselect();
        }

        #endregion
    }
}
