//Used for help debug GameLift packet issues and other misc. GameLift potential problems.
#define ASL_DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Command;
using Aws.GameLift.Realtime.Types;
using ASL;

public class BoardGameManager : MonoBehaviour
{
    // The singleton instance for this class
    private static BoardGameManager m_Instance;

    private GameObject groupLobby;
    private GameObject groupLobbyCanvas;
    private GameObject studentUI;
    private GameObject teacherUI;
    private GameObject endGameUI;
    private PlayerGrouping playerGrouping;
    private GameObject camLight;
    private GameObject playerDataManager;
    private GameReport gameReport;
    public StarRankingPanel starRanking;
    [SerializeField] private int currId;
    [SerializeField] private int m_groupWorldSpacing = 75;
    public bool m_SendFloat = false;
    public static int hostID;


    // Start is called before the first frame update
    void Start()
    {
        m_Instance = this;
        groupLobby = GameObject.Find("GroupingLobby");
        groupLobbyCanvas = groupLobby.transform.Find("Canvas").gameObject;
        studentUI = GameObject.Find("StudentUI");
        teacherUI = GameObject.Find("TeacherUI");
        endGameUI = GameObject.Find("EndGameUI");
        playerGrouping = this.gameObject.GetComponent<PlayerGrouping>();
        camLight = GameObject.Find("camLight");
        playerDataManager = GameObject.Find("PlayerDataManager");
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        playerDataManager.SetActive(false);
        studentUI.SetActive(false);
        teacherUI.SetActive(false);
        endGameUI.SetActive(false);
        currId = GameLiftManager.GetInstance().m_PeerId;
        studentUI.GetComponent<ASLObject>()._LocallySetFloatCallback(studentFloatFunction);
        hostID = GameLiftManager.GetInstance().GetLowestPeerId();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_SendFloat && (studentUI.transform.childCount == playerGrouping.m_playerGroups.Count))
        {
            ASLObject thisASL = studentUI.GetComponent<ASLObject>();
            float[] m_MyFloats = new float[4];
            m_MyFloats[0] = 1;
            m_MyFloats[1] = 1;
            thisASL.SendAndSetClaim(() =>
            {
                string floats = "BoardGameManager Floats sent: ";
                for (int i = 0; i < m_MyFloats.Length; i++)
                {
                    floats += m_MyFloats[i].ToString();
                    if (m_MyFloats.Length - 1 != i)
                    {
                        floats += ", ";
                    }
                }
                Debug.Log(floats);
                thisASL.SendFloatArray(m_MyFloats);
            });
            m_SendFloat = false;
        }
    }
    //Called by host
    public void endGame()
    {
        Debug.Log("host endGame");
        ASLObject thisASL = GetComponent<ASL.ASLObject>();
        float[] m_MyFloats = new float[4];
        m_MyFloats[0] = 3;
        //Claim the object
        thisASL.SendAndSetClaim(() =>
        {
            Debug.Log("BoardGameManager Floats sent: 3 0 0 0");
            thisASL.SendFloatArray(m_MyFloats);
        });
    }
    //Called by host
    public void startGame()
    {
        Debug.Log("host startGame");
        playerGrouping.updatePlayerGroups();
        studentUI.SetActive(true);
        setupGroupWorlds();
        //Claim the object
        groupLobby.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
        {
            //Send and then set (once received - NOT here) the tag
            groupLobby.GetComponent<ASL.ASLObject>().SendAndSetTag("Finish");
        });
        starRanking.teacherUISetUp();
        playerDataManager.SetActive(true);
        studentUI.SetActive(false);
        groupLobbyCanvas.SetActive(false);
        teacherUI.SetActive(true);
        Scroll.startGame = true;
    }
    public int getPlayerGroup()
    {
        //for (int i = 0; i < playerGrouping.playersGrid.transform.childCount; i++)
        //{
        //    GameObject player = playerGrouping.playersGrid.transform.GetChild(i).gameObject;
        //    if (Int16.Parse(player.name) == GameLiftManager.GetInstance().m_PeerId)
        //        return i / playerGrouping.groupLimit + 1;
        //}
        //return -1;
        return playerGrouping.getPlayerGroup(GameLiftManager.GetInstance().m_PeerId);
    }
    public int getPlayerGroup(int id)
    {
        //for (int i = 0; i < playerGrouping.playersGrid.transform.childCount; i++)
        //{
        //    GameObject player = playerGrouping.playersGrid.transform.GetChild(i).gameObject;
        //    if (Int16.Parse(player.name) == GameLiftManager.GetInstance().m_PeerId)
        //        return i / playerGrouping.groupLimit + 1;
        //}
        //return -1;
        return playerGrouping.getPlayerGroup(id);
    }

    public void playerStartGame()
    {
        Debug.Log("playerStartGame");
        playerDataManager.SetActive(true);
        groupLobbyCanvas.SetActive(false);
        int id = GameLiftManager.GetInstance().m_PeerId;
        if (id != 1)
        {
            studentUI.SetActive(true);
            int groupNum = getPlayerGroup();
            for (int i = 0; i < studentUI.transform.childCount; i++)
            {
                if (i == groupNum - 1)
                {
                    studentUI.transform.GetChild(i).gameObject.SetActive(true);
                    studentUI.transform.GetChild(i).Find("CamLight").gameObject.SetActive(true);
                    camLight.SetActive(false);
                } else
                {
                    studentUI.transform.GetChild(i).gameObject.SetActive(false);
                }
                
            }
        }
    }


    private void setupGroupWorlds()
    {
        Debug.Log("setupGroupWorlds");
        int numOfGroups = (int)Mathf.Ceil((playerGrouping.playersGrid.transform.childCount) / (float)playerGrouping.groupLimit);
        for (int i = 1; i <= numOfGroups; i++)
        {
            createGroupWorld(i);
        }
        m_SendFloat = true;
    }

    public GameObject getGroupWorld(int groupNum)
    {
        if (groupNum < 1 || groupNum > studentUI.transform.childCount)
            return null;
        return studentUI.transform.GetChild(groupNum - 1).gameObject;
    }

    public GameObject getPlayerPanel(int groupNum)
    {
        GameObject groupWorld = getGroupWorld(groupNum);
        if (groupWorld == null)
            return null;
        return groupWorld.transform.Find("Canvas").Find("PlayerPanels").gameObject;
    }

    private void createGroupWorld(int groupNum)
    {
        Debug.Log("createGroupWorld");
        ASLHelper.InstantiateASLObject("GroupWorld", new Vector3(studentUI.transform.position.x + m_groupWorldSpacing * (groupNum - 1), studentUI.transform.position.y,
            studentUI.transform.position.z), Quaternion.identity, studentUI.GetComponent<ASLObject>().m_Id);
    }

    private void setupPlayerPanel(int groupNum, int playerNum, GameObject player)
    {
        GameObject playerPanels = getPlayerPanel(groupNum);
        Debug.Assert(playerPanels != null);
        if (player.name[0] == 'b')
        {
            playerPanels.transform.GetChild(playerNum).gameObject.SetActive(false);
        }
        else
        {
            playerPanels.transform.GetChild(playerNum).gameObject.GetComponent<PlayerPanel>().setPlayer(Int16.Parse(player.name));
        }
    }

    public static void studentFloatFunction(string _id, float[] _myFloats)
    {
        switch(_myFloats[0])
        {
            case 1:
                Debug.Log("BoardGameManager studentFloatFunction case 1");
                BoardGameManager bgManager = BoardGameManager.GetInstance();
                int index1 = 0;
                foreach (List<int> subList in bgManager.playerGrouping.m_playerGroups)
                {
                    GameObject gameWorld = bgManager.studentUI.transform.GetChild(index1++).gameObject;
                    GameObject playerPanels = gameWorld.transform.Find("Canvas").Find("PlayerPanels").gameObject;
                    int index2 = 0;
                    foreach (int playerID in subList)
                    {
                        if (index2 == 4)
                        {
                            Debug.Log("BoardGameManager studentFloatFunction index 4; playerID: " + playerID);
                            break;
                        }
                        if (playerID == 0)
                        {
                            playerPanels.transform.GetChild(index2++).gameObject.SetActive(false);
                        }
                        else
                        {
                            playerPanels.transform.GetChild(index2++).gameObject.GetComponent<PlayerPanel>().setPlayer(playerID);
                        }
                    }
                    while (index2 < 4)
                    {
                        playerPanels.transform.GetChild(index2++).gameObject.SetActive(false);
                    }
                }
                break;
            default:
                Debug.Log("BoardGameManager studentFloatFunction default case");
                break;
        }
    }

    public static BoardGameManager GetInstance()
    {
        if (m_Instance != null)
        {
            return m_Instance;
        }
        else
        {
            Debug.LogError("BoardGameManager not initialized.");
        }
        return null;
    }
    public void endGameUIHelper()
    {
        endGameUI.GetComponent<EndGameUI>().endGameSetUp();
        //studentUI.SetActive(false);
        if (GameLiftManager.GetInstance().m_PeerId != hostID)
        {
            GameObject gw = getGroupWorld(getPlayerGroup());
            foreach (Transform child in gw.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        teacherUI.SetActive(false);
        endGameUI.SetActive(true);
        camLight.SetActive(true);
    }
}
