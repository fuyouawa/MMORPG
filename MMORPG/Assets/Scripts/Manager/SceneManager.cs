using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SceneManager : MonoSingleton<SceneManager>
{
    private Canvas _mainCanvas;

    public Canvas MainCanvas => _mainCanvas;

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    protected override void Awake()
    {
        base.Awake();
        _mainCanvas = FindFirstObjectByType<Canvas>();
        if (_mainCanvas == null)
        {
            Global.Logger.Warn("当前场景还未创建Canvas组件!");
        }
        if (MessageBoxManager.Instance == null)
        {
            var obj = Resources.Load<GameObject>("Prefeb/UI/MessageBox Manager");
            var inst = Instantiate(obj, MainCanvas.transform);
            inst.name = "MessageBox Manager(Create by SceneManager)";
        }
        if (SpinnerBoxManager.Instance == null)
        {
            var obj = Resources.Load<GameObject>("Prefeb/UI/SpinnerBox Manager");
            var inst = Instantiate(obj, MainCanvas.transform);
            inst.name = "SpinnerBox Manager(Create by SceneManager)";
        }
    }

    private void Start()
    {
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