using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaceContent : MonoBehaviour {

    public ARRaycastManager raycastManager;
    public GraphicRaycaster raycaster;

    bool wasDoubleTouch;

    private float initialDistance;
    private Vector3 initialScale;

    void Update() {

        if (Input.GetMouseButtonDown(0)) {
            wasDoubleTouch = false;
        }

        if (Input.GetMouseButtonDown(1)) {
            wasDoubleTouch = true;
        }

        if (Input.GetMouseButtonUp(0) && !IsClickOverUI() && !wasDoubleTouch) {
        
            List<ARRaycastHit> hitPoints = new List<ARRaycastHit>();
            raycastManager.Raycast(Input.mousePosition, hitPoints, TrackableType.Planes);

            if (hitPoints.Count > 0) {
                Pose pose = hitPoints[0].pose;
                transform.rotation = pose.rotation;
                transform.position = pose.position;
            }
        }

        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0); 
            var touchOne = Input.GetTouch(1);

            // if one of the touches Ended or Canceled do nothing
            if(touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled  
            || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled) 
            {
                return;
            }

            // It is enough to check whether one of them began since we
            // already excluded the Ended and Canceled phase in the line before
            if(touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                // track the initial values
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            // else now is any other case where touchZero and/or touchOne are in one of the states
            // of Stationary or Moved
            else
            {
                // otherwise get the current distance
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                // A little emergency brake ;)
                if(Mathf.Approximately(initialDistance, 0)) return;

                // get the scale factor of the current distance relative to the inital one
                var factor = currentDistance / initialDistance;

                // apply the scale
                // instead of a continuous addition rather always base the 
                // calculation on the initial and current value only
                transform.localScale = initialScale * factor;
            }
        }
    }

    bool IsClickOverUI() {
        //dont place content if pointer is over ui element
        PointerEventData data = new PointerEventData(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(data, results);
        return results.Count > 0;
    }
}