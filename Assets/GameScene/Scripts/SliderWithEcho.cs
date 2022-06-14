using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithEcho : MonoBehaviour {

    public Slider TheSlider = null;
    public Text TheEcho = null;
    public Text TheLabel = null;
    public CameraManipulation camera = null;
    private bool zoom = false;
    public SoundManagerScript notification;

    public delegate void SliderCallbackDelegate(float v);      // defined a new data type
    private SliderCallbackDelegate mCallBack = null;           // private instance of the data type


	// Use this for initialization
	void Start () {
        Debug.Assert(TheSlider != null);
        Debug.Assert(TheEcho != null);
        Debug.Assert(TheLabel != null);

        TheSlider.onValueChanged.AddListener(SliderValueChange);
        zoom = TheSlider.wholeNumbers;
        notification = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
    }

    public void SetSliderListener(SliderCallbackDelegate listener)
    {
        mCallBack = listener;
    }
	
    // GUI element changes the object
	void SliderValueChange(float v)
    {
        if(zoom){
            TheEcho.text = v.ToString("0");
            camera.zoomspeed = (int)v;
        } else {
            TheEcho.text = v.ToString("0.000");
            if(gameObject.name=="MovementSpeed"){
                camera.movespeed = v;
            } else if(gameObject.name =="EventNoti"){
                notification.tileVolume = v;
            } else if (gameObject.name=="QuestionNoti"){
                notification.questionVolume = v;
            }
        }
        // Debug.Log("SliderValueChange: " + v);
        if (mCallBack != null)
            mCallBack(v);
    }

    public float GetSliderValue() { return TheSlider.value; }
    public void SetSliderLabel(string l) { TheLabel.text = l; }
    public void SetSliderValue(float v) { TheSlider.value = v; SliderValueChange(v); }
    public void InitSliderRange(float min, float max, float v)
    {
        TheSlider.minValue = min;
        TheSlider.maxValue = max;
        SetSliderValue(v);
    }

}