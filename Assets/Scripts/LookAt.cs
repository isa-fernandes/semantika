using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    TextMeshPro textMeshPro;
    Transform textMeshTransform;

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshTransform = textMeshPro.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textMeshTransform.position - Camera.main.transform.position != Vector3.zero)
        {
            textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - Camera.main.transform.position);
        }
        
    }
}
