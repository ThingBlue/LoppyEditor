using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoppyEditor
{
    public class Connector : MonoBehaviour
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

                // Follow movement of nodes
                GetComponent<LineRenderer>().SetPosition(0, connectedNodeObjects[0].transform.position);
                GetComponent<LineRenderer>().SetPosition(1, connectedNodeObjects[1].transform.position);
            }
        }
    }
}
