using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;



public class TranslationManager : MonoBehaviour
{
    public TMP_FontAsset polishFont;

    [SerializeField]
    private NewDict translation;

    [SerializeField]
    private SoundChange soundChange;

    [SerializeField]
    private ImageChange imageChange;

    private Dictionary<TextMeshProUGUI, UiElementsInfo> previousValuesCollection = new Dictionary<TextMeshProUGUI, UiElementsInfo>();
    private Dictionary<TextMeshProUGUI, string> UICollection;

    private Dictionary<AudioSource,AudioClip> AudioChangeCollection;

    private Dictionary<GameObject, UnityEngine.Sprite> ImageChangeCollection;

    public static TranslationManager instance;

#if UnityEditor
    [Button(nameof(GetAllTextTMPInScene))]
    public bool buttonField;

    public void GetAllTextTMPInScene()
    {

            var objectsInScene = GetAllObjectsOnlyInScene();
            foreach (var obj in objectsInScene)
            {
                if (obj.GetComponent<TextMeshProUGUI>() != null && obj.GetComponent<TextMeshProUGUI>().text != string.Empty)
                {
                    Debug.Log(obj.name);
                    NewDictItem item = new NewDictItem()
                    {
                        text = obj.GetComponent<TextMeshProUGUI>().text,
                        UiItem = obj.GetComponent<TextMeshProUGUI>()
                    };
                    translation.listOfItemsToTranslate.Add(item);
                }

            }
            List<GameObject> GetAllObjectsOnlyInScene()
            {
                List<GameObject> objectsInScene = new List<GameObject>();

                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                        objectsInScene.Add(go);
                }

                return objectsInScene;
            
        }

    }
#endif
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ImageChangeCollection = imageChange.ToDictionary();   
        AudioChangeCollection = soundChange.ToDictionary();
        UICollection = translation.ToDictionary();
        if(MissionAmygdalaManager.Instance.usePlLanguage) 
        {
            TranslateItems();
        }
        
    }

    public void TranslateItems()
    {
        foreach (var image in ImageChangeCollection)
        {
            image.Key.GetComponent<UnityEngine.UI.Image>().sprite = image.Value;
        }

        foreach (var audio in AudioChangeCollection)
        {
            audio.Key.clip = audio.Value;
        }
        if(previousValuesCollection.Count == 0)
        {
            foreach (var item in UICollection)
            {
                previousValuesCollection.Add(item.Key, new UiElementsInfo(item.Key.text,item.Key.font));
                previousFontAsset = item.Key.font;
                item.Key.text = item.Value;
                item.Key.font = polishFont;
            }
        }
            
        else
        {
            foreach (var item in previousValuesCollection)
            {
                item.Key.text = item.Value.Text;
                item.Key.font = item.Value.FontAsset;
            }
            previousValuesCollection.Clear();
        }

    }
}

public class UiElementsInfo
{
    public UiElementsInfo(string text, TMP_FontAsset font)
    {
        this.text = text; 
        this.fontAsset = font;
    }
    private string text;
    private TMP_FontAsset fontAsset;

    public string Text { get => text;}
    public TMP_FontAsset FontAsset { get => fontAsset; }


}
[Serializable]
public class NewDict
{
    [SerializeField]
    public List<NewDictItem> listOfItemsToTranslate;

    public Dictionary<TextMeshProUGUI, string> ToDictionary()
    {
        Dictionary<TextMeshProUGUI, string> newDictionary = new Dictionary<TextMeshProUGUI, string>();

        foreach (var item in listOfItemsToTranslate)
        {
            newDictionary.Add(item.UiItem, item.text);
        }

        return newDictionary;
    }
}

[Serializable]
public class NewDictItem
{
    [SerializeField]
    public TextMeshProUGUI UiItem;
    [SerializeField]
    public string text;
}


[Serializable]
public class SoundChange
{
    [SerializeField]
    private SoundChangeItem[] listOfTranscriptItems;


    public Dictionary<AudioSource, AudioClip> ToDictionary()
    {
        Dictionary<AudioSource, AudioClip> newDictionary = new Dictionary<AudioSource, AudioClip>();

        foreach (var item in listOfTranscriptItems)
        {
            newDictionary.Add(item.source, item.audio);
        }

        return newDictionary;
    }
}

[Serializable]
public class SoundChangeItem
{
    [SerializeField]
    public AudioSource source;
    [SerializeField]
    public AudioClip audio;
}

[Serializable]
public class ImageChange
{
    [SerializeField]
    private ImageChangeItem[] listOfTranscriptItems;


    public Dictionary<GameObject, UnityEngine.Sprite> ToDictionary()
    {
        Dictionary<GameObject, UnityEngine.Sprite> newDictionary = new Dictionary<GameObject, UnityEngine.Sprite>();

        foreach (var item in listOfTranscriptItems)
        {
            newDictionary.Add(item.ImageHolder, item.ReplacementImage);
        }

        return newDictionary;
    }
}

[Serializable]
public class ImageChangeItem
{
    [SerializeField]
    public GameObject ImageHolder;
    [SerializeField]
    public UnityEngine.Sprite ReplacementImage;
}

