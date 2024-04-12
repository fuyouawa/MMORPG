using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 此组件为一个场景的基础组件
/// 1. 用于支持MessageBox, SpinnerBox, NotificationBox
/// 2. 管理一些资源, 比如Canvas
/// 3. 提供Invoke函数, 使函数可以在Unity主线程中运行
/// </summary>
public class SceneManager : MonoSingleton<SceneManager>
{
    /// <summary>
    /// 选择激活Box服务
    /// </summary>
    public bool EnableMessageBox = true;
    public bool EnableSpinnerBox = true;
    public bool EnableNotificationBox = true;
    public bool EnableBlackField = true;

    [Header("BlackField Config")]
    [Tooltip("当场景开始时播放FadeOut(淡出)动画")]
    public bool FadeOutOnAwake = true;

    private Canvas _mainCanvas;

    /// <summary>
    /// 当前场景中的主要Canvas
    /// </summary>
    public Canvas MainCanvas => _mainCanvas;

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private RectTransform _managerGroup;

    private MessageBoxManager _messageBoxManager;
    private NotificationBoxManager _notificationBoxManager;
    private SpinnerBoxManager _spinnerBoxManager;
    private BlackFieldManager _blackFieldManager;

    [MenuItem("Tools/MMORPG/Initialize Scene")]
    public static void InitializeScene()
    {
        if (FindFirstObjectByType<SceneManager>() == null) {
            var manager = new GameObject("Manager");
            var self = new GameObject("Scene Manager");
            self.transform.SetParent(manager.transform, false);
            self.AddComponent<SceneManager>();
        }
    }


    [MenuItem("Debug/Close current channel")]
    public static void CloseChannel()
    {
        NetClient.Session.Close();
    }


    protected override void Awake()
    {
        base.Awake();
        _mainCanvas = FindFirstObjectByType<Canvas>();
        if (_mainCanvas == null)
        {
            if (EnableMessageBox || EnableNotificationBox || EnableSpinnerBox)
            {
                throw new Exception("要启动Box, 当前场景必须存在一个Canvas!");
            }
            else
            {
                Global.Logger.Warn("当前场景还未创建Canvas组件!");
            }
        }
        else
        {
            var found = _mainCanvas.transform.Find("Manager Group");
            if (found != null)
            {
                _managerGroup = found.GetComponent<RectTransform>();
            }
            else
            {
                _managerGroup = new GameObject("Manager Group").AddComponent<RectTransform>();
                _managerGroup.SetParent(MainCanvas.transform, false);
                InitManagers();
            }
        }
    }

    

    private void InitManagers()
    {
        if (EnableMessageBox)
        {
            _messageBoxManager = Instantiate(Resources.Load<MessageBoxManager>("Prefabs/UI/Tool/MessageBox Manager"), _managerGroup);
        }
        if (EnableNotificationBox)
        {
            _notificationBoxManager = Instantiate(Resources.Load<NotificationBoxManager>("Prefabs/UI/Tool/NotificationBox Manager"), _managerGroup);
        }
        if (EnableSpinnerBox)
        {
            _spinnerBoxManager = Instantiate(Resources.Load<SpinnerBoxManager>("Prefabs/UI/Tool/SpinnerBox Manager"), _managerGroup);
        }
        if (EnableBlackField)
        {
            _blackFieldManager = Instantiate(Resources.Load<BlackFieldManager>("Prefabs/UI/Tool/BlackField Manager"), _managerGroup);
            if (FadeOutOnAwake)
                _blackFieldManager.FadeOut();
            else
                _blackFieldManager.gameObject.SetActive(false);
        }
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
    /// <summary>
    /// 使得函数能够在Unity主线程中被调用
    /// </summary>
    public void Invoke(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    /// <summary>
    /// 使得函数能够在Unity主线程中被调用
    /// </summary>
    public void Invoke(Action action)
    {
        Invoke(ActionWrapper(action));
    }

    /// <summary>
    /// 使得函数能够在Unity主线程中被异步调用
    /// </summary>
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
    /// <summary>
    /// 在屏幕中显示一个模态的消息框
    /// </summary>
    public void ShowMessageBox(MessageBoxConfig config)
    {
        Invoke(() =>
        {
            _messageBoxManager.Config = config;
            _messageBoxManager.Show();
        });
    }
    /// <summary>
    /// 在屏幕中显示异步一个模态的消息框
    /// </summary>
    /// <returns>选中的结果</returns>
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

    /// <summary>
    /// 在屏幕中显示一个通知框
    /// </summary>
    public void CreateNotificationBox(NotificationBoxConfig config)
    {
        Invoke(() =>
        {
            _notificationBoxManager.Config = config;
            _notificationBoxManager.Create();
        });
    }

    /// <summary>
    /// 在屏幕中显示一个模态的旋转加载框
    /// 注意一定要配合EndSpinnerBox结束显示
    /// </summary>
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

    #region Scene
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void SwitchScene(string sceneName)
    {
        void LoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        if (EnableBlackField)
        {
            _blackFieldManager.FadeIn(onComplete: () =>
            {
                LoadScene();
            });
        }
        else
        {
            LoadScene();
        }
    }
    #endregion
}