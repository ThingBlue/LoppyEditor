using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loppy.Editor
{
    public class EditorConnector : MonoBehaviour
    {
        public bool connected = false;
        public List<GameObject> connectedNodeObjects;

        public void addConnectedNode(GameObject node) { connectedNodeObjects.Add(node); }

        private void FixedUpdate()
        {
            if (connected)
            {
                // Destroy connector if either of its connected nodes are destroyed
                foreach (GameObject nodeObject in connectedNodeObjects)
                {
                    if (!nodeObject) Destroy(gameObject);
                }
            }
        }
    }
}
