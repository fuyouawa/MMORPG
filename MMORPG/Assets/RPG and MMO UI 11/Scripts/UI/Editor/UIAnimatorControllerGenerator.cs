using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	public class UIAnimatorControllerGenerator
	{
		/// <summary>
		/// Generate an the animator contoller.
		/// </summary>
		/// <returns>The animator contoller.</returns>
		/// <param name="triggersProperty">Triggers property.</param>
		/// <param name="preferredName">Preferred name.</param>
		public static UnityEditor.Animations.AnimatorController GenerateAnimatorContoller(SerializedProperty triggersProperty, string preferredName)
		{
			// Prepare the triggers list
			List<string> triggersList = new List<string>();
			
			SerializedProperty serializedProperty = triggersProperty.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			
			while (serializedProperty.NextVisible(true) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
			{
				triggersList.Add(!string.IsNullOrEmpty(serializedProperty.stringValue) ? serializedProperty.stringValue : serializedProperty.name);
			}
			
			// Generate the animator controller
			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggersList, preferredName);
		}

        /// <summary>
        /// Generates an animator contoller.
        /// </summary>
        /// <returns>The animator contoller.</returns>
        /// <param name="animationTriggers">Animation triggers.</param>
        /// <param name="preferredName">The preferred animator name.</param>
        public static UnityEditor.Animations.AnimatorController GenerateAnimatorContoller(List<string> animationTriggers, string preferredName)
        {
            return UIAnimatorControllerGenerator.GenerateAnimatorContoller(animationTriggers, preferredName, false);
        }

        /// <summary>
        /// Generates an animator contoller.
        /// </summary>
        /// <returns>The animator contoller.</returns>
        /// <param name="animationTriggers">Animation triggers.</param>
        /// <param name="preferredName">The preferred animator name.</param>
        /// <param name="initialState">If animator should have initial state.</param>
        public static UnityEditor.Animations.AnimatorController GenerateAnimatorContoller(List<string> animationTriggers, string preferredName, bool initialState)
		{
			if (string.IsNullOrEmpty(preferredName))
				preferredName = "New Animator Controller";
			
			string saveControllerPath = UIAnimatorControllerGenerator.GetSaveControllerPath(preferredName);
			
			if (string.IsNullOrEmpty(saveControllerPath))
				return null;
			
			UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);
			
            if (initialState)
                UIAnimatorControllerGenerator.GenerateInitialState(animatorController);

            foreach (string trigger in animationTriggers)
			{
				UIAnimatorControllerGenerator.GenerateTriggerableTransition(trigger, animatorController);
			}
			
			return animatorController;
		}
		
		private static string GetSaveControllerPath(string name)
		{
			string message = string.Format("Create a new animator controller with name '{0}':", name);
			return EditorUtility.SaveFilePanelInProject("New Animator Contoller", name, "controller", message);
		}
		
		private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
		{
			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
			AssetDatabase.AddObjectToAsset(animationClip, controller);
			AnimatorState animatorState = controller.AddMotion(animationClip);
			controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
			AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
			AnimatorStateTransition animatorStateTransition = stateMachine.AddAnyStateTransition(animatorState);
			animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0f, name);
			return animationClip;
		}

        private static AnimationClip GenerateInitialState(AnimatorController controller)
        {
            AnimationClip animationClip = AnimatorController.AllocateAnimatorClip("Initial");
            AssetDatabase.AddObjectToAsset(animationClip, controller);
            controller.AddMotion(animationClip);
            return animationClip;
        }

        public static void GenerateBool(string name, AnimatorController controller)
        {
            foreach (AnimatorControllerParameter param in controller.parameters)
            {
                if (param.name.Equals(name))
                    return;
            }

            controller.AddParameter(name, AnimatorControllerParameterType.Bool);
        }
    }
}
