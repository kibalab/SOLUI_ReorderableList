using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


[ExecuteAlways]
public class RuntimeReorderableList : MonoBehaviour
{
    private ObservableCollection<object> elements;

    private Dictionary<object, RectTransform> elementTransforms;

    public RectTransform ElementObject;

    public RectTransform ContentsPanel;

    public bool add = false;

    public void Update()
    {
        if (add)
        {
            add = false;
            if(elements != null) elements.Add(Random.value);
            else elements = new ObservableCollection<object>() { Random.value };
        }
    }

    private void Start()
    {
        elements = new ObservableCollection<object>();
        elementTransforms = new Dictionary<object, RectTransform>();
        elements.CollectionChanged += OnListChanged;
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
                foreach (var item in args.NewItems)
                {
                    elementTransforms.Remove(item);
                }
                break;
            case NotifyCollectionChangedAction.Replace :
                break;
            case NotifyCollectionChangedAction.Reset :
                break;
        }
        UpdatePanelScale();
    }

    public RectTransform SpawnElement()
    {
        var Element = Instantiate(ElementObject, ContentsPanel);
        Element.anchoredPosition = new Vector2(0,-ListLastPos);
        return Element;
    }

    public void UpdatePanelScale() => ContentsPanel.sizeDelta = new Vector2(0, ListLastPos);

    public float ListLastPos { get => ElementObject.sizeDelta.y * elements.Count - 1; }
}
