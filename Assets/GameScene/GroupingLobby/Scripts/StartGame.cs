using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public static bool started;
    private GameObject groupLobby;
    private GameObject groupLobbyCanvas;
    private GameObject teacherUI;

    private void Start()
    {
        started = false;
        groupLobby = GameObject.Find("GroupingLobby");
        groupLobbyCanvas = groupLobby.transform.Find("Canvas").gameObject;
        // teacherUI = GameObject.Find("TeacherUI");
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(temp);
    }

    private void temp(string _id, float[] _f)
    {
        if (_f[0] == 1)
        {
            groupLobbyCanvas.SetActive(false);
            //SceneManager.LoadScene("studentgame");
            if (GameLiftManager.GetInstance().m_PeerId == 1)
            {
                SceneManager.LoadScene("studentgame");
                // teacherUI.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene("studentgame");
                // teacherUI.SetActive(false);
            }
        }
    }
    public void begin()
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[2];
            sendValue[0] = 1f;
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
