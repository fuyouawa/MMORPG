using UnityEngine;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Raycast Filters/Rectangular Raycast Filter"), RequireComponent(typeof(RectTransform))]
	public class UIRectangularRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
	{
		[SerializeField] private Vector2 m_Offset = Vector2.zero;
		
		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public Vector2 offset
		{
			get
			{
				return this.m_Offset;
			}
			set
			{
				this.m_Offset = value;
			}
		}
		
		[SerializeField] private RectOffset m_Borders = new RectOffset();
		
		/// <summary>
		/// Gets or sets the borders.
		/// </summary>
		/// <value>The borders.</value>
		public RectOffset borders
		{
			get
			{
				return this.m_Borders;
			}
			set
			{
				this.m_Borders = value;
			}
		}
		
		[Range(0f, 1f)]
		[SerializeField] private float m_ScaleX = 1f;
		
		/// <summary>
		/// Gets or sets the X scale.
		/// </summary>
		/// <value>The X scale.</value>
		public float scaleX
		{
			get
			{
				return this.m_ScaleX;
			}
			set
			{
				this.m_ScaleX = value;
			}
		}
		
		[Range(0f, 1f)]
		[SerializeField] private float m_ScaleY = 1f;
		
		/// <summary>
		/// Gets or sets the Y scale.
		/// </summary>
		/// <value>The Y scale.</value>
		public float scaleY
		{
			get
			{
				return this.m_ScaleY;
			}
			set
			{
				this.m_ScaleY = value;
			}
		}
		
		/// <summary>
		/// Gets the scaled rect including the offset.
		/// </summary>
		/// <value>The scaled rect.</value>
		public Rect scaledRect
		{
			get
			{
				RectTransform rt = (RectTransform)this.transform;
				return new Rect(
					(this.offset.x + this.borders.left + (rt.rect.x + ((rt.rect.width - (rt.rect.width * this.m_ScaleX)) / 2f))), 
					(this.offset.y + this.borders.bottom + (rt.rect.y + ((rt.rect.height - (rt.rect.height * this.m_ScaleY)) / 2f))), 
					((rt.rect.width * this.m_ScaleX) - this.borders.left - this.borders.right), 
					((rt.rect.height * this.m_ScaleY) - this.borders.top - borders.bottom)
				);
			}
		}
		
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (!this.enabled)
				return true;
			
			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)this.transform, screenPoint, eventCamera, out localPositionPivotRelative);
			return this.scaledRect.Contains(localPositionPivotRelative);
		}
	}
}
