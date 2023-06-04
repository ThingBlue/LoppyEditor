using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoppyEditor
{
    public class Connector : MonoBehaviour
    {
        #region Inspector members

        public float lineEndMargin = 0.5f;

        public LineRenderer lineRenderer;
        public PolygonCollider2D polygonCollider;

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
        public bool massSelectHover = false;

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

            if (connected)
            {
                // Follow movement of nodes
                lineRenderer.SetPosition(0, connectedNodeObjects[0].transform.position + (connectedNodeObjects[1].transform.position - connectedNodeObjects[0].transform.position).normalized * lineEndMargin);
                lineRenderer.SetPosition(1, connectedNodeObjects[1].transform.position + (connectedNodeObjects[0].transform.position - connectedNodeObjects[1].transform.position).normalized * lineEndMargin);
            }
        }

        private void FixedUpdate()
        {
            // Set colour
            Color colourToSet = defaultColour;

            if (selected) colourToSet = selectedColour;
            else if (mouseHover && mouseDown) colourToSet = pressedColour;
            else if (mouseHover || massSelectHover) colourToSet = hoverColour;
            else colourToSet = defaultColour;

            lineRenderer.startColor = colourToSet;
            lineRenderer.endColor = colourToSet;

            if (connected)
            {
                // Update collider
                List<Vector2> colliderPoints = calculateColliderPoints();
                polygonCollider.SetPath(0, colliderPoints.ConvertAll(point => (Vector2)transform.InverseTransformPoint(point)));

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
            }
        }

        private List<Vector2> calculateColliderPoints()
        {
            // Get all positions on the line renderer
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            // Get the width of the Line
            float width = lineRenderer.startWidth;

            // m = (y2 - y1) / (x2 - x1)
            float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
            float deltaX = (width / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
            float deltaY = (width / 2f) * (1 / Mathf.Pow(1 + m * m, 0.5f));

            // Calculate the fffset from each point to the collision vertex
            Vector3[] offsets = new Vector3[2];
            offsets[0] = new Vector3(-deltaX, deltaY);
            offsets[1] = new Vector3(deltaX, -deltaY);

            // Generate the collider's vertices
            List<Vector2> colliderPositions = new List<Vector2>{
                positions[0] + offsets[0],
                positions[1] + offsets[0],
                positions[1] + offsets[1],
                positions[0] + offsets[1]
            };

            return colliderPositions;
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "MassSelectionBox") massSelectHover = true;
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.tag == "MassSelectionBox") massSelectHover = false;
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
