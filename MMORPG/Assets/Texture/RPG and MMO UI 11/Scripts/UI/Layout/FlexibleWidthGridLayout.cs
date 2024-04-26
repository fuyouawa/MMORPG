using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    /// <summary>
    /// In this class we calculate board cell width prior to layout calculation.
    /// </summary>
    public class FlexibleWidthGridLayout : GridLayoutGroup
    {
        public override void SetLayoutHorizontal()
        {
            UpdateCellSize();
            base.SetLayoutHorizontal();
        }

        public override void SetLayoutVertical()
        {
            UpdateCellSize();
            base.SetLayoutVertical();
        }

        private void UpdateCellSize()
        {
            float x = (rectTransform.rect.size.x - padding.horizontal - spacing.x * (this.constraintCount - 1)) / this.constraintCount;
            this.constraint = Constraint.FixedColumnCount;
            this.cellSize = new Vector2(x, this.cellSize.y);
        }
    }
}
