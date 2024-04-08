using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SceneManager : MonoSingleton<SceneManager>
{
    [Header("Box服务")]
    public bool EnableMessageBox = true;
    public bool EnableSpinnerBox = true;
    public bool EnableNotificationBox = true;

    private Canvas _mainCanvas;

    public Canvas MainCanvas => _mainCanvas;

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private RectTransform _managerGroup;

    private MessageBoxManager _messageBoxManager;
    private NotificationBoxManager _notificationBoxManager;
    private SpinnerBoxManager _spinnerBoxManager;

    protected override void Awake()
    {
        base.Awake();
        _mainCanvas = FindFirstObjectByType<Canvas>();
        if (_mainCanvas == null)
        {
            if (EnableMessageBox || EnableNotificationBox || EnableSpinnerBox)
            {
                throw new Exception("要激活Box服务, 当前创建必须存在Canvas组件!");
            }
            else
            {
                Global.Logger.Warn("当前场景还未创建Canvas组件!");
            }
        }
        else
        {
            _managerGroup = new GameObject("Manager Group(Create by SceneManager)").AddComponent<RectTransform>();
            _managerGroup.SetParent(MainCanvas.transform, false);
            InitManagers();
        }
    }

    private void InitManagers()
    {
        _messageBoxManager = Instantiate(Resources.Load<MessageBoxManager>("Prefabs/UI/Tool/MessageBox Manager"), _managerGroup);
        _notificationBoxManager = Instantiate(Resources.Load<NotificationBoxManager>("Prefabs/UI/Tool/NotificationBox Manager"), _managerGroup);
        _spinnerBoxManager = Instantiate(Resources.Load<SpinnerBoxManager>("Prefabs/UI/Tool/SpinnerBox Manager"), _managerGroup);
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

#region Invoke

    public void Invoke(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    public void Invoke(Action action)
    {
        Invoke(ActionWrapper(action));
    }

    public Task InvokeAsync(Action action)
    {
        var tcs = new TaskCompletionSource<bool>();

        void WrappedAction()
        {
            try
            {
                action();
                tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        Invoke(ActionWrapper(WrappedAction));
        return tcs.Task;
    }

    private IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }
    #endregion

#region Box
    public void ShowMessageBox(MessageBoxConfig config)
    {
        Invoke(() =>
        {
            _messageBoxManager.Config = config;
            _messageBoxManager.Show();
        });
    }
    public Task<MessageBoxResult> ShowMessageBoxAsync(MessageBoxConfig config)
    {
        var tcs = new TaskCompletionSource<MessageBoxResult>();
        void OnChose(MessageBoxResult result)
        {
            var suc = tcs.TrySetResult(result);
            Debug.Assert(suc);
        }
        config.OnChose += OnChose;
        ShowMessageBox(config);
        return tcs.Task;
    }

    public void CreateNotificationBox(NotificationBoxConfig config)
    {
        Invoke(() =>
        {
            _notificationBoxManager.Config = config;
            _notificationBoxManager.Create();
        });
    }

    public void BeginSpinnerBox(SpinnerBoxConfig config)
    {
        Invoke(() =>
        {
            _spinnerBoxManager.Config = config;
            _spinnerBoxManager.Show();
        });
    }

    public void EndSpinnerBox()
    {
        Invoke(() =>
        {
            _spinnerBoxManager.Close();
        });
    }
    #endregion
}