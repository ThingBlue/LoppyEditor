using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Loppy.Editor
{
    public class PanelResizer : MonoBehaviour,
        IDragHandler
    {
        #region Inspector members

        public RectTransform panelRectTransform;
        public RectTransform canvasRectTransform;

        #endregion

        public void OnDrag(PointerEventData eventData)
        {
            // Resize panel to follow mouse
            float newSize = Mathf.Clamp((Screen.width - Input.mousePosition.x) / canvasRectTransform.localScale.x, 50, Screen.width / canvasRectTransform.localScale.x);
            panelRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize);
        }
    }
}
