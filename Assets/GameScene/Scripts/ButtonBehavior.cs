using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
    public GameObject questionPanel;
    //Question on the question panel
    public Text question;
    //Button's question text
    private string buttontxt;
    private string answer;
    //Selfgrade
    public Text question2;
    public Text answerTxt;
    public GameObject sg;
    public bool submitted = false;
    public bool answered = false;

    private GameObject newMark;
    private string studentAnswer;
    //For GameReport's reportData list
    public int questionIndex = -1;
    
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenQ);
        buttontxt = "Question: " + this.GetComponentInChildren<Text>().text;
        //questionPanel = GameObject.Find("Question");
        if (questionPanel == null)
            questionPanel = GameObject.Find("Canvas").transform.Find("Question").gameObject;
        if (question == null)
            question = GameObject.Find("Canvas").transform.Find("Question").Find("Q Scroll View").Find("Viewport").Find("Content").Find("QuestionText").GetComponent<Text>();
        if (question2 == null)
            question2 = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("Q Scroll View").Find("Viewport").Find("Content").Find("QuestionText").GetComponent<Text>();
        if (answerTxt == null)
            answerTxt = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("TA Scroll View").Find("Viewport").Find("Content").Find("TeacherAnswer").GetComponent<Text>();
        sg = GameObject.Find("Canvas").transform.Find("Selfgrade").gameObject;
        newMark = gameObject.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setQA(string q, string a)
    {
        this.GetComponentInChildren<Text>().text = q;
        answer = a;
        buttontxt = "Question: " + q;
    }

    public string getQ(){
        return buttontxt;
    }

    public string getA(){
        return answer;
    }

    public void setStudentAnswer(string s){
        studentAnswer = s;
    }

    void OpenQ(){
        if(!questionPanel.GetComponent<QuestionPanel>().getChance()){
            bool isActive;
            if(newMark.activeSelf){
                newMark.SetActive(false);
            }
            if(answered && sg!=null && sg.GetComponent<Selfgrader>().graded){
                isActive = sg.activeSelf;
                GameObject correct = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("Correct").gameObject;
                GameObject incorrect = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("Incorrect").gameObject;
                if(questionPanel!=null){
                    questionPanel.SetActive(false);
                }
                correct.SetActive(false);
                incorrect.SetActive(false);
                if(sg.activeSelf){
                    string q = question2.text;
                    if (buttontxt == q){
                        correct.SetActive(true);
                        incorrect.SetActive(true);
                        sg.SetActive(false); 
                    } else {
                        sg.GetComponent<Selfgrader>().setText(buttontxt, answer, studentAnswer);
                    }
                } else {
                    sg.GetComponent<Selfgrader>().setText(buttontxt, answer, studentAnswer);
                    sg.SetActive(true);
                }
            } else if (questionPanel != null && (!sg.activeSelf || sg.GetComponent<Selfgrader>().graded)) { 
                if(sg.activeSelf){
                    sg.SetActive(false);
                } 
                questionPanel.GetComponent<QuestionPanel>().setAnswer(answer);
                isActive = questionPanel.activeSelf;
                sg.GetComponent<Selfgrader>().qButton = GetComponent<Button>();
                sg.GetComponent<Selfgrader>().setText(buttontxt, "Teacher's Answer: "+answer);
                sg.GetComponent<Selfgrader>().questionIndex = questionIndex;
                if (isActive){
                    string q = question.text;
                    if (buttontxt == q){
                        questionPanel.SetActive(!isActive); 
                    } else {
                        question.text = buttontxt;
                    }
                } else {
                    questionPanel.SetActive(!isActive);
                    question.text = buttontxt;
                }             
            } 
        }
    }
}
