using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[ExecuteAlways]
public class RuntimeReorderableList : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ObservableCollection<object> elements;

    public Dictionary<object, RectTransform> elementTransforms;

    
    public RectTransform ElementObject;

    public RectTransform ContentsPanel;

    public RectTransform MoveerLine;

    public Scrollbar VerticalScrollbar;

    public float ScrollSensitivity = 0.1f;
    
    public float ScrollElastic = 2;
    
    public bool mouse_over = false;

    public bool add = false;
    public bool remove = false;
    
    public void Update()
    {
        if (add)
        {
            add = false;
            if(elements != null) elements.Add(Random.value);
            else elements = new ObservableCollection<object>() { Random.value };
        }

        if (remove)
        {
            remove = false;
            if(elements != null) elements.RemoveAt(0);
        }

        VerticalScrollbar.size = GetPanelScale();
        VerticalScrollbar.value = GetPanelPosDelta() + (mouse_over ? -Input.mouseScrollDelta.y * ScrollSensitivity : 0);

        if (VerticalScrollbar.value > 1)
            VerticalScrollbar.value = Mathf.Lerp(VerticalScrollbar.value, 1, Time.deltaTime * ScrollElastic);
        else if (VerticalScrollbar.value < 0)
            VerticalScrollbar.value = Mathf.Lerp(VerticalScrollbar.value, 0, Time.deltaTime * ScrollElastic);

        VerticalScrollbar.gameObject.SetActive(VerticalScrollbar.size != 1);

    }

    private void Start()
    {
        
        elements = new ObservableCollection<object>();
        elementTransforms = new Dictionary<object, RectTransform>();
        elements.CollectionChanged += OnListChanged;
        VerticalScrollbar.onValueChanged.AddListener(OnScrollChanged);
    }

    public void OnListChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add :
                foreach (var item in args.NewItems)
                {
                    elementTransforms.Add(item, SpawnElement(item));
                }
                break;
            case NotifyCollectionChangedAction.Remove :
                foreach (var item in args.OldItems)
                {
                    var removeTarget = elementTransforms[item];
                    elementTransforms.Remove(item);
                    Destroy(removeTarget.gameObject);
                }
                ReorderElementObjects();
                break;
            case NotifyCollectionChangedAction.Replace :
                foreach (var item in args.OldItems)
                {
                    var removeTarget = elementTransforms[item];
                    elementTransforms.Remove(item);
                    Destroy(removeTarget.gameObject);
                }
                foreach (var item in args.NewItems)
                {
                    elementTransforms.Add(item, SpawnElement(item));
                }
                ReorderElementObjects();
                break;
            case NotifyCollectionChangedAction.Reset :
                foreach (var item in elementTransforms)
                    Destroy(item.Value.gameObject);
                elementTransforms.Clear();
                break;
            case NotifyCollectionChangedAction.Move :
                ReorderElementObjects();
                break;
        }
        UpdatePanelScale();
        UpdateScrollSize();
    }

    public void OnScrollChanged(float value)
    {
        var pos = ContentsPanel.anchoredPosition;
        pos.y = (ContentsPanel.sizeDelta.y - ((RectTransform)transform).sizeDelta.y) * value;
        ContentsPanel.anchoredPosition = pos;
    }

    public void ReorderElementObjects()
    {
        var i = 0;
        foreach (var element in elements)
        {
            elementTransforms[element].anchoredPosition = new Vector2(0, -ElementObject.sizeDelta.y * i);
            i++;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = !eventData.hovered.Contains(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = !eventData.hovered.Contains(gameObject);
    }

    public RectTransform SpawnElement(object Item)
    { 
        var Element = Instantiate(ElementObject, ContentsPanel);
        Element.anchoredPosition = new Vector2(0,ElementObject.sizeDelta.y-ListLastPos);
        Element.GetComponentInChildren<Text>().text = Item.ToString();
        Element.GetComponentInChildren<ElementMover>().ListRoot = this;
        return Element;
    }

    public void DrawMoverLine(bool b, int targetIndex)
    {
        MoveerLine.gameObject.SetActive(b);
        if (!b) return;
        
        MoveerLine.anchoredPosition = new Vector2(0, -ElementObject.sizeDelta.y * targetIndex);
        MoveerLine.SetAsLastSibling();
    }

    public float GetPanelScale() => 1 - (ContentsPanel.sizeDelta.y - ((RectTransform) transform).sizeDelta.y) /  ContentsPanel.sizeDelta.y;

    public float GetPanelPosDelta() => ContentsPanel.anchoredPosition.y / (ContentsPanel.sizeDelta.y - ((RectTransform) transform).sizeDelta.y);

    public void UpdatePanelScale() => ContentsPanel.sizeDelta = new Vector2(ContentsPanel.sizeDelta.x, ListLastPos);

    public void UpdateScrollSize() => VerticalScrollbar.size = ((RectTransform) transform).sizeDelta.y / ContentsPanel.sizeDelta.y;

    public float ListLastPos { get => ElementObject.sizeDelta.y * elements.Count - 1; }
}
