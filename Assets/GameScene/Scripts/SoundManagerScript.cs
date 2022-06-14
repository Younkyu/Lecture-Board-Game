using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public AudioClip notiSound;
    public AudioClip winStars;
    public AudioClip loseStars;
    public AudioClip tileSound;
    public AudioSource audioSrc;
    public float questionVolume = 1f;
    public float tileVolume = 1f;
    public bool questionToggle = true;
    public bool eventToggle = true;
    // Start is called before the first frame update
    void Start()
    {
        //notiSound = Resource.Load<AudioClip> ("notiSound");
        audioSrc = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void changeQuestion(){
        questionToggle = !questionToggle;
    }

    public void changeEvent(){
        eventToggle = !eventToggle;
    }

    //When new question is posted
    public void playSound(){
        if(questionToggle){
            audioSrc.PlayOneShot(notiSound, questionVolume);
        }
    }

    public void tileNotification(){
        if(eventToggle){
            audioSrc.PlayOneShot(tileSound, tileVolume);
        }
    }

    public void getStars(){
        if(eventToggle){
            audioSrc.PlayOneShot(winStars, tileVolume);
        }
    }

    public void dropStars(){
        if(eventToggle){
            audioSrc.PlayOneShot(loseStars, tileVolume);
        }
    }
}
