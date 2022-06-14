using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;


public class MenuBehavior : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button menuButton;

    void Start()
    {
        menuButton.onClick.AddListener(menuOnOff);
        quitButton.onClick.AddListener(goToEndGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuOnOff();
        }
    }
    public void menuOnOff()
    {
        if (menu.activeSelf)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }

    public void goToEndGame()
    {
        BoardGameManager.GetInstance().endGameUIHelper();
    }

}
