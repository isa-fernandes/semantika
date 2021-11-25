using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    void Update()
    {
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                GameObject parent = raycastHit.collider.gameObject.transform.parent.gameObject;

                Transform child = parent.GetComponentInChildren<Transform>();
                TextMeshPro textMeshPro = child.gameObject.GetComponent<TextMeshPro>();
                TextInfo textInfo = child.gameObject.GetComponent<TextInfo>();

                if (textInfo.isOriginal)
                {
                    textMeshPro.text = textInfo.translated;
                } else
                {
                    textMeshPro.text = textInfo.original;
                }

                textInfo.isOriginal = !textInfo.isOriginal;
            }
        }
    }
}
