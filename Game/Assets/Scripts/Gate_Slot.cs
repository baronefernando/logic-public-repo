using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gate_Slot : MonoBehaviour, IDropHandler
{
    public Sprite[] gates = new Sprite[6];
    
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null && GetComponent<Image>().sprite == null)
        {
            if(eventData.pointerDrag.name.Equals("not"))
            {
                GetComponent<Image>().sprite = gates[0];
                eventData.pointerDrag.transform.SetParent(transform);
            }
            else if(eventData.pointerDrag.name.Equals("and"))
            {
                GetComponent<Image>().sprite = gates[1];
                eventData.pointerDrag.transform.SetParent(transform);
            } 
            else if(eventData.pointerDrag.name.Equals("or"))
            {
                GetComponent<Image>().sprite = gates[2];
                eventData.pointerDrag.transform.SetParent(transform);
            } 
        }
    }
}
