using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestWordPos : MonoBehaviour
{
    [SerializeField] TMP_FontAsset font;

    // Start is called before the first frame update
    void Start()
    {
        GameObject text1 = AddTextToParent(gameObject.transform, "chair", "의사");
        RectTransform rect1 = text1.GetComponent<RectTransform>();
        rect1.pivot = new Vector2(rect1.pivot.x, 0);
        text1.GetComponent<BoxCollider>().center = new Vector3(0, 2.5f, 0);

        GameObject text2 = AddTextToParent(gameObject.transform, "의사", "chair");
        RectTransform rect2 = text2.GetComponent<RectTransform>();
        rect2.pivot = new Vector2(rect2.pivot.x, 1);
        text2.GetComponent<BoxCollider>().center = new Vector3(0, -2.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject AddTextToParent(Transform parent, string translated, string original)
    {
        GameObject textObj = new GameObject("textObj" + original);

        textObj.AddComponent<LookAt>();

        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.text = translated;
        textMesh.fontSize = 14;
        textMesh.font = font;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.ForceMeshUpdate();

        textObj.AddComponent<BoxCollider>();
        textObj.GetComponent<BoxCollider>().size = new Vector3(4+ textMesh.textBounds.extents.x, 2, 0.1f);
        
        TextInfo textInfo = textObj.AddComponent<TextInfo>();
        textInfo.translated = translated;
        textInfo.original = original;

        textObj.transform.parent = parent;

        RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.02f, 0.02f, 1f);

        return textObj;
    }
}
