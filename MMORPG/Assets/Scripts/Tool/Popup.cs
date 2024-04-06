using PimDeWitte.UnityMainThreadDispatcher;
using Serivce;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public static Popup Instance; // µ¥ÀýÊµÀý

    public GameObject PopupPanel;
    public TMP_Text PopupText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Close();
    }


    public void Show(string message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            PopupText.text = message;
            PopupPanel.SetActive(true);
        });
    }

    public void Close()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            PopupPanel.SetActive(false);
        });
    }
}
