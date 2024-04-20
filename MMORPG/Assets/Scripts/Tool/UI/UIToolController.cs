using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIToolController : MonoBehaviour, IController
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        this.RegisterEvent<ShowMessageBoxEvent>(OnShowMessageBox).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<ShowNotificationBoxEvent>(OnShowNotificationBox).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<ShowSpinnerBoxEvent>(OnShowSpinnerBox).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<CloseSpinnerBoxEvent>(OnCloseSpinnerBox).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnCloseSpinnerBox(CloseSpinnerBoxEvent e)
    {
    }

    private void OnShowSpinnerBox(ShowSpinnerBoxEvent e)
    {
    }

    private void OnShowNotificationBox(ShowNotificationBoxEvent e)
    {
    }

    private void OnShowMessageBox(ShowMessageBoxEvent e)
    {
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}