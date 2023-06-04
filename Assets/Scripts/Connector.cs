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

        #endregion

        public bool connected = false;
        public List<GameObject> connectedNodeObjects;

        public void addConnectedNode(GameObject node) { connectedNodeObjects.Add(node); }

        private void FixedUpdate()
        {
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

        private void OnMouseDown()
        {
            Debug.Log("LINE CLICKED");
        }
    }
}
