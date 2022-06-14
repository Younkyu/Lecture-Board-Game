using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class delayStart : MonoBehaviour
{

    public GameObject startButton;
    private bool isHost = false;
    // Start is called before the first frame update
    void Start()
    {
        isHost = GameLiftManager.GetInstance().m_PeerId == 1;
        if(isHost){
            startButton.SetActive(false);
            StartCoroutine(delayActive());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator delayActive()
    {
        //yield on a new YieldInstruction that waits for 3 seconds.
        yield return new WaitForSeconds(3);

        startButton.SetActive(true);
    }
}
