using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class TileNode : MonoBehaviour
{
    public TileNode next;
    public TileNode split;
    public GameObject rentedMark;
    //Player moves vertical (up and down): 1
    //Player moves horizontal (left and right): 2
    public int animation;
    public List<int> players = new List<int>(); //playerNumber of players on tile

    void Start()
    {
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(floatFunction);
    }

    public void AddPlayer(int playerNum)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[4];
            sendValue[0] = 1;
            sendValue[1] = playerNum;
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }

    public void RemovePlayer(int playerNum)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[4];
            sendValue[0] = 2;
            sendValue[1] = playerNum;
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }

    public void floatFunction(string _id, float[] _f)
    {
        switch ((int)_f[0])
        {
            case 1: //add to players
                players.Add((int)_f[1]);
                break;
            case 2: //remove from players
                players.Remove((int)_f[1]);
                break;
            default:
                break;
        }
    }
}
