using System;
using UnityEngine;

namespace DuloGames.UI
{
    /// <summary>
    /// This components is used when using the UIUtility to bring objects forward.
    /// When an object is being brought forward any objects with this component will be kept on top in the specified order.
    /// </summary>
    [AddComponentMenu("UI/Always On Top", 8)]
    [DisallowMultipleComponent]
    public class UIAlwaysOnTop : MonoBehaviour, IComparable
    {
        public const int ModalBoxOrder              = 99996;
        public const int SelectFieldBlockerOrder    = 99997;
        public const int SelectFieldOrder           = 99998;
        public const int TooltipOrder               = 99999;

        [SerializeField] private int m_Order = 0;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int order { get { return this.m_Order; } set { this.m_Order = value; } }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                UIAlwaysOnTop comp = obj as UIAlwaysOnTop;

                if (comp != null)
                {
                    return this.order.CompareTo(comp.order);
                }
            }

            return 1;
        }
    }
}
