using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiPublish : MonoBehaviour
{
    public GameObject content;
    private GameObject button;
    public teacherUI teachUI;
    private string[] set;
    private bool allSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(publish);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void publish(){
        int amount = content.transform.childCount;
        for (int i=0; i<amount; i++){
            button = content.transform.GetChild(i).gameObject;
            if (button.transform.GetChild(2).gameObject.GetComponent<Toggle>().isOn){
                button.GetComponent<TeacherButton>().published = true;
                //Yellow - Color (255,214,0)
                button.gameObject.GetComponent<Image>().color = new Color32(157, 241, 146, 255);
                GameObject DataSend = GameObject.Find("DataSend");
                set = button.GetComponent<TeacherButton>().getQA();
                DataSend.GetComponent<SendNewQuestion>().sendQuestion(set[0], set[1], button.GetComponent<TeacherButton>().questionIndex);
                teachUI.incrementQuestionsPosted();
                button.transform.GetChild(2).gameObject.GetComponent<Toggle>().isOn = false;
                button.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        allSelected = false;
    }

    public void selectAll()
    {
        bool setToggle = true;
        if(allSelected){
            setToggle = false;
        }

        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject toggleObject = content.transform.GetChild(i).GetChild(2).gameObject;
            if (toggleObject.activeSelf)
            {
                toggleObject.GetComponent<Toggle>().isOn = setToggle;
            }
        }
        allSelected=!allSelected;
    }

}
