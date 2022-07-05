using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ElementMover : UIBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RuntimeReorderableList ListRoot;
    public RectTransform ElementRoot;
    private bool isSelected = false;
    private Vector2 _dragOffset;

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;

        var item = ListRoot.elementTransforms.FirstOrDefault(x => x.Value == ElementRoot).Key;
        var newIndex = GetTargetIndex();
        ListRoot.elements.Move(ListRoot.elements.IndexOf(item), newIndex >= ListRoot.elements.Count ? ListRoot.elements.Count - 1 : newIndex);
        ListRoot.DrawMoverLine(false, 0);
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
            var newIndex = GetTargetIndex();
            ListRoot.DrawMoverLine(true, newIndex);
        }
    }
    
    public int GetTargetIndex() => (int) -(ElementRoot.anchoredPosition.y / ListRoot.ElementObject.sizeDelta.y);
}
