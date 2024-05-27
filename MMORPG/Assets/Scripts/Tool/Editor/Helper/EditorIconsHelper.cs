// using Sirenix.OdinInspector.Editor;
// using Sirenix.OdinInspector;
// using Sirenix.Utilities.Editor;
// using UnityEngine;
//
// namespace MMORPG.Tool
// {
//     public class EditorIconsHelper
//     {
//         private static Texture2D bigUnityInfoIcon;
//
//         public static Texture2D BigUnityInfoIcon
//         {
//             get
//             {
//                 if (bigUnityInfoIcon == null)
//                 {
//                     bigUnityInfoIcon = SdfIcons.CreateTransparentIconTexture(SdfIconType.InfoCircleFill, SirenixGUIStyles.HighlightedTextColor, 22, 22, 2);
//                     CleanupUtility.DestroyObjectOnAssemblyReload(bigUnityInfoIcon);
//                 }
//                 return bigUnityInfoIcon;
//             }
//         }
//
//         private static Texture2D bigUnityWarningIcon;
//
//         public static Texture2D BigUnityWarningIcon
//         {
//             get
//             {
//                 if (bigUnityWarningIcon == null)
//                 {
//                     bigUnityWarningIcon = SdfIcons.CreateTransparentIconTexture(SdfIconType.ExclamationTriangleFill, SirenixGUIStyles.YellowWarningColor, 22, 22, 2);
//                     CleanupUtility.DestroyObjectOnAssemblyReload(bigUnityWarningIcon);
//                 }
//                 return bigUnityWarningIcon;
//             }
//         }
//
//         private static Texture2D bigUnityErrorIcon;
//
//         public static Texture2D BigUnityErrorIcon
//         {
//             get
//             {
//                 if (bigUnityErrorIcon == null)
//                 {
//                     bigUnityErrorIcon = SdfIcons.CreateTransparentIconTexture(SdfIconType.ExclamationOctagonFill, SirenixGUIStyles.RedErrorColor, 22, 22, 2);
//                     CleanupUtility.DestroyObjectOnAssemblyReload(bigUnityErrorIcon);
//                 }
//                 return bigUnityErrorIcon;
//             }
//         }
//     }
// }
