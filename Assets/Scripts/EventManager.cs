using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * STEPS FOR USING UNITY EVENTS:
 *     1. ADD NEW EVENT
 *         public UnityEvent eventName;
 *     2. INITIALIZE EVENT
 *         if (eventName == null) eventName = new UnityEvent();
 *     3. ADD LISTENER TO EVENT
 *         EventManager.instance.eventName.AddListener(eventCallbackName);
 *     4. INVOKE EVENT CALLBACKS
 *         EventManager.instance.eventName.Invoke();
 */

namespace LoppyEditor
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager instance;

        #region Events

        public UnityEvent nodeSelectedEvent;
        public UnityEvent connectorSelectedEvent;

        #endregion

        private void Awake()
        {
            // Singleton
            if (instance == null) instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            // Initialize events
            if (nodeSelectedEvent == null) nodeSelectedEvent = new UnityEvent();
            if (connectorSelectedEvent == null) connectorSelectedEvent = new UnityEvent();
        }
    }
}
