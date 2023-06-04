using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LoppyEditor
{
    public class CameraController : MonoBehaviour
    {
        #region Inspector members

        public float zoomSpeed;
        public float zoomMin = 0.1f;
        public float zoomMax = 10f;

        #endregion

        private Vector3 panOrigin;
        private Vector3 zoomOrigin;

        private void LateUpdate()
        {
            // Handle camera panning
            if (Input.GetMouseButtonDown(2))
            {
                panOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(2))
            {
                Vector3 panDelta = panOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += panDelta;
            }

            // Handle camera zooming
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                // Get mouse position before zooming
                zoomOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Handle zooming
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, zoomMin, zoomMax);

                // Move camera based on mouse position delta after zooming
                Vector3 zoomPositionDelta = zoomOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += zoomPositionDelta;
            }
        }

        public void resetCamera()
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            Camera.main.orthographicSize = 5;
        }
    }
}
