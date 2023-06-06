using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoppyEditor
{
    public class InspectorManager : MonoBehaviour
    {
        public static InspectorManager instance;

        #region Inspector members

        public GameObject inspectorPanel;
        public TMP_InputField nameInputField;
        public TMP_InputField regionInputField;
        public TMP_InputField typeInputField;
        public Toggle terminalToggle;
        public TMP_InputField entranceCountInputField;

        #endregion

        public EditorNode currentNode = null;
        public bool triggerUICallbacks = true;

        private void Awake()
        {
            // Singleton
            if (instance == null) instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            inspectorPanel.SetActive(false);
        }

        public void displayNodeData()
        {
            nameInputField.text = currentNode.nodeData.name;
            regionInputField.text = currentNode.nodeData.region;
            typeInputField.text = currentNode.nodeData.type;
            terminalToggle.isOn = currentNode.nodeData.terminal;
            entranceCountInputField.text = currentNode.nodeData.entranceCount.ToString();
        }

        public void setCurrentNode(EditorNode node)
        {
            currentNode = node;

            // Open inspector
            inspectorPanel.SetActive(true);
            triggerUICallbacks = false;
            displayNodeData();
            triggerUICallbacks = true;
        }

        public void resetCurrentNode()
        {
            // Apply whatever is in the inspector before closing it
            //setNodeData();

            // Close inspector
            currentNode = null;
            inspectorPanel.SetActive(false);
        }

        public void setNodeData()
        {
            // Why do I have to do this
            if (!triggerUICallbacks) return;

            currentNode.nodeData.name = nameInputField.text;
            currentNode.nodeData.region = regionInputField.text;
            currentNode.nodeData.type = typeInputField.text;
            currentNode.nodeData.terminal = terminalToggle.isOn;
            currentNode.nodeData.entranceCount = int.Parse(entranceCountInputField.text);

            // Update node and inspector to reflect changes
            displayNodeData();
        }
    }
}
