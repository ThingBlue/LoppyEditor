using JetBrains.Annotations;
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
        public int id;
        public string name;
        public string region;
        public string type;
        public bool terminal;
        public int entranceCount;
        public List<int> connections;
        public Vector2 editorPosition;

        public EditorNodeData(int id, string name, string region, string type, bool terminal, int entranceCount, List<int> connections, Vector2 editorPosition)
        {
            this.id = id;
            this.name = name;
            this.region = region;
            this.type = type;
            this.terminal = terminal;
            this.entranceCount = entranceCount;
            this.connections = connections;
            this.editorPosition = editorPosition;
        }

        public EditorNodeData(EditorNodeData other)
        {
            this.id = other.id;
            this.name = other.name;
            this.region = other.region;
            this.type = other.type;
            this.terminal = other.terminal;
            this.entranceCount = other.entranceCount;
            this.connections = other.connections;
            this.editorPosition = other.editorPosition;
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
        public List<EditorNode> connectedNodes;

        public bool selected = false;
        private bool mouseDown = false;
        public bool mouseHover = false;
        public bool massSelectHover = false;

        public void setNodeData(EditorNodeData newNodeData) { nodeData = newNodeData; }
        public void setName(string name) { nodeData.name = name; }
        public void setRegion(string region) { nodeData.region = region; }
        public void setType(string type) { nodeData.type = type; }
        public void setEntranceCount(int entranceCount) { nodeData.entranceCount = entranceCount; }
        public void addConnection(EditorNode other)
        {
            connectedNodes.Add(other);
            if (!nodeData.connections.Contains(other.nodeData.id)) nodeData.connections.Add(other.nodeData.id);
        }
        public void removeConnection(EditorNode other)
        {
            connectedNodes.Remove(other);
            nodeData.connections.Remove(other.nodeData.id);
        }
        public void clearConnections()
        {
            // Remove connections on to this node from other nodes
            foreach (EditorNode other in connectedNodes)
            {
                other.removeConnection(this);
            }
            connectedNodes.Clear();

            // Clear connection data
            nodeData.connections.Clear();
        }
        public bool hasConnection(EditorNode other) { return connectedNodes.Contains(other); }

        public void initializeNodeData()
        {
            nodeData = new EditorNodeData(gameObject.GetInstanceID(), "New node", "", "", true, 0, new List<int>(), Vector2.zero);
        }

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.objectSelectedEvent.AddListener(onObjectSelected);
        }

        private void Update()
        {
            // Set editor position
            nodeData.editorPosition = transform.position;

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

            // Set displat text
            nameText.text = nodeData.name;
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
            if (InspectorManager.instance.currentNode == this)
            {
                InspectorManager.instance.resetCurrentNode();
            }
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
