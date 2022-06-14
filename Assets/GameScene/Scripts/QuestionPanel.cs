using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : MonoBehaviour
{
    private string answer;
    public InputField userInput;
    public Button submitButton;
    public GameObject selfgrade;
    public Button closeButton;
    private bool chanceQ = false;
    private Color32 normal = new Color32(61, 197, 212, 239);
    private Color32 special = new Color32(199, 101, 255, 239);

    // Start is called before the first frame update
    void Start()
    {
        Button btn = submitButton.GetComponent<Button>();
        btn.onClick.AddListener(checkAnswer);
        Button btn2 = closeButton.GetComponent<Button>();
        btn2.onClick.AddListener(close);
        if (selfgrade == null)
            selfgrade = GameObject.Find("Canvas").transform.Find("Selfgrade").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getChance(){
        return chanceQ;
    }

    public void chanceQuestion(string a){
        answer = a;
        chanceQ = true;
        GetComponent<Image>().color = special;
        selfgrade.GetComponent<Image>().color = special;
    }

    public void setAnswer(string a){
        answer = a;
    }

    void checkAnswer(){
        if (selfgrade != null) {
            if(chanceQ){
                chanceQ = false;
                GetComponent<Image>().color = normal;
            }
            
            bool isActive = selfgrade.activeSelf;
            selfgrade.SetActive(!isActive);
            selfgrade.GetComponent<Selfgrader>().studentSubmit(userInput.text);
            userInput.text = "";
            selfgrade.GetComponent<Selfgrader>().activeButtons();
            close();
        }
    }

    void close(){
        if(!closeButton.gameObject.activeSelf){
            closeButton.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
