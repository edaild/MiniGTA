using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Looby : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject MemberShipPanel;
    public Button GameStartButton;
    public Button LoginButton;
    public Button MembershipButton;

    public bool isLoginPanel;
    public bool isMemberShipPanel;


    private void Start()
    {
        GameStartButton.onClick.AddListener(OpenLoginPanel);
        LoginButton.onClick.AddListener(OpenLoginPanel);
        MembershipButton.onClick.AddListener(OpenMembershipPanel);
    }

    private void Update()
    {
        if(isLoginPanel)
            LoginPanel.gameObject.SetActive(true);
        else
            LoginPanel.gameObject.SetActive(false);

        if(isMemberShipPanel)
            MemberShipPanel.gameObject.SetActive(true);
        else
            MemberShipPanel.gameObject.SetActive(false);
    }

    void OpenLoginPanel()
    {
        if (!isLoginPanel)
        {
           isLoginPanel = true;
            isMemberShipPanel = false;
        }
        else
            isLoginPanel = false; 
    }

    void OpenMembershipPanel()
    {
        if (!isMemberShipPanel)
        {
            isMemberShipPanel = true;
            isLoginPanel = false;
        }
        else
            isMemberShipPanel = false;      
    }

}
