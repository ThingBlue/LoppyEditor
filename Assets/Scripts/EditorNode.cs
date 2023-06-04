using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoppyEditor
{
    [Serializable]
    public class EditorNodeData
    {
        public string name;
        public string region;
        public string type;
        public int entranceCount;
        public List<EditorNode> connections;

        public EditorNodeData(string name, string region, string type, int entranceCount, List<EditorNode> connections)
        {
            this.name = name;
            this.region = region;
            this.type = type;
            this.entranceCount = entranceCount;
            this.connections = connections;
        }
    }

    public class EditorNode : MonoBehaviour
    {
        #region Inspector members

        public TMP_Text nameText;

        public SpriteRenderer sprite;
        public Color defaultColour;
        public Color hoverColour;
        public Color pressedColour;
        public Color selectedColour;

        public Color disabledColour;

        #endregion

        public EditorNodeData nodeData;

        public bool selected = false;
        private bool mouseDown = false;
        public bool mouseHover = false;
        public bool massSelectHover = false;

        public void setNodeData(EditorNodeData newNodeData) { nodeData = newNodeData; }
        public void setName(string name) { nodeData.name = name; }
        public void setRegion(string region) { nodeData.region = region; }
        public void setType(string type) { nodeData.type = type; }
        public void setEntranceCount(int entranceCount) { nodeData.entranceCount = entranceCount; }
        public void addConnection(EditorNode other) { nodeData.connections.Add(other); }
        public void removeConnection(EditorNode other) { nodeData.connections.Remove(other); }
        public void clearConnections()
        {
            // Remove connections on to this node from other nodes
            foreach (EditorNode other in nodeData.connections)
            {
                other.removeConnection(this);
            }
            nodeData.connections.Clear();
        }
        public bool hasConnection(EditorNode other) { return nodeData.connections.Contains(other); }

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.objectSelectedEvent.AddListener(onObjectSelected);

            nodeData = new EditorNodeData("New node", "", "", 0, new List<EditorNode>());
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
            if (selected) sprite.color = selectedColour;
            else if (mouseHover && mouseDown) sprite.color = pressedColour;
            else if (mouseHover || massSelectHover) sprite.color = hoverColour;
            else sprite.color = defaultColour;
        }

        private void OnMouseDown()
        {
            mouseDown = true;
        }

        private void OnMouseUp()
        {
            if (!selected && mouseDown && mouseHover) onSelect();
        }

        private void OnMouseEnter()
        {
            mouseHover = true;
        }

        private void OnMouseExit()
        {
            mouseHover = false;
        }

        private void OnMouseDrag()
        {
            if (selected)
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            }
        }

        private void onSelect()
        {
            if (selected) return;

            EventManager.instance.objectSelectedEvent.Invoke();

            mouseDown = false;
            selected = true;

            // Show info in inspector
            InspectorManager.instance.setCurrentNode(this);
        }

        private void onDeselect()
        {
            if (!selected) return;

            selected = false;
            if (InspectorManager.instance.currentNode == this) InspectorManager.instance.resetCurrentNode();
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
