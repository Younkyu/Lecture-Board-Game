using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class QuestionInfo : MonoBehaviour
{
    public static int[,] grid;
    //For each button made, fill in 0,0 (correct, answered)
    //Need method to take in row int to correctly update the right question info
    //Meaning that student buttons need to store which quesiton number it is
    //Unsure if teacher will have buttons or just text
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //[column, row]
    //column 0 = correct
    //column 1 = total answered
    public void updateInfo(int row, bool correct){
        if (correct){
            grid[0,row] = grid[0,row]+1;
        }
        //Always update total answered
        grid[1,row] = grid[1,row]+1;
    }
}
