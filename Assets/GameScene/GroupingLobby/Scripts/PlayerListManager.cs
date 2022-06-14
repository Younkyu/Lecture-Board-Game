using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListManager : MonoBehaviour
{
    private GameObject playerListText;
    public GameObject groupNameText;
    public GameObject playerNameBox;
    public GameObject listContentBox;
    public int playerCount = 0;
    public int groupCount = 0;
    private int groupLimit = 4;
    public GameObject groupCountTestText;
    // Start is called before the first frame update
    void Start()
    {
        playerListText = GameObject.Find("PlayerListText");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addPlayer()
    {
        if (playerCount == 0)
        {
            groupNameText.SetActive(true);
            ++groupCount;
            playerNameBox.SetActive(true);
            playerNameBox.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Player " + (++playerCount);
        } else
        {
            if (playerCount%4 == 0)
            {
                GameObject newGroup = GameObject.Instantiate(groupNameText);
                newGroup.GetComponent<UnityEngine.UI.Text>().text = "Group " + (++groupCount);
                newGroup.transform.parent = listContentBox.transform;
            }
            GameObject newPlayer = GameObject.Instantiate(playerNameBox);
            newPlayer.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Player " + (++playerCount);
            newPlayer.transform.parent = listContentBox.transform;
        }
        playerListText.GetComponent<UnityEngine.UI.Text>().text = "Player List (" + playerCount + ")";
    }
    public void checkGroups()
    {
        int playersInGroup = 0;
        for (int i = 0; i < groupCount + playerCount; i++)
        {
            GameObject item = listContentBox.transform.GetChild(i).gameObject;
            if (item.name[0] == 'g')
            {
                if (playersInGroup > 4)
                {
                    groupCountTestText.GetComponent<UnityEngine.UI.Text>().text = "More than 4 players in one or more group";
                    return;
                } else
                {
                    playersInGroup = 0;
                    groupCountTestText.GetComponent<UnityEngine.UI.Text>().text = "All groups have 4 or less players";
                }
            } else
            {
                playersInGroup++;
            }
        }
    }
}
