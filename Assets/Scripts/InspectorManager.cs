using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Loppy.Editor
{
    public class InspectorManager : MonoBehaviour
    {
        public static InspectorManager instance;

        #region Inspector members

        public GameObject inspectorPanel;
        public TMP_InputField nameInputField;
        public TMP_InputField regionInputField;
        public TMP_InputField typeInputField;
        public TMP_InputField entranceCountInputField;

        #endregion

        private EditorNode currentNode = null;

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
            currentNode.nameText.text = currentNode.nodeData.name;

            nameInputField.text = currentNode.nodeData.name;
            regionInputField.text = currentNode.nodeData.region;
            typeInputField.text = currentNode.nodeData.type;
            entranceCountInputField.text = currentNode.nodeData.entranceCount.ToString();
        }

        public void setCurrentNode(EditorNode node)
        {
            currentNode = node;

            // Open inspector
            inspectorPanel.SetActive(true);
            displayNodeData();
        }

        public void resetCurrentNode()
        {
            // Apply whatever is in the inspector before closing it
            setNodeData();

            // Close inspector
            currentNode = null;
            inspectorPanel.SetActive(false);
        }

        public void setNodeData()
        {
            currentNode.nodeData.name = nameInputField.text;
            currentNode.nodeData.region = regionInputField.text;
            currentNode.nodeData.type = typeInputField.text;
            currentNode.nodeData.entranceCount = int.Parse(entranceCountInputField.text);

            // Update node and inspector to reflect changes
            displayNodeData();
        }
    }
}
