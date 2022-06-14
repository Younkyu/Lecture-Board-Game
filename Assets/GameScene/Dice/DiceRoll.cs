using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class DiceRoll : MonoBehaviour
{
    private static Rigidbody rb;
    public static int DiceNumber;
    public static GameObject DiceView;
    public static bool canRoll;
    public static GameObject DiceDetector;
    private PlayerMovement pm;
    public static int movePoints;
    private BoardGameManager bgm;
    private PlayerGrouping pGroup;
    public static int starCount;
    private PlayerData playerData;
    public InputField inputField;

    private Vector3 originalPos;

    // Start is called before the first frame update
    void Start()
    {
        DiceNumber = 0;
        canRoll = true;
        rb = GetComponent<Rigidbody>();
        DiceView = GameObject.Find("DiceView");
        DiceView.SetActive(false);
        DiceDetector = GameObject.Find("DiceDetector");
        DiceDetector.SetActive(false);
        Invoke("findPlayer", 1);
        movePoints = 0;
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void findPlayer()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            int playerNumber = 0;
            for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
            {
                if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
                {
                    playerNumber = i;
                }
            }
            pm = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player" + playerNumber + "Piece(Clone)").GetComponent<PlayerMovement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canRoll && movePoints > 0 && !inputField.isFocused)
        {
            canRoll = false;
            movePoints--;
            playerData.sendData();
            Invoke("tempMethodEnableRoll", 5);
            DiceView.SetActive(true);
            DiceDetector.SetActive(true);
            Invoke("DiceOff", 5);
            Roll();
        }
    }

    private void tempMethodEnableRoll()
    {
        canRoll = true;
    }

    private void DiceOff()
    {
        DiceView.SetActive(false);
        pm.diceMove();
    }

    void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector3.zero;
            float dirX = Random.Range(0, 5000);
            float dirY = Random.Range(0, 5000);
            float dirZ = Random.Range(0, 5000);
            float upForce = Random.Range(200, 500);
            transform.position = originalPos;
            transform.rotation = Quaternion.identity;
            rb.AddForce(transform.up * upForce);
            rb.AddTorque(dirX, dirY, dirZ);
        }
    }
}
