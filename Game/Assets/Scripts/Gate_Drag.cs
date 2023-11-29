using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gate_Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        transform.position = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y, 0.0f);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(transform.parent.name.Contains("node"))
        {
            GameObject.Find(transform.parent.name).GetComponent<Image>().sprite = null;
        }

        transform.SetParent(GameObject.Find("logic_mini_game").transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Pointer Down");
    }
}
