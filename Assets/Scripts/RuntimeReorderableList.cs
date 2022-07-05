using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[ExecuteAlways]
public class RuntimeReorderableList : MonoBehaviour
{
    private ObservableCollection<object> elements;

    private Dictionary<object, RectTransform> elementTransforms;

    public RectTransform ElementObject;

    public RectTransform ContentsPanel;

    public Scrollbar VerticalScrollbar;

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
                    elementTransforms.Add(item, SpawnElement());
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
                break;
            case NotifyCollectionChangedAction.Reset :
                foreach (var item in elementTransforms)
                    Destroy(item.Value.gameObject);
                elementTransforms.Clear();
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
    

    public RectTransform SpawnElement()
    {
        var Element = Instantiate(ElementObject, ContentsPanel);
        Element.anchoredPosition = new Vector2(0,ElementObject.sizeDelta.y-ListLastPos);
        return Element;
    }

    public void UpdatePanelScale() => ContentsPanel.sizeDelta = new Vector2(ContentsPanel.sizeDelta.x, ListLastPos);

    public void UpdateScrollSize() => VerticalScrollbar.size = ((RectTransform) transform).sizeDelta.y / ContentsPanel.sizeDelta.y;

    public float ListLastPos { get => ElementObject.sizeDelta.y * elements.Count - 1; }
}
