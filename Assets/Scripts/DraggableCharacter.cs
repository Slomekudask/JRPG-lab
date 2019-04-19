using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCharacter : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public float gridSize = 1f;

    bool isDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.enterEventCamera != null)
        {
            Plane characterPlane = new Plane(transform.up, transform.position);
            var ray = eventData.enterEventCamera.ScreenPointToRay(eventData.position);
            if (characterPlane.Raycast(ray, out float enter))
            {
                var hitPoint = ray.origin + ray.direction * enter;
                //transform.position = transform.position + Vector3.ProjectOnPlane(eventData.pointerCurrentRaycast.worldPosition - transform.position, Vector3.up);

                var delta = hitPoint - transform.position;

                delta.x = Mathf.Round(delta.x / gridSize) * gridSize;
                //hitPoint.y = Mathf.Round(hitPoint.y / gridSize) * gridSize;
                delta.z = Mathf.Round(delta.z / gridSize) * gridSize;

                var newPos = transform.position + delta;

                var collider = GetComponent<CapsuleCollider>();

                if (collider != null )
                {
                    var checkLocation = collider.bounds.center + delta;
                    if (Physics.CheckSphere(checkLocation, collider.radius))
                    {
                        Debug.DrawLine(transform.position, checkLocation, Color.red);
                    }
                    else
                        transform.position += delta;
                }
                else
                    transform.position += delta;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}



