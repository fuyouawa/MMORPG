using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 全屏背景图片等比例拉伸自适应
/// </summary>
[ExecuteInEditMode]
public class BGScaler : MonoBehaviour
{
    // 图片原大小(压缩前的)
    //public Vector2 textureOriginSize = new Vector2(1376, 920);

    private RectTransform rt;
    private Image bgImage;
    private Canvas canvas;

    void OnEnable()
    {
        Initialize();
        Scaler();
    }

    // 初始化组件
    void Initialize()
    {
        rt = GetComponent<RectTransform>();
        bgImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    // 适配
    void Scaler()
    {
        if (canvas == null || bgImage == null)
        {
            Debug.LogError("Canvas or Image component not found.");
            return;
        }

        // 当前画布尺寸
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        // 当前画布尺寸长宽比
        float screenxyRate = canvasSize.x / canvasSize.y;

        // 图片尺寸
        float textureWidth = bgImage.mainTexture.width;
        float textureHeight = bgImage.mainTexture.height;
        // 视频尺寸长宽比
        float texturexyRate = textureWidth / textureHeight;

        // 视频x偏长,需要适配y（下面的判断 '>' 改为 '<' 就是视频播放器的视频方式）
        if (texturexyRate > screenxyRate)
        {
            int newSizeY = Mathf.CeilToInt(canvasSize.y);
            int newSizeX = Mathf.CeilToInt((float)newSizeY / textureHeight * textureWidth);
            rt.sizeDelta = new Vector2(newSizeX, newSizeY);
        }
        else
        {
            int newVideoSizeX = Mathf.CeilToInt(canvasSize.x);
            int newVideoSizeY = Mathf.CeilToInt((float)newVideoSizeX / textureWidth * textureHeight);
            rt.sizeDelta = new Vector2(newVideoSizeX, newVideoSizeY);
        }
    }

    // 在窗口大小变化时重新适配背景图片
    void OnRectTransformDimensionsChange()
    {
        Scaler();
    }
}
