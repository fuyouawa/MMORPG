using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Raycast Filters/Mask Raycast Filter"), ExecuteInEditMode, RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
	public class UIMaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
	{
		private Image m_Image;
		private Sprite m_Sprite;
		
		[SerializeField, Range(0f, 1f)]
		private float m_AlphaTreshold = 0.01f;
		
		protected void Awake()
		{
			this.m_Image = this.gameObject.GetComponent<Image>();
			if (this.m_Image != null) this.m_Sprite = this.m_Image.sprite;
		}
	
#if UNITY_EDITOR
		protected void OnValidate()
		{
			// Check if the sprite texture is readable
			if (this.m_Sprite != null)
			{
				string path = AssetDatabase.GetAssetPath(this.m_Sprite.texture);
				TextureImporter textureAsset = (TextureImporter)AssetImporter.GetAtPath(path);
				
				// Make it readable if it's not
				if (!textureAsset.isReadable)
				{
					textureAsset.isReadable = true;
					AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				}
			}
		}
#endif
		
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			if (this.m_Image == null || this.m_Sprite == null)
				return false;
			
			RectTransform rectTransform = (RectTransform)this.transform;
			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out localPositionPivotRelative);
			
			// convert to bottom-left origin coordinates
			Vector2 localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width, localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);
			
			Rect spriteRect = this.m_Sprite.textureRect;
			Rect maskRect = rectTransform.rect;
			
			var x = 0;
			var y = 0;
			
			// convert to texture space
			switch (this.m_Image.type)
			{
				case Image.Type.Sliced:
				{
					Vector4 border = this.m_Sprite.border;
					
					// x slicing
					if (localPosition.x < border.x)
					{
						x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
					}
					else if (localPosition.x > maskRect.width - border.z)
					{
						x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
					} 
					else
					{
						x = Mathf.FloorToInt(spriteRect.x + border.x +
						                     ((localPosition.x - border.x)/
						 (maskRect.width - border.x - border.z)) *
						                     (spriteRect.width - border.x - border.z));
					}
					// y slicing
					if (localPosition.y < border.y)
					{
						y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
					}
					else if (localPosition.y > maskRect.height - border.w)
					{
						y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
					}
					else
					{
						y = Mathf.FloorToInt(spriteRect.y + border.y +
						                     ((localPosition.y - border.y) /
						 (maskRect.height - border.y - border.w)) *
						                     (spriteRect.height - border.y - border.w));
					}
				}
				break;
				case Image.Type.Simple:
				default:
				{
					// conversion to uniform UV space
					x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
					y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
				}
				break;
			}
			
			// Try to read the pixel
			try
			{
				return (this.m_Sprite.texture.GetPixel(x,y).a > this.m_AlphaTreshold);
			}
			catch (UnityException e)
			{
				Debug.LogError(e);
				Destroy(this);
				return false;
			}
		}
	}
}
