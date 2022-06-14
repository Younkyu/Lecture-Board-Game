using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using ASL;
using UnityEngine.UI;

[System.Serializable]
public class PlayerGrouping : MonoBehaviour
{
    private GameObject playerListText;
    public GameObject playersGrid;
    public GameObject groupNumList;
    public Scrollbar listScroll;
    public int playerCount = 0; //not including teacher
    public int groupCount = 0;
    private int blankCount = 0;
    public int groupLimit = 4;
    public string gameScene = "studentgame";
    private GameObject groupLobby;
    private bool updateNames = false;
    /// <summary>Bool that toggles when we send the floats, gets set to false after we send to save bandwidth</summary>
    public bool m_SendFloat = false;
    private ASLObject thisASL;
    public SortedList<int, int> m_players = new SortedList<int, int>(); //K,V = id,groupNum
    public List<List<int>> m_playerGroups = new List<List<int>>();

    public GameObject cancelButton;
    public GameObject addBlankButton;
    public GameObject uploadButton;
    public GameObject startButton;
    public GameObject helpButton;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        thisASL = GetComponent<ASLObject>();
        groupLobby = GameObject.Find("GroupingLobby");
        playerListText = GameObject.Find("PlayerListText");
        playerListText.GetComponent<UnityEngine.UI.Text>().text = "Player List (" + (GameLiftManager.GetInstance().m_Players.Count - 1) + ")";
        listScroll.value = 1;
        thisASL._LocallySetFloatCallback(MyFloatFunction);
        if (GameLiftManager.GetInstance().m_PeerId != GameLiftManager.GetInstance().GetLowestPeerId())
        {
            //cancelButton.SetActive(true);
            addBlankButton.SetActive(false);
            uploadButton.SetActive(false);
            startButton.SetActive(false);
            helpButton.SetActive(false);
            StartCoroutine(startPlayerGame());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            populatePlayers();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateNames && (GameLiftManager.GetInstance().m_Players.Count == playersGrid.transform.childCount + 1))
        {
            float[] m_MyFloats = new float[4];
            m_MyFloats[0] = 1;
            m_MyFloats[1] = 1;
            thisASL.SendAndSetClaim(() =>
            {
                string floats = "PlayerGrouping Floats sent: ";
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
            updateNames = false;
        }
        if (m_SendFloat)
        {
            if (groupCount != groupNumList.transform.childCount)
            {
                float[] m_MyFloats = new float[4];
                m_MyFloats[0] = 0;
                if (groupNumList.transform.childCount - 1 == groupCount)
                {
                    m_MyFloats[1] = 2;
                    m_MyFloats[2] = groupNumList.transform.childCount - 1;
                    thisASL.SendAndSetClaim(() =>
                    {
                        string floats = "PlayerGrouping Floats sent: ";
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
                else
                {
                    groupCount = groupNumList.transform.childCount;
                    m_MyFloats[1] = 1;
                    thisASL.SendAndSetClaim(() =>
                    {
                        string floats = "PlayerGrouping Floats sent: ";
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
        }
    }

    public void populatePlayers()
    {
        var players = GameLiftManager.GetInstance().m_Players;
        foreach (var player in players)
        {
            if (player.Key != 1)
            {
                addPlayerGroups(player.Key, 0);
                Debug.Log("populatePlayers instantiate player " + player.Key);
                if ((playerCount++) % groupLimit == 0)
                {
                    addGroup();
                }
                Debug.Log("add player playersGrid.GetComponent<ASLObject>().m_Id = " + playersGrid.GetComponent<ASLObject>().m_Id);
                ASLHelper.InstantiateASLObject("playerName", new Vector3(0, 0, 0), Quaternion.identity, playersGrid.GetComponent<ASLObject>().m_Id);
            }
        }
        updateNames = true;
    }

    public void addBlank()
    {
        if (((blankCount++) + playerCount) % groupLimit == 0)
        {
            addGroup();
            m_SendFloat = true;
        }
        ASLHelper.InstantiateASLObject("blankName", new Vector3(0, 0, 0), Quaternion.identity, playersGrid.GetComponent<ASLObject>().m_Id);
        Debug.Log("add blank playersGrid.GetComponent<ASLObject>().m_Id = " + playersGrid.GetComponent<ASLObject>().m_Id);
    }

    //private void addPlayer(string name, int id, int index)
    //{
    //    if (index < playersGrid.transform.childCount && index >= 0)
    //    {
    //        if ((playerCount++) % groupLimit == 0)
    //        {
    //            addGroup();
    //        }
    //        //GameObject newPlayer = GameObject.Instantiate(playerNameBox);
    //        //newPlayer.name = "" + id;
    //        //newPlayer.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = name;
    //        //newPlayer.SetActive(true);
    //        //newPlayer.transform.SetParent(playersGrid.transform);
    //        GameObject newPlayer = playersGrid.transform.GetChild(index).gameObject;
    //        newPlayer.name = "" + id;
    //        newPlayer.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = name;
    //    }

    //}

    private void addGroup()
    {
        ASLHelper.InstantiateASLObject("ASL_Text", new Vector3(0, 0, 0), Quaternion.identity, groupNumList.GetComponent<ASLObject>().m_Id);
    }
    //update the number of the added group
    private void updateGroupNames()
    {
        if (groupNumList.transform.childCount > 0)
        {
            for (int i = 0; i < groupNumList.transform.childCount; i++)
            {
                updateGroupName(i);
            }
        }
    }
    private void updateGroupName(int index)
    {
        if (index >= 0 && index < groupNumList.transform.childCount)
        {
            GameObject group = groupNumList.transform.GetChild(index).gameObject;
            group.GetComponent<UnityEngine.UI.Text>().text = "" + (index + 1);
        }
    }

    private IEnumerator startPlayerGame()
    {
        while (groupLobby.tag != "Finish")
        {
            yield return null;
        }
        BoardGameManager.GetInstance().playerStartGame();
    }

    public void cancel()
    {
        if (GameLiftManager.GetInstance().m_PeerId == 1 && !EndGameUI.ended)
        {
            float[] m_MyFloats = new float[2];
            m_MyFloats[0] = 4;
            m_MyFloats[1] = 0;
            thisASL.SendAndSetClaim(() =>
            {
                string floats = "PlayerGrouping Floats sent: ";
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
        }
        else
        {
            //GameLiftManager.GetInstance().DisconnectFromServer();
            //SceneManager.LoadScene("ASL_LobbyScene");
            Application.Quit();
        }
    }

    public void addPlayerGroups(int id, int group)
    {
        float[] m_MyFloats = new float[4];
        m_MyFloats[0] = 0;
        m_MyFloats[1] = 3;
        m_MyFloats[2] = id;
        m_MyFloats[3] = group;
        thisASL.SendAndSetClaim(() =>
        {
            string floats = "PlayerGrouping Floats sent: ";
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
    }

    public void updatePlayerGroups()
    {
        float[] m_MyFloats = new float[4];
        m_MyFloats[0] = 2;
        m_MyFloats[1] = 0;
        thisASL.SendAndSetClaim(() =>
        {
            string floats = "PlayerGrouping Floats sent: ";
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
    }

    public int getPlayerGroup(int id)
    {
        return m_players[id];
    }

    public void MyFloatFunction(string _id, float[] _myFloats)
    {
        switch (_myFloats[0])
        {
            case 1: //setup player names
                foreach (var player in m_players)
                {
                    Debug.Log("Player sList: " + player.Key + " ");
                }
                Debug.Log("PlayerGrouping MyFloatFunction s1 case 1");
                var players = GameLiftManager.GetInstance().m_Players;
                int index = 0;
                foreach (var player in m_players)
                {
                    if (player.Key != 1)
                    {
                        GameObject newPlayer = playersGrid.transform.GetChild(index++).gameObject;
                        newPlayer.name = "" + player.Key;
                        newPlayer.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = players[player.Key];
                    }
                }
                break;
            case 2: //update m_players groups and m_playerGroups
                Debug.Log("PlayerGrouping MyFloatFunction s1 case 2");
                List<int> group = new List<int>();
                for (int i = 0; i < playersGrid.transform.childCount; i++)
                {
                    GameObject player = playersGrid.transform.GetChild(i).gameObject;
                    if (player.name[0] != 'b')
                    {
                        m_players[Int16.Parse(player.name)] = i / groupLimit + 1;
                        group.Add(Int16.Parse(player.name));
                        Debug.Log("id:"+ player.name + "; GroupNum:" + m_players[Int16.Parse(player.name)]);
                    }
                    else
                    {
                        group.Add(0);
                    }
                    if ((group.Count == 4) || ((i == playersGrid.transform.childCount - 1) && (group.Count != 0)))
                    {
                        string debug = "PlayerGrouping MyFloatFunction m_playerGroups[" + m_playerGroups.Count + "] contains: ";
                        foreach (int playerID in group)
                        {
                            debug += playerID + " ";
                        }
                        Debug.Log(debug);
                        m_playerGroups.Add(group);
                        group = new List<int>();
                    }
                }
                break;
            case 3: //end game
                BoardGameManager.GetInstance().endGameUIHelper();
                break;
            case 4: //cancel
                //GameLiftManager.GetInstance().DisconnectFromServer();
                //SceneManager.LoadScene("ASL_LobbyScene");
                Application.Quit();
                break;
            default:
                Debug.Log("PlayerGrouping MyFloatFunction default1 case");
                break;
        }
        switch (_myFloats[1])
        {
            case 1: //update all group names
                Debug.Log("PlayerGrouping MyFloatFunction s2 case 1");
                updateGroupNames();
                groupCount = groupNumList.transform.childCount;
                break;
            case 2: //change index
                Debug.Log("PlayerGrouping MyFloatFunction s2 case 2");
                updateGroupName((int)_myFloats[2]);
                groupCount++;
                break;
            case 3: //add player group list
                Debug.Log("PlayerGrouping MyFloatFunction s2 case 3");
                //addPlayerGroupsHelper((int)_myFloats[2], (int)_myFloats[3]);
                m_players.Add((int)_myFloats[2], (int)_myFloats[3]);
                break;
            default:
                Debug.Log("PlayerGrouping MyFloatFunction default2 case");
                break;
        }
    }
}