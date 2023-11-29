using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Toolbox_Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            if(eventData.pointerDrag.name.Equals("not"))
            {
                eventData.pointerDrag.transform.SetParent(transform);
                eventData.pointerDrag.transform.SetAsFirstSibling();
            }
            else if(eventData.pointerDrag.name.Equals("or"))
            {
                eventData.pointerDrag.transform.SetParent(transform);
                eventData.pointerDrag.transform.SetAsLastSibling();
            }
            else if(eventData.pointerDrag.name.Equals("and"))
            {
                eventData.pointerDrag.transform.SetParent(transform);
                eventData.pointerDrag.transform.SetSiblingIndex(1);
            }
        }
    }
}
