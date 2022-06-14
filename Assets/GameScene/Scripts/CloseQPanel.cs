using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseQPanel : MonoBehaviour
{
    public GameObject questionPanel;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(closePanel);
        questionPanel = GameObject.Find("Canvas").transform.Find("Question").gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void closePanel(){
        if (questionPanel != null) {  
            questionPanel.SetActive(false);             
        }
    }
}
