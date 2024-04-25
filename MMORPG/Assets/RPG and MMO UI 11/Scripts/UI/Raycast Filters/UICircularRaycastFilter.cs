using UnityEngine;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Raycast Filters/Circular Raycast Filter"), RequireComponent(typeof(RectTransform))]
	public class UICircularRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
	{
		[SerializeField] private bool m_RadiusInPercents = true;
		
		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="UICircularRaycastFilter"/> radius value is in percents.
		/// </summary>
		/// <value><c>true</c> if radius in percents; otherwise, <c>false</c>.</value>
		public bool radiusInPercents
		{
			get
			{
				return this.m_RadiusInPercents;
			}
			set
			{
				this.m_RadiusInPercents = value;
			}
		}
		
		[Range(0f, 100f)]
		[SerializeField] private float m_Radius = 70f;
		
		/// <summary>
		/// Gets or sets the radius.
		/// </summary>
		/// <value>The radius.</value>
		public float radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = value;
			}
		}
		
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
		
		/// <summary>
		/// Gets the center of the rect from bottom-left point.
		/// </summary>
		/// <value>The center.</value>
		public Vector2 center
		{
			get
			{
				RectTransform rt = (RectTransform)this.transform;
				return new Vector2((rt.rect.width / 2f), (rt.rect.height / 2f));
			}
		}
		
		/// <summary>
		/// Gets the rect max radius.
		/// </summary>
		/// <returns>The rect max radius.</returns>
		private float rectMaxRadius
		{
			get
			{
				return Mathf.Sqrt((this.center.x * this.center.x) + (this.center.y * this.center.y));
			}
		}
		
		/// <summary>
		/// Gets the operational radius.
		/// </summary>
		/// <value>The operational radius.</value>
		public float operationalRadius
		{
			get
			{
				return (this.m_RadiusInPercents ? (this.rectMaxRadius * (this.m_Radius / 100f)) : this.m_Radius);
			}
		}
		
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (!this.enabled)
				return true;
			
			if (this.m_Radius == 0f)
				return false;
			
			RectTransform rectTransform = (RectTransform)this.transform;
			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)this.transform, screenPoint, eventCamera, out localPositionPivotRelative);
			
			// convert to bottom-left origin coordinates
			Vector2 localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
			                                localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);
			
			// Add the offset to the rect center
			Vector2 center = this.offset + this.center;
			
			// Check if the mouse is within the radius
			return (Vector2.Distance(localPosition, center) <= this.operationalRadius);
		}
	}
}
