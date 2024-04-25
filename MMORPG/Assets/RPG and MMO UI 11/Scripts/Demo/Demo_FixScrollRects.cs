using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Demo_FixScrollRects : MonoBehaviour {

    protected void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoaded;
    }

    protected void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoaded;
    }

    private void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        ScrollRect[] rects = Component.FindObjectsOfType<ScrollRect>();

        foreach (ScrollRect rect in rects)
        {
            LayoutRebuilder.MarkLayoutForRebuild(rect.viewport);
        }
    }
}
