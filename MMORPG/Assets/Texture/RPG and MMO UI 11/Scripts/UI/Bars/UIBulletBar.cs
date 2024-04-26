using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/Bars/Bullet Bar"), RequireComponent(typeof(RectTransform))]
    public class UIBulletBar : UIBehaviour, IUIProgressBar
    {
        public enum BarType
        {
            Horizontal,
            Vertical,
            Radial
        }

        #pragma warning disable 0649
        [SerializeField] private BarType m_BarType = BarType.Horizontal;

        [SerializeField] private bool m_FixedSize = false;
        [SerializeField] private Vector2 m_BulletSize = Vector2.zero;

        [SerializeField] private Sprite m_BulletSprite;
        [SerializeField] private Color m_BulletSpriteColor = Color.white;
        [SerializeField] private Sprite m_BulletSpriteActive;
        [SerializeField] private Color m_BulletSpriteActiveColor = Color.white;

        [SerializeField] private float m_SpriteRotation = 0f;
        [SerializeField] private Vector2 m_ActivePosition = Vector2.zero;
        
        [SerializeField][Range(0f, 360f)] private float m_AngleMin = 0f;
        [SerializeField][Range(0f, 360f)] private float m_AngleMax = 360f;
        [SerializeField] private int m_BulletCount = 10;
        [SerializeField] private float m_Distance = 100f;

        [SerializeField][Range(0f, 1f)] private float m_FillAmount = 1f;
        [SerializeField] private bool m_InvertFill = true;

        [SerializeField][HideInInspector] private GameObject m_BulletsContainer = null;
        [SerializeField][HideInInspector] private List<GameObject> m_FillBullets;
        #pragma warning restore 0649

        /// <summary>
        /// Gets or sets the fill amount (0 to 1).
        /// </summary>
        public float fillAmount
        {
            get { return this.m_FillAmount; }
            set {
                this.m_FillAmount = Mathf.Clamp01(value);
                this.UpdateFill();
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the fill should be inverted.
        /// </summary>
        public bool invertFill
        {
            get { return this.m_InvertFill; }
            set {
                this.m_InvertFill = value;
                this.UpdateFill();
            }
        }

        /// <summary>
        /// Gets the rect transform.
        /// </summary>
        public RectTransform rectTransform
        {
            get { return this.transform as RectTransform; }
        }

        protected override void Start()
        {
            base.Start();

            if (this.m_BulletSprite == null || this.m_BulletSpriteActive == null)
            {
                Debug.LogWarning("The Bullet Bar script on game object " + this.gameObject.name + " requires that both bullet sprites are assigned to work.");
                this.enabled = false;
                return;
            }

            // Check if the bullets are constructed
            if (this.m_BulletsContainer == null)
            {
                this.ConstructBullets();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            // Update the bar fill
            this.UpdateFill();
        }
#endif

        /// <summary>
        /// Updates the fill.
        /// </summary>
        public void UpdateFill()
        {
            if (!this.isActiveAndEnabled || this.m_FillBullets == null || this.m_FillBullets.Count == 0)
                return;

            GameObject[] list = this.m_FillBullets.ToArray();

            if (this.m_InvertFill)
            {
                System.Array.Reverse(list);
            }

            int index = 0;
            foreach (GameObject go in list)
            {
                float currentPct = (float)index / (float)this.m_BulletCount;
                
                Image img = go.GetComponent<Image>();
                if (img != null)
                {
                    img.enabled = (this.m_FillAmount > 0f && currentPct <= this.m_FillAmount);
                }

                index++;
            }
        }

        /// <summary>
        /// Constructs the bullet game objects and components.
        /// </summary>
        public void ConstructBullets()
        {
            if (this.m_BulletSprite == null || this.m_BulletSpriteActive == null || !this.isActiveAndEnabled)
                return;

            // Destroy the old bullets
            this.DestroyBullets();

            // Create the container
            this.m_BulletsContainer = new GameObject("Bullets", typeof(RectTransform));
            this.m_BulletsContainer.transform.SetParent(this.transform);
            this.m_BulletsContainer.layer = this.gameObject.layer;

            RectTransform crt = this.m_BulletsContainer.transform as RectTransform;
            crt.localScale = new Vector3(1f, 1f, 1f);
            crt.sizeDelta = this.rectTransform.sizeDelta;
            crt.localPosition = Vector3.zero;
            crt.anchoredPosition = Vector2.zero;

            // Create new bullets
            for (int i = 0; i < this.m_BulletCount; i++)
            {
                float pct = (float)i / (float)this.m_BulletCount;

                // Create the background
                GameObject obj = new GameObject("Bullet " + i.ToString(), typeof(RectTransform));
                obj.transform.SetParent(this.m_BulletsContainer.transform);
                obj.layer = this.gameObject.layer;

                RectTransform rt = obj.transform as RectTransform;
                rt.localScale = new Vector3(1f, 1f, 1f);
                rt.localPosition = Vector3.zero;

                Image img = obj.AddComponent<Image>();
                img.sprite = this.m_BulletSprite;
                img.color = this.m_BulletSpriteColor;

                if (this.m_FixedSize)
                    rt.sizeDelta = this.m_BulletSize;
                else
                    img.SetNativeSize();

                // Position the bullet
                if (this.m_BarType == BarType.Radial)
                {
                    float ang = (this.m_AngleMin + (pct * (this.m_AngleMax - this.m_AngleMin)));

                    Vector2 pos;
                    pos.x = 0f + this.m_Distance * Mathf.Sin(ang * Mathf.Deg2Rad);
                    pos.y = 0f + this.m_Distance * Mathf.Cos(ang * Mathf.Deg2Rad);

                    rt.anchoredPosition = pos;
                    rt.Rotate(new Vector3(0f, 0f, ((this.m_SpriteRotation + ang) * -1f)));
                }
                else if (this.m_BarType == BarType.Horizontal)
                {
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchorMin = new Vector2(1f, 0.5f);
                    rt.anchorMax = new Vector2(1f, 0.5f);

                    float occupiedSpace = rt.sizeDelta.x * this.m_BulletCount;
                    float freeSpace = this.rectTransform.rect.width - occupiedSpace;
                    float spacing = freeSpace / (this.m_BulletCount - 1);
                    
                    float offsetX = (rt.sizeDelta.x * i) + (spacing * i);

                    Vector2 pos;
                    pos.x = (offsetX + (rt.sizeDelta.x / 2f)) * -1f;
                    pos.y = 0f;

                    rt.anchoredPosition = pos;
                    rt.Rotate(new Vector3(0f, 0f, this.m_SpriteRotation));
                }
                else if (this.m_BarType == BarType.Vertical)
                {
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchorMin = new Vector2(0.5f, 1f);
                    rt.anchorMax = new Vector2(0.5f, 1f);

                    float occupiedSpace = rt.sizeDelta.y * this.m_BulletCount;
                    float freeSpace = this.rectTransform.rect.height - occupiedSpace;
                    float spacing = freeSpace / (this.m_BulletCount - 1);

                    float offsetY = (rt.sizeDelta.y * i) + (spacing * i);
                    
                    Vector2 pos;
                    pos.x = 0f;
                    pos.y = (offsetY + (rt.sizeDelta.y / 2f)) * -1f;

                    rt.anchoredPosition = pos;
                    rt.Rotate(new Vector3(0f, 0f, this.m_SpriteRotation));
                }

                // Create the fill
                GameObject objFill = new GameObject("Fill", typeof(RectTransform));
                objFill.transform.SetParent(obj.transform);
                objFill.layer = this.gameObject.layer;

                RectTransform rtFill = objFill.transform as RectTransform;
                rtFill.localScale = new Vector3(1f, 1f, 1f);
                rtFill.localPosition = Vector3.zero;
                rtFill.anchoredPosition = this.m_ActivePosition;
                rtFill.rotation = rt.rotation;

                Image imgFill = objFill.AddComponent<Image>();
                imgFill.sprite = this.m_BulletSpriteActive;
                imgFill.color = this.m_BulletSpriteActiveColor;

                if (this.m_FixedSize)
                    rtFill.sizeDelta = this.m_BulletSize;
                else
                    imgFill.SetNativeSize();
                
                // Add the fill bullet to the list
                this.m_FillBullets.Add(objFill);
            }

            // Update
            this.UpdateFill();
        }

        protected void DestroyBullets()
        {
            // Clear the list
            this.m_FillBullets.Clear();

            GameObject go = this.m_BulletsContainer;

            // Destroy bullets
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(go);
                };
#endif
            }
            else Destroy(go);

            // Null the variable
            this.m_BulletsContainer = null;
        }
    }
}
