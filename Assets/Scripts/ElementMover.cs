using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ElementMover : UIBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform ElementRoot;
    public bool isSelected = false;
    public Vector2 _dragOffset;

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isSelected = true;
        var draggedTarget = eventData.position;
        _dragOffset = ElementRoot.position - Input.mousePosition;
    }

    private void Update()
    {
        if (isSelected)
        {
            ElementRoot.position = (Vector2)Input.mousePosition + _dragOffset;
        }
    }
}
