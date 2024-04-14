using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

/// <summary>
/// 此组件为一个场景的基础组件
/// 1. 用于支持MessageBox, SpinnerBox, NotificationBox
/// 2. 管理一些资源, 比如Canvas
/// 3. 提供Invoke函数, 使函数可以在Unity主线程中运行
/// </summary>
public class SceneHelperController : MonoSingleton<SceneHelperController>
{
    [SerializeField]
    private Canvas _mainCanvas;
    /// <summary>
    /// 当前场景中的主要Canvas
    /// </summary>
    public Canvas MainCanvas => _mainCanvas;

    public MessageBoxManager MessageBoxManager;
    public NotificationBoxManager NotificationBoxManager;
    public SpinnerBoxManager SpinnerBoxManager;
    public BlackFieldManager BlackFieldManager;

    private readonly Queue<Action> _executionQueue = new Queue<Action>();

#if UNITY_EDITOR
    [MenuItem("Tools/MMORPG/初始化场景")]
    public static void InitializeScene()
    {
        var canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("错误", "您必须先创建一个Canvas!", "确定");
            return;
        }
        if (FindFirstObjectByType<SceneHelperController>() == null)
        {
            var controller = Instantiate(Resources.Load<SceneHelperController>("Prefabs/UI/Tool/SceneHelper Controller"), canvas.transform);
            controller.name = "SceneHelper Controller";
            controller._mainCanvas = canvas;
        }
        EditorUtility.DisplayDialog("提示", "初始化成功!\n注意: 初始化后您必须先手动保存一下当前场景, 否则初始化创建的GameObject可能会丢失", "确定");
    }
#endif

    private void Start()
    {
        if (_mainCanvas == null)
        {
            _mainCanvas = FindFirstObjectByType<Canvas>();
            Debug.Assert(_mainCanvas != null);
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

    public void Invoke(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() =>
            {
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
}

public static class SceneHelper
{
    /// <summary>
    /// 使得函数能够在Unity主线程中被调用
    /// </summary>
    public static void Invoke(Action action)
    {
        SceneHelperController.Instance.Invoke(action);
    }

    /// <summary>
    /// 使得函数能够在Unity主线程中被异步调用
    /// </summary>
    public static Task InvokeAsync(Action action)
    {
        return SceneHelperController.Instance.InvokeAsync(action);
    }

    /// <summary>
    /// 在屏幕中显示一个模态的消息框
    /// </summary>
    public static void ShowMessageBox(MessageBoxConfig config)
    {
        Invoke(() =>
        {
            SceneHelperController.Instance.MessageBoxManager.Config = config;
            SceneHelperController.Instance.MessageBoxManager.Show();
        });
    }
    /// <summary>
    /// 在屏幕中显示异步一个模态的消息框
    /// </summary>
    /// <returns>选中的结果</returns>
    public static Task<MessageBoxResult> ShowMessageBoxAsync(MessageBoxConfig config)
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
    public static void CreateNotificationBox(NotificationBoxConfig config)
    {
        Invoke(() =>
        {
            SceneHelperController.Instance.NotificationBoxManager.Config = config;
            SceneHelperController.Instance.NotificationBoxManager.Create();
        });
    }

    /// <summary>
    /// 在屏幕中显示一个模态的旋转加载框
    /// 注意一定要配合EndSpinnerBox结束显示
    /// </summary>
    public static void BeginSpinnerBox(SpinnerBoxConfig config)
    {
        Invoke(() =>
        {
            SceneHelperController.Instance.SpinnerBoxManager.Config = config;
            SceneHelperController.Instance.SpinnerBoxManager.Show();
        });
    }

    public static void EndSpinnerBox()
    {
        Invoke(() =>
        {
            SceneHelperController.Instance.SpinnerBoxManager.Close();
        });
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public static void SwitchScene(string sceneName)
    {
        SceneHelperController.Instance.BlackFieldManager.FadeIn(onComplete: () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }
}