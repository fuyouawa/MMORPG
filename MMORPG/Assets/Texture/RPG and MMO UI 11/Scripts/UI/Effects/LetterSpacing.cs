using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*

Produces an simple tracking/letter-spacing effect on UI Text components.

Set the spacing parameter to adjust letter spacing.
  Negative values cuddle the text up tighter than normal. Go too far and it'll look odd.
  Positive values spread the text out more than normal. This will NOT respect the text area you've defined.
  Zero spacing will present the font with no changes.

Relies on counting off characters in your Text compoennt's text property and
matching those against the quads passed in via the verts array. This is really
rather primative, but I can't see any better way at the moment. It means that
all sorts of things can break the effect...

This component should be placed higher in component list than any other vertex
modifiers that alter the total number of verticies. Eg, place this above Shadow
or Outline effects. If you don't, the outline/shadow won't match the position
of the letters properly. If you place the outline/shadow effect second however,
it will just work on the altered vertices from this component, and function
as expected.

This component works best if you don't allow text to automatically wrap. It also
blows up outside of the given text area. Basically, it's a cheap and dirty effect,
not a clever text layout engine. It can't affect how Unity chooses to break up
your lines. If you manually use line breaks however, it should detect those and
function more or less as you'd expect.

The spacing parameter is measured in pixels multiplied by the font size. This was
chosen such that when you adjust the font size, it does not change the visual spacing
that you've dialed in. There's also a scale factor of 1/100 in this number to
bring it into a comfortable adjustable range. There's no limit on this parameter,
but obviously some values will look quite strange.

This component doesn't really work with Rich Text. You don't need to remember to
turn off Rich Text via the checkbox, but because it can't see what makes a
printable character and what doesn't, it will typically miscount characters when you
use HTML-like tags in your text. Try it out, you'll see what I mean. It doesn't
break down entirely, but it doesn't really do what you'd want either.

*/

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Effects/Letter Spacing", 14), RequireComponent(typeof(Text))]
    public class LetterSpacing : BaseMeshEffect, ILayoutElement
	{
		[SerializeField] private float m_spacing = 0f;
		
		public float spacing
		{
			get { return this.m_spacing; }
			set
			{
				if (this.m_spacing == value) return;
				this.m_spacing = value;
				if (this.graphic != null) this.graphic.SetVerticesDirty();
				LayoutRebuilder.MarkLayoutForRebuild((RectTransform)this.transform);
            }
		}
		
		private Text text
		{
			get { return this.gameObject.GetComponent<Text>(); }
		}
		
		public float minWidth
		{
			get { return this.text.minWidth; }
		}
		
		public float preferredWidth
		{
			get { return ((this.spacing * (float)this.text.fontSize / 100f) * (this.text.text.Length - 1)) + this.text.preferredWidth; }
		}
		
		public float flexibleWidth
		{
			get { return this.text.flexibleWidth; }
		}
		
		public float minHeight
		{
			get { return this.text.minHeight; }
		}
		
		public float preferredHeight
		{
			get { return this.text.preferredHeight; }
		}
		
		public float flexibleHeight
		{
			get { return this.text.flexibleHeight; }
		}
		
		public int layoutPriority
		{
			get { return this.text.layoutPriority; }
		}
		
		protected LetterSpacing() { }
		
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
            if (this.graphic != null) this.graphic.SetVerticesDirty();
			LayoutRebuilder.MarkLayoutForRebuild((RectTransform)this.transform);
        }
#endif

        public void CalculateLayoutInputHorizontal() { }
		public void CalculateLayoutInputVertical() { }
		
		private string[] GetLines()
		{
			IList<UILineInfo> lineInfos = text.cachedTextGenerator.lines;
			string[] lines = new string[lineInfos.Count];
            
			for (int i = 0; i < lineInfos.Count; i++)
			{
				if (i < (lineInfos.Count - 1))
				{
                    UILineInfo nextLine = lineInfos[i + 1];
                    int lineStart = lineInfos[i].startCharIdx;
                    int length = (nextLine.startCharIdx - 1) - lineStart;
                    
                    lines[i] = this.text.text.Substring(lineInfos[i].startCharIdx, length);
				}
			    else
				{
					lines[i] = this.text.text.Substring(lineInfos[i].startCharIdx);
				}
			}
            
            return lines;
		}
        
        public override void ModifyMesh(VertexHelper vertexHelper)
		{
			if (!this.IsActive())
				return;
			
			List<UIVertex> list = new List<UIVertex>();
			vertexHelper.GetUIVertexStream(list);
			
			ModifyVertices(list);  // calls the old ModifyVertices which was used on pre 5.2
			
			vertexHelper.Clear();
			vertexHelper.AddUIVertexTriangleStream(list);
		}
        
        public void ModifyVertices(List<UIVertex> verts)
		{
			if (!this.IsActive() || verts.Count == 0)
                return;
			
			string[] lines = this.GetLines();
            
			Vector3 pos;
			float letterOffset = this.spacing * (float)this.text.fontSize / 100f;
			float alignmentFactor = 0;
			int glyphIdx = 0;
			
			switch (this.text.alignment)
			{
			case TextAnchor.LowerLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.UpperLeft:
				alignmentFactor = 0f;
				break;
				
			case TextAnchor.LowerCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.UpperCenter:
				alignmentFactor = 0.5f;
				break;
				
			case TextAnchor.LowerRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.UpperRight:
				alignmentFactor = 1f;
				break;
			}
			
			for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
			{
				string line = lines[lineIdx];
				float lineOffset = ((line.Length - 1) * letterOffset) * alignmentFactor;
				
				for (int charIdx = 0; charIdx < line.Length; charIdx++)
				{
					int idx1 = glyphIdx * 6 + 0;
					int idx2 = glyphIdx * 6 + 1;
					int idx3 = glyphIdx * 6 + 2;
					int idx4 = glyphIdx * 6 + 3;
					int idx5 = glyphIdx * 6 + 4;
					int idx6 = glyphIdx * 6 + 5;
					
					// Check for truncated text (doesn't generate verts for all characters)
					if (idx6 > verts.Count - 1) return;
					
					UIVertex vert1 = verts[idx1];
					UIVertex vert2 = verts[idx2];
					UIVertex vert3 = verts[idx3];
					UIVertex vert4 = verts[idx4];
					UIVertex vert5 = verts[idx5];
					UIVertex vert6 = verts[idx6];
					
					pos = Vector3.right * ((letterOffset * charIdx) - lineOffset);
					
					vert1.position += pos;
					vert2.position += pos;
					vert3.position += pos;
					vert4.position += pos;
					vert5.position += pos;
					vert6.position += pos;
					
					verts[idx1] = vert1;
					verts[idx2] = vert2;
					verts[idx3] = vert3;
					verts[idx4] = vert4;
					verts[idx5] = vert5;
					verts[idx6] = vert6;
					
					glyphIdx++;
				}
				
				// Offset for carriage return character that still generates verts
				glyphIdx++;
			}
		}
	}
}
