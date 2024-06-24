using DialogueGraph.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TranscriptControllerBase : MonoBehaviour
{

    [Header("Dialogue")]
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI viewport;
    public GameObject dialogueWindow;
    [Space]
    public NPCDialogue npcDialogue;
    public TranscriptObject DialogueScriptable;
    public GameObject ButtonHolder;
    public List<GameObject> dialogueGraphHolders;


    [Header("Medal")]
    public GameObject medalObject;
    public GameObject medalDoors;


    [Header("Audio")]
    public AudioClip endingMusic;
    public AudioClip flowersAndCandlesMusic;
    public AudioClip backtrackMusic;


    [Header("Misc")]
    protected string mood;
    protected bool isLastDialogue = false;

    public bool usesSprites = false;
    public List<Texture> choiceSprites;



#if UNITY_EDITOR
    [CustomEditor(typeof(TranscriptControllerBase), true)]
    class MyClassEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TranscriptControllerBase self = (TranscriptControllerBase)target;
            serializedObject.Update();
            if (self.usesSprites)
                DrawDefaultInspector();
            else
            {
                DrawPropertiesExcluding(serializedObject, "choiceSprites");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    public async void FinishDay()
    {
        await Task.Delay(3000);
        ChangeLvl("GalaxyMenu");
    }

    public void ChangeLvl(string lvlName)
    {
        MissionAmygdalaManager.Instance.ChangeLvl(lvlName);
    }

    public bool SpecialPostMoodDialogue()
    {
        if (mood.ToLower() == "happy" || mood.ToLower() == "okay")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlayWhimsical()
    {
        SoundManager.instance.StopAllBackground();
        SoundManager.instance.PlayBackgroundSound(endingMusic, true);
    }

    protected void Refresh(GameObject obj)
    {
        obj.SetActive(false);
        obj.SetActive(true);
    }

    protected void GetDialoguePart(int number)
    {
        npcDialogue.DialogueSystem = dialogueGraphHolders[number].GetComponent<RuntimeDialogueGraph>();
    }
    protected void OnDialogueStarted()
    {
        
        foreach (var item in DialogueScriptable.DialogueListObjects)
        {
            string holder1 = item.trascriptText;
            string holder2 = npcDialogue.textToShow;
            string normalized1 = Regex.Replace(holder1, @"\s", "");
            string normalized2 = Regex.Replace(holder2, @"\s", "");

            if (string.Equals(normalized1, normalized2, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Matching item found");
                if (item.clip != null)
                {
                    videoPlayer.isLooping = false;
                    videoPlayer.clip = null;
                    videoPlayer.clip = item.clip;
                    videoPlayer.Play();
                }
                else
                    videoPlayer.isLooping = true;
                if (item.audio != null)
                {
                    SoundManager.instance.StopAllAudios();
                    SoundManager.instance.PlayAudio(item.audio);
                }

                break;
            }
        }
    }

    protected void OnTextFinished()
    {
        if (isLastDialogue)
            FinishDay();
        else
            npcDialogue.NextDialogue();
    }
    protected void OnDoorOpenned()
    {
            if (npcDialogue != null)
            {
                npcDialogue.StartDialogue();
            }

            else
                Debug.Log("NPCDialogue is Null");
    }


    public async void PresentMedal()
    {
        ButtonHolder.SetActive(false);

        medalDoors.SetActive(true);
        medalObject.SetActive(true);
        await Task.Delay(animManager.length*1000);
        SoundManager.instance.StopAllBackground();
        SoundManager.instance.PlayBackgroundSound(endingMusic);
        medalDoors.SetActive(false);

        ButtonHolder.SetActive(true);
    }

    public void ToggleVideoPause()
    {
        if(videoPlayer.isPaused == false)
        {
            videoPlayer.playbackSpeed = 0;
        }
        else
        {
            videoPlayer.playbackSpeed = 1;
        }
    }

    public void SetFramedPicture(int number)
    {
        if (!usesSprites || choiceSprites.Count == 0)
            return;
        PlayWhimsical();
        videoPlayer.transform.parent.GetComponentInChildren<RawImage>().texture = choiceSprites[number];
        videoPlayer.transform.parent.GetComponentInChildren<RawImage>().transform.GetChild(0).gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        isLastDialogue = true;
    }


}
