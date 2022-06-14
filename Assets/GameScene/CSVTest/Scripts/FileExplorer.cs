using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SmartDLL;
using System.IO;

public class FileExplorer : MonoBehaviour
{
    public Button openExplorerButton;
    private bool setOnce;
    public SmartFileExplorer fileExplorer = new SmartFileExplorer();

    private void OnEnable()
    {
        openExplorerButton.onClick.AddListener(delegate { ShowExplorer(); });
    }

    // Start is called before the first frame update
    void Start()
    {
        setOnce = true;
        //openExplorerButton = GameObject.Find("FileExplorer").GetComponent<Button>();
    }

    void Update()
    {
        if (fileExplorer.resultOK)
        {
            ManageCSV.csvFile = File.ReadAllText(fileExplorer.fileName);
            ManageCSV.grid = ManageCSV.getCSVGrid(ManageCSV.csvFile);
            Scroll.imported = true;
            /*if (setOnce)
            {
                setOnce = false;
                Scroll.buttonSetup();
            }*/
        }
    }

    void ShowExplorer()
    {
        string initialDir = @"C:/";
        bool restoreDir = true;
        string title = "Open a CSV File";
        string defExt = "csv";
        string filter = "csv files (*.csv)|*.csv";

        fileExplorer.OpenExplorer(initialDir, restoreDir, title, defExt, filter);
    }
}
