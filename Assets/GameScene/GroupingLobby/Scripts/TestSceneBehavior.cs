using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class TestSceneBehavior : MonoBehaviour
{
    public GameObject studentUI;
    public GameObject groupWorld;
    int groupNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(groupWorld != null);
        GameObject playerPanels = groupWorld.transform.Find("Canvas").Find("PlayerPanels").gameObject;
        Debug.Assert(playerPanels != null);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            createGroupWorld(++groupNum);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            getGroupNum();
        }
    }
    private void createGroupWorld(int groupNum)
    {
        Debug.Log("createGroupWorld");
        ASLHelper.InstantiateASLObject("GroupWorld", new Vector3(studentUI.transform.position.x + 75 * (groupNum - 1), studentUI.transform.position.y,
            studentUI.transform.position.z), Quaternion.identity, studentUI.GetComponent<ASLObject>().m_Id);
    }
    //private void setupPlayerPanels(int groupNum)
    //{
    //    GameObject playerPanels = getPlayerPanels(groupNum);
    //    Debug.Assert(playerPanels != null);
    //    if (player.name[0] == 'b')
    //    {
    //        playerPanels.transform.GetChild(playerNum).gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        playerPanels.transform.GetChild(playerNum).gameObject.GetComponent<PlayerPanel>().setPlayer(player.transform.GetChild(0).gameObject.GetComponent<Text>().text, Int16.Parse(player.name));
    //        m_Players.Add(Int16.Parse(player.name), groupNum);
    //    }
    //}

    public GameObject getGroupWorld(int groupNum)
    {
        if (groupNum < 1 || groupNum >= studentUI.transform.childCount)
            return null;
        return studentUI.transform.GetChild(groupNum - 1).gameObject;
    }

    public GameObject getPlayerPanels(int groupNum)
    {
        GameObject groupWorld = getGroupWorld(groupNum);
        if (groupWorld == null)
            return null;
        return groupWorld.transform.Find("PlayerPanels").gameObject;
    }
    private void getGroupNum()
    {
        Debug.Log("groupNum: " + groupNum);
    }
}
