using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticDoorController : MonoBehaviour
{

    public delegate void DoorClosed();
    public DoorClosed OnDoorClosedDelegate;

    public delegate void DoorOpenned();
    public DoorClosed OnDoorOpennedDelegate;

    private Animator animator;

    public bool isDoorOpenned = true;
    public int animationLenght;
    public static StaticDoorController instance;
    public void Awake()
    {
        DontDestroyOnLoad(this.transform.parent);
        if (instance == null)
        {
            instance = this;

        }
        else if (instance != this)
        {
            Destroy(gameObject);

            return;

        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animationLenght = animator.GetCurrentAnimatorClipInfo(0).Length ;
        SceneManager.sceneLoaded += OpenDoorOnSceneChanged;
    }
    public void OpenDoorOnSceneChanged(Scene scene, LoadSceneMode loadSceneMode)
    {
        OpenDoor();
    }
    public void OpenDoor()
    {
        if(!isDoorOpenned)
        animator.enabled = true;
    }
    public void EnableDoorAnimation()
    {
        animator.enabled = true;
    }

    public void DisableDoorAnimation()
    {
        animator.enabled = false;
        
    }

    public async void OnDoorOppened()
    {
        isDoorOpenned = true;
        OnDoorOpennedDelegate?.Invoke();
        await Task.Delay(100);
        MissionAmygdalaManager.Instance.RefreshDoorInstance();
    }

    public void OnDoorClosed()
    {
        isDoorOpenned = false;
        //SoundManager.instance.StopAll();
        OnDoorClosedDelegate?.Invoke();
    }
}