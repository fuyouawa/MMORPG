using System;

namespace DuloGames.UI
{
	[Flags]
	public enum UISpellInfo_Flags
	{
		Passive = (1 << 0),
		InstantCast = (1 << 1),
		PowerCostInPct = (1 << 2),
	}
	
	public static class UISpellInfo_FlagsExtensions
	{
		public static bool Has(this UISpellInfo_Flags type, UISpellInfo_Flags value)
		{
			try {
				return (((int)type & (int)value) == (int)value);
			} 
			catch {
				return false;
			}
		}
		
		public static bool Is(this UISpellInfo_Flags type, UISpellInfo_Flags value)
		{
			try {
				return (int)type == (int)value;
			}
			catch {
				return false;
			}    
		}
	
		public static UISpellInfo_Flags Add(this UISpellInfo_Flags type, UISpellInfo_Flags value)
		{
			try {
				return (UISpellInfo_Flags)(((int)type | (int)value));
			}
			catch(Exception ex)
			{
				throw new ArgumentException(string.Format("Could not append value from enumerated type '{0}'.", typeof(UISpellInfo_Flags).Name), ex);
			}    
		}
	
		public static UISpellInfo_Flags Remove(this UISpellInfo_Flags type, UISpellInfo_Flags value)
		{
			try {
				return (UISpellInfo_Flags)(((int)type & ~(int)value));
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format("Could not remove value from enumerated type '{0}'.", typeof(UISpellInfo_Flags).Name), ex);
			}  
		}
	}
}
