using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwordEffectController))]
[CanEditMultipleObjects]
public class WeaponEffectEditor : Editor
{
    public override void OnInspectorGUI () {
         DrawDefaultInspector();
         SwordEffectController swordEffectController = (SwordEffectController) target;
         if(GUILayout.Button("Activate Vfx 1")) {
             swordEffectController.ActivateVFX1();
         }
         if(GUILayout.Button("Activate Vfx 2")) {
             swordEffectController.ActivateVFX2();
         }
         if(GUILayout.Button("Activate Vfx 3")) {
             swordEffectController.ActivateVFX3();
         }
         if(GUILayout.Button("Activate Vfx 4")) {
             swordEffectController.ActivateVFX4();
         }
         if(GUILayout.Button("Activate Vfx 5")) {
             swordEffectController.ActivateVFX5();
         }
         if(GUILayout.Button("Activate Vfx 6")) {
             swordEffectController.ActivateVFX6();
         }     

        if(GUILayout.Button("Disable Effects")) {
             swordEffectController.RemoveExistingEffects();
         }         

         
    }
}
