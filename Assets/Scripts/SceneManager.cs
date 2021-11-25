using Google.Cloud.Vision.V1;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using Vuforia;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;

public class SceneManager : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] Button screenShotBtn;
    [SerializeField] TMP_Dropdown TMPDropdown;
    [SerializeField] TMP_FontAsset font;

    int resWidth = Screen.width;
    int resHeight = Screen.height;

    static Texture2D screenShot;

    UnityEvent foundTargetEvent;

    string apiKey;

    void Start()
    {
        if (foundTargetEvent == null)
        {
            foundTargetEvent = new UnityEvent();
        }

        StartCoroutine(PostRequest("https://translation.googleapis.com/language/translate/v2/languages?key="+apiKey, 2));
        
    }

    void OnGUI()
    {
        screenShotBtn.interactable = true;
    }

    IEnumerator PostRequest(string url, int requestType, string json="", Transform parent=null, string annotaionName="")
    {
        var uwr = new UnityWebRequest(url, "POST");
        if (requestType == 0)
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.SetRequestHeader("Content-Type", "application/json");
        }
        
        uwr.downloadHandler = new DownloadHandlerBuffer();
        

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            textMeshPro.SetText("Error While Sending: " + uwr.error);
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            textMeshPro.SetText("Received: " + uwr.downloadHandler.text);
            Debug.Log("Received: " + uwr.downloadHandler.text);
            JObject requestresponse = JObject.Parse(uwr.downloadHandler.text);
            
            if (requestType == 0)
            {
                try
                {
                    IList<JToken> results = requestresponse["responses"][0]["localizedObjectAnnotations"].Children().ToList();

                    IList<LocalizedObjectAnnotation> searchResults = new List<LocalizedObjectAnnotation>();
                    foreach (JToken result in results)
                    {
                        // JToken.ToObject is a helper method that uses JsonSerializer internally
                        LocalizedObjectAnnotation searchResult = result.ToObject<LocalizedObjectAnnotation>();
                        searchResults.Add(searchResult);
                    }
                    DetectObjects(searchResults);
                }
                catch (Exception e)
                {
                    textMeshPro.SetText("No results found.");
                }
            } else if (requestType == 1)
            {
                try
                {
                    Debug.Log(requestresponse["data"]["translations"][0]["translatedText"]);
                    string translatedText = (string) requestresponse["data"]["translations"][0]["translatedText"];
                    GameObject text1 = AddTextToParent(parent, translatedText, annotaionName);
                    RectTransform rect1 = text1.GetComponent<RectTransform>();
                    rect1.pivot = new Vector2(rect1.pivot.x, 0);
                    text1.GetComponent<BoxCollider>().center = new Vector3(0, 2.5f, 0);

                    GameObject text2 = AddTextToParent(parent, annotaionName, translatedText);
                    RectTransform rect2 = text2.GetComponent<RectTransform>();
                    rect2.pivot = new Vector2(rect2.pivot.x, 1);
                    text2.GetComponent<BoxCollider>().center = new Vector3(0, -2.5f, 0);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            } else if (requestType == 2)
            {
                try
                {
                    IList<JToken> results = requestresponse["data"]["languages"].Children().ToList();
                    
                    foreach (JToken result in results)
                    {
                        // JToken.ToObject is a helper method that uses JsonSerializer internally
                        Debug.Log(result["language"]);
                        string searchResult = result["language"].ToObject<string>();
                        TMPDropdown.options.Add(new TMP_Dropdown.OptionData() { text = searchResult });
                        if (searchResult == "ko")
                        {
                            TMPDropdown.value = TMPDropdown.options.Count - 1;
                        }
                    }


                } catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            
            
        }
    }

    public void TakePicture ()
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        arCamera.targetTexture = rt;
        screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        arCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        arCamera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        string json = "{'requests':[{'image':{'content':'" + System.Convert.ToBase64String(screenShot.EncodeToJPG()) + "'},'features':[{'type':'OBJECT_LOCALIZATION','maxResults':1}]}]}";
        Debug.Log(json);
        StartCoroutine(PostRequest("https://vision.googleapis.com/v1/images:annotate?key="+apiKey, 0, json));
    }

    public static Rect FromBoundingPoly(BoundingPoly poly, int width, int height)
    {
        if (poly == null)
        {
            Debug.Log("Null poly");
            return new Rect();
        }
        var vertices = poly.NormalizedVertices;
        if (vertices.Count != 4)
        {
            Debug.Log("Count different from 4");
            return new Rect();
        }
        if (vertices[0].Y == vertices[1].Y &&
            vertices[0].X <= vertices[1].X &&
            vertices[1].X == vertices[2].X &&
            vertices[1].Y <= vertices[2].Y &&
            vertices[2].Y == vertices[3].Y &&
            vertices[2].X >= vertices[3].X &&
            vertices[3].X == vertices[0].X &&
            vertices[3].Y >= vertices[0].Y)
        {
            return new Rect((int)(vertices[0].X * width), (int)(vertices[0].Y * height), (int)(vertices[2].X * width), (int)(vertices[2].Y * height));
        }
        return new Rect();
    }

    public void FoundObject (ObserverBehaviour observerbehavour, TargetStatus status)
    {
        textMeshPro.SetText("tracking " + observerbehavour.gameObject.name);
    }

    GameObject AddTextToParent (Transform parent, string translated, string original)
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
        textObj.GetComponent<BoxCollider>().size = new Vector3(4 + textMesh.textBounds.extents.x, 2, 0.1f);

        TextInfo textInfo = textObj.AddComponent<TextInfo>();
        textInfo.translated = translated;
        textInfo.original = original;

        textObj.transform.parent = parent;

        RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.02f, 0.02f, 1f);

        return textObj;
    }

    void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Transform[] children = vb.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }

    public object DetectObjects(IList<LocalizedObjectAnnotation> searchResults)
    {
        foreach (LocalizedObjectAnnotation annotation in searchResults)
        {
            string poly = string.Join(" - ", annotation.BoundingPoly.Vertices.Select(v => $"({v.X}, {v.Y})"));
            Debug.Log(
                $"Name: {annotation.Name}; ID: {annotation.Mid}; Score: {annotation.Score}; Bounding poly: {poly}");
            textMeshPro.SetText($"Name: {annotation.Name}; Bounding poly: {poly}");
            NormalizedVertex bound1 = annotation.BoundingPoly.NormalizedVertices.ElementAt(0);
            NormalizedVertex bound2 = annotation.BoundingPoly.NormalizedVertices.ElementAt(2);
            Rect rect = FromBoundingPoly(annotation.BoundingPoly, screenShot.width, screenShot.height);
            Debug.Log(screenShot.width + " - " + screenShot.height);
            Debug.Log(rect.ToString());
            Color[] pixels = screenShot.GetPixels((int)rect.xMin,(int)rect.yMin,(int)(rect.width-rect.xMin),(int)(rect.height-rect.yMin));
            Texture2D cropImg = new Texture2D((int)(rect.width - rect.xMin), (int)(rect.height - rect.yMin), TextureFormat.RGB24,false);
            cropImg.SetPixels(pixels);

            var mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
            cropImg,
            0.2f,
            annotation.Name);

            Debug.Log(TMPDropdown.options[TMPDropdown.value].text);
            StartCoroutine(PostRequest("https://translation.googleapis.com/language/translate/v2?key="+apiKey+"&source=en&format=text&target="+TMPDropdown.options[TMPDropdown.value].text+"&q="+annotation.Name, 1, parent:mTarget.transform, annotaionName:annotation.Name));

            // add the Default Observer Event Handler to the newly created game object
            mTarget.gameObject.AddComponent<TurnOffBehaviour>();
            DefaultObserverEventHandler observer = mTarget.gameObject.AddComponent<DefaultObserverEventHandler>();
            observer.StatusFilter = DefaultObserverEventHandler.TrackingStatusFilter.Tracked;
            mTarget.OnTargetStatusChanged += FoundObject;
            
        }
        return searchResults;
    }


}

[Serializable]
public class Response
{
    public LocalizedObjectAnnotation[] localizedObjectAnnotations;
}

[Serializable]
public class Responses
{
    public Response[] responses;
}
