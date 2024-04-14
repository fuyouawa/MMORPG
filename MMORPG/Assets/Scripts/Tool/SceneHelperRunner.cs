using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 此组件为一个场景的基础组件
/// 1. 用于支持MessageBox, SpinnerBox, NotificationBox
/// 2. 管理一些资源, 比如Canvas
/// 3. 提供Invoke函数, 使函数可以在Unity主线程中运行
/// </summary>
public class SceneHelperRunner : MonoSingleton<SceneHelperRunner>
{
    /// <summary>
    /// 当前场景中的主要Canvas
    /// </summary>
    public Canvas MainCanvas { get; private set; }

    public MessageBoxManager MessageBoxManager { get; private set; }
    public NotificationBoxManager NotificationBoxManager { get; private set; }
    public SpinnerBoxManager SpinnerBoxManager { get; private set; }
    public BlackFieldManager BlackFieldManager { get; private set; }

    private readonly Queue<Action> _executionQueue = new Queue<Action>();

#if UNITY_EDITOR
    [MenuItem("Tools/MMORPG/初始化场景")]
    public static void InitializeScene()
    {
        if (GameObject.FindFirstObjectByType<SceneHelperRunner>() == null)
        {
            new GameObject("SceneHelper Runner").AddComponent<SceneHelperRunner>();
        }
        var canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            //EditorUtility.DisplayDialog("错误", "您必须先创建一个Canvas!", "确定");
            return;
        }
        var group = canvas.transform.Find("Box Manager");
        if (group == null)
        {
            group = new GameObject("Box Manager").AddComponent<RectTransform>();
            group.SetParent(canvas.transform, false);
        }
        var group2 = canvas.transform.Find("Auxiliary");
        if (group2 == null)
        {
            group2 = new GameObject("Auxiliary").AddComponent<RectTransform>();
            group2.SetParent(canvas.transform, false);
        }

        if (GameObject.FindFirstObjectByType<MessageBoxManager>() == null)
        {
            var inst = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tool/MessageBox Manager"), group);
            inst.name = "MessageBox Manager";
        }
        if (GameObject.FindFirstObjectByType<NotificationBoxManager>() == null)
        {
            var inst = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tool/NotificationBox Manager"), group);
            inst.name = "NotificationBox Manager";
        }
        if (GameObject.FindFirstObjectByType<SpinnerBoxManager>() == null)
        {
            var inst = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tool/SpinnerBox Manager"), group);
            inst.name = "SpinnerBox Manager";
        }
        if (GameObject.FindFirstObjectByType<BlackFieldManager>() == null)
        {
            var inst = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tool/BlackField Manager"), group2);
            inst.name = "BlackField Manager";
        }
        //EditorUtility.DisplayDialog("提示", "初始化完成!", "确定");
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        var found = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Assert(found.Length == 1);
        MainCanvas = found[0];

        Debug.Assert(MainCanvas != null);
        MessageBoxManager = GameObject.FindFirstObjectByType<MessageBoxManager>();
        Debug.Assert(MessageBoxManager != null);
        NotificationBoxManager = GameObject.FindFirstObjectByType<NotificationBoxManager>();
        Debug.Assert(NotificationBoxManager != null);
        SpinnerBoxManager = GameObject.FindFirstObjectByType<SpinnerBoxManager>();
        Debug.Assert(SpinnerBoxManager != null);
        BlackFieldManager = GameObject.FindFirstObjectByType<BlackFieldManager>();
        Debug.Assert(BlackFieldManager != null);
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
}

public static class SceneHelper
{
    /// <summary>
    /// 使得函数能够在Unity主线程中被调用
    /// </summary>
    public static void Invoke(Action action)
    {
        SceneHelperRunner.Instance.Invoke(action);
    }

    /// <summary>
    /// 使得函数能够在Unity主线程中被异步调用
    /// </summary>
    public static Task InvokeAsync(Action action)
    {
        return SceneHelperRunner.Instance.InvokeAsync(action);
    }

    /// <summary>
    /// 在屏幕中显示一个模态的消息框
    /// </summary>
    public static void ShowMessageBox(MessageBoxConfig config)
    {
        Invoke(() =>
        {
            SceneHelperRunner.Instance.MessageBoxManager.Config = config;
            SceneHelperRunner.Instance.MessageBoxManager.Show();
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
            SceneHelperRunner.Instance.NotificationBoxManager.Config = config;
            SceneHelperRunner.Instance.NotificationBoxManager.Create();
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
            SceneHelperRunner.Instance.SpinnerBoxManager.Config = config;
            SceneHelperRunner.Instance.SpinnerBoxManager.Show();
        });
    }

    public static void EndSpinnerBox()
    {
        Invoke(() =>
        {
            SceneHelperRunner.Instance.SpinnerBoxManager.Close();
        });
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public static void SwitchScene(string sceneName)
    {
        SceneHelperRunner.Instance.BlackFieldManager.FadeIn(onComplete: () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }
}