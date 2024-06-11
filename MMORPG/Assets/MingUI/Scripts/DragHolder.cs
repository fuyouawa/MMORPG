using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

 
public class DragHolder : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Vector3 offset;

    [SerializeField]
    private GameObject dragObject;

    [SerializeField]
    private bool dragWithLeftButton = true;

    [SerializeField]
    private bool dragWithRightButton = true;


    public bool IsDragging {  get; private set; }

    private bool CanDrag(PointerEventData eventData)
    {
        // 检查鼠标按钮是否符合要求
        if ((eventData.button == PointerEventData.InputButton.Left && !dragWithLeftButton) ||
            (eventData.button == PointerEventData.InputButton.Right && !dragWithRightButton))
        {
            return false;
        }
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 检查是否可以拖拽
        if (!CanDrag(eventData)) return;
        // 将拖拽的对象置于最顶层
        dragObject.transform.SetAsLastSibling();
        // 记录初始位置和偏移量
        offset = dragObject.transform.position - Input.mousePosition;
        // 标记为正在拖拽中
        IsDragging = true;

    }

    public void OnDrag(PointerEventData eventData)
    {
        // 检查是否可以拖拽
        if (!CanDrag(eventData)) return;
        // 更新位置
        dragObject.transform.position = Input.mousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 检查是否可以拖拽
        if (!CanDrag(eventData)) return;
        // 取消拖拽标记
        IsDragging = false;
    }

}
