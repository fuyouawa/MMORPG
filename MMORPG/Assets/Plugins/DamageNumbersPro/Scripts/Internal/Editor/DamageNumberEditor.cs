#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.Rendering;
using DamageNumbersPro.Internal;
using UnityEditor.SceneManagement;

namespace DamageNumbersPro
{
    [CustomEditor(typeof(DamageNumber), true), CanEditMultipleObjects]
    public class DamageNumberEditor : Editor
    {
        public static string version = "4.33";

        void OnEnable()
        {
            DNPEditorInternal.PrepareInspector(this);
        }

        public override void OnInspectorGUI()
        {
            //For some operations:
            bool isMesh = DNPEditorInternal.damageNumbers[0].IsMesh();

            //Internal:
            DNPEditorInternal.OnInspectorGUI(this);
            DNPEditorInternal.BeginInspector();
            DNPEditorInternal.DrawInspectorTop(isMesh);

            //Structure:
            if (DNPEditorInternal.CheckStructure(this))
            {
                DNPEditorInternal.EndInspector();
                return;
            }

            //Detect and warn about common mistakes.
            WarningCheck(isMesh);

            //Update:
            serializedObject.Update();

            switch(DNPEditorInternal.currentTab)
            {
                case (0): //Main
                    DisplayMainSettings();
                    DisplayFeature("enable3DGame", "3D Game");
                    break;
                case (1): //Text
                    DisplayTextMain(isMesh);
                    DisplayFeature("enableNumber", "Number");
                    DisplayFeature("enableLeftText", "Left Text");
                    DisplayFeature("enableRightText", "Right Text");
                    DisplayFeature("enableTopText", "Top Text");
                    DisplayFeature("enableBottomText", "Bottom Text");
                    DisplayFeature("enableColorByNumber", "Color By Number");
                    break;
                case (2): //Fade In
                    DisplayFadeMain("In");
                    DisplayFeature("enableOffsetFadeIn", "Offset In", "Offset");
                    DisplayFeature("enableScaleFadeIn", "Scale In", "Scale");
                    DisplayFeature("enableShakeFadeIn", "Shake In", "Shake");
                    DisplayFeature("enableCrossScaleFadeIn", "Cross Scale In", "Cross Scale");
                    break;
                case (3): //Fade Out
                    DisplayFadeMain("Out");
                    DisplayFeature("enableOffsetFadeOut", "Offset Out", "Offset");
                    DisplayFeature("enableScaleFadeOut", "Scale Out", "Scale");
                    DisplayFeature("enableShakeFadeOut", "Shake Out", "Shake");
                    DisplayFeature("enableCrossScaleFadeOut", "Cross Scale Out", "Cross Scale");
                    break;
                case (4): //Movement
                    DisplayFeature("enableLerp", "Lerp");
                    DisplayFeature("enableVelocity", "Velocity");
                    DisplayFeature("enableShaking", "Shaking");
                    DisplayFeature("enableFollowing", "Following");
                    DisplayMovementHints(isMesh);
                    break;
                case (5): //Scale
                    DisplayFeature("enableStartRotation", "Start Rotation");
                    DisplayFeature("enableRotateOverTime", "Rotate Over Time");
                    DisplayFeature("enableScaleByNumber", "Scale By Number");
                    DisplayFeature("enableScaleOverTime", "Scale Over Time");
                    break;
                case (6): //Spam Control
                    DisplaySpamControlMain(isMesh);
                    DisplayFeature("enableCombination", "Combination");
                    DisplayFeature("enableDestruction", "Destruction");
                    DisplayFeature("enableCollision", "Collision");
                    DisplayFeature("enablePush", "Push");
                    DisplaySpamControlHints(isMesh);
                    break;
                case (7): //Performance
                    DisplayPerformanceMain();
                    DisplayFeature("enablePooling", "Pooling");
                    DisplayPerformanceHints();
                    break;
            }

            //Fix Variables:
            foreach(DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                FixAllVariables(dn);
            }

            //Apply Properties:
            serializedObject.ApplyModifiedProperties();

            //Update Text:
            if (Application.isPlaying == false)
            {
                foreach (DamageNumber damageNumber in DNPEditorInternal.damageNumbers)
                {
                    if(damageNumber.gameObject.activeInHierarchy)
                    {
                        if (isMesh)
                        {
                            DNPEditorInternal.FixTextMeshPro();
                        }

                        damageNumber.UpdateText();
                        damageNumber.UpdateAlpha(1);
                    }
                }
            }

            //External Editors:
            DNPEditorInternal.Externalnspectors(isMesh, target);
            DNPEditorInternal.FinalInformation();

            DNPEditorInternal.EndInspector();
        }

        #region Editor Menus
        [MenuItem("GameObject/Damage Numbers Pro/Damage Number (Mesh)", priority = 1)]
        public static void CreateDamageNumberMesh(MenuCommand menuCommand)
        {
            //Create GameObject:
            GameObject newDN = new GameObject("Damage Number (Mesh)");
            GameObjectUtility.SetParentAndAlign(newDN, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(newDN, "Create " + newDN.name);
            Selection.activeObject = newDN;

            //Add damage number component.
            newDN.AddComponent<DamageNumberMesh>();

            //Prepare Structure:
            DNPEditorInternal.PrepareMeshStructure(newDN);

            //Position:
            if(Camera.main != null)
            {
                //Position in front of the camera.
                Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 10;

                //Set Z to 0 for 2D Games.
                if(Camera.main.orthographic)
                {
                    position.z = 0;
                }

                newDN.transform.position = position;
            }
        }

        [MenuItem("GameObject/Damage Numbers Pro/Damage Number (GUI)", priority = 2)]
        public static void CreateDamageNumberGUI(MenuCommand menuCommand)
        {
            //Create GameObject:
            GameObject newDN = new GameObject("Damage Number (GUI)");
            GameObjectUtility.SetParentAndAlign(newDN, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(newDN, "Create " + newDN.name);
            Selection.activeObject = newDN;

            //Add damage number component.
            newDN.AddComponent<DamageNumberGUI>();

            //Prepare Structure:
            DNPEditorInternal.PrepareGUIStructure(newDN);

            //Position:
            if (Camera.main != null)
            {
                //Position in front of the camera.
                Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 10;

                //Set Z to 0 for 2D Games.
                if (Camera.main.orthographic)
                {
                    position.z = 0;
                }

                newDN.transform.position = position;
            }
        }

        [MenuItem("CONTEXT/DamageNumberMesh/Switch to GUI (experimental)")]
        public static void SwitchToGUI(MenuCommand menuCommand)
        {
            foreach(GameObject selected in Selection.gameObjects)
            {
                DamageNumberMesh oldComponent = selected.GetComponent<DamageNumberMesh>();
                Undo.RecordObject(oldComponent.gameObject, "Switched " + oldComponent.name);

                if (oldComponent != null)
                {
                    //Get TMP Settings:
                    TMP_FontAsset fontAsset = oldComponent.GetFontMaterial();
                    Color tmpColor = oldComponent.GetTextMesh().color;
                    VertexGradient colorGradient = oldComponent.GetTextMesh().colorGradient;
                    bool enableGradient = oldComponent.GetTextMesh().enableVertexGradient;

                    //Clear Children:
                    while (oldComponent.transform.childCount > 0)
                    {
                        Object.DestroyImmediate(oldComponent.transform.GetChild(0).gameObject);
                    }

                    //Add GUI Component:
                    DamageNumberGUI newComponent = oldComponent.gameObject.AddComponent<DamageNumberGUI>();
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    DNPEditorInternal.PrepareGUIStructure(selected);

                    //TMP Settings:
                    newComponent.SetFontMaterial(fontAsset);
                    foreach(TMP_Text tmp in newComponent.GetTextMeshs())
                    {
                        tmp.color = tmpColor;
                        tmp.enableVertexGradient = enableGradient;
                        tmp.colorGradient = colorGradient;
                    }

                    //Settings:
                    foreach (System.Reflection.FieldInfo propA in oldComponent.GetType().GetFields())
                    {
                        System.Reflection.FieldInfo propB = newComponent.GetType().GetField(propA.Name);

                        if(propB.IsPublic && propB.IsPublic)
                        {
                            propB.SetValue(newComponent, propA.GetValue(oldComponent));
                        }
                    }

                    //3D:
                    newComponent.enable3DGame = false;

                    //Remove old component:
                    DestroyImmediate(oldComponent);
                }
            }
        }
        [MenuItem("CONTEXT/DamageNumberGUI/Switch to Mesh (experimental)")]
        public static void SwitchToMesh(MenuCommand menuCommand)
        {
            foreach (GameObject selected in Selection.gameObjects)
            {
                DamageNumberGUI previousComponent = selected.GetComponent<DamageNumberGUI>();
                Undo.RecordObject(previousComponent.gameObject, "Switched " + previousComponent.name);

                if (previousComponent != null)
                {
                    //Get TMP Settings:
                    TMP_FontAsset fontAsset = previousComponent.GetFontMaterial();
                    Color tmpColor = previousComponent.GetTextMesh().color;
                    VertexGradient colorGradient = previousComponent.GetTextMesh().colorGradient;
                    bool enableGradient = previousComponent.GetTextMesh().enableVertexGradient;

                    //Clear Children:
                    while (previousComponent.transform.childCount > 0)
                    {
                        Object.DestroyImmediate(previousComponent.transform.GetChild(0).gameObject);
                    }

                    //Add GUI Component:
                    DamageNumberMesh newComponent = previousComponent.gameObject.AddComponent<DamageNumberMesh>();
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                    DNPEditorInternal.PrepareMeshStructure(selected);

                    //TMP Settings:
                    newComponent.SetFontMaterial(fontAsset);
                    foreach (TMP_Text tmp in newComponent.GetTextMeshs())
                    {
                        tmp.color = tmpColor;
                        tmp.enableVertexGradient = enableGradient;
                        tmp.colorGradient = colorGradient;
                    }

                    //Settings:
                    foreach (System.Reflection.FieldInfo propA in previousComponent.GetType().GetFields())
                    {
                        System.Reflection.FieldInfo propB = newComponent.GetType().GetField(propA.Name);

                        if (propB.IsPublic && propB.IsPublic)
                        {
                            propB.SetValue(newComponent, propA.GetValue(previousComponent));
                        }
                    }

                    //3D:
                    newComponent.CheckAndEnable3D();

                    //Remove old components:
                    DestroyImmediate(previousComponent);
                    RectTransform rectComponent = selected.GetComponent<RectTransform>();
                    if(rectComponent != null)
                    {
                        Debug.Log("Please rightclick on the RectTransform component and select 'Remove Component'.");
                    }
                }
            }
        }
        #endregion

        #region Fixing Variables
        void FixAllVariables(DamageNumber dn)
        {
            //Performance:
            dn.updateDelay = Mathf.Max(0, dn.updateDelay);

            //Main:
            dn.lifetime = Mathf.Max(0, dn.lifetime);
            dn.distanceScalingSettings.baseDistance = Mathf.Max(1, dn.distanceScalingSettings.baseDistance);

            //Text:
            dn.numberSettings = FixTextSettings(dn.numberSettings);
            dn.leftTextSettings = FixTextSettings(dn.leftTextSettings);
            dn.rightTextSettings = FixTextSettings(dn.rightTextSettings);

            //Fade:
            dn.durationFadeIn = Mathf.Max(0, dn.durationFadeIn);
            dn.durationFadeOut = Mathf.Max(0, dn.durationFadeOut);

            //Digit:
            dn.digitSettings.decimals = Mathf.Max(0, dn.digitSettings.decimals);
            dn.digitSettings.dotDistance = Mathf.Max(0, dn.digitSettings.dotDistance);

            //Text Shader:
            TMP_FontAsset font = dn.GetFontMaterial();
            string is3D = dn.enable3DGame ? "3D" : "";
            if(font != null && font.name + is3D != dn.editorLastFont)
            {
                dn.editorLastFont = font.name + is3D;

                Object[] objects = new Object[2];
                objects[0] = dn;
                objects[1] = font;
                Undo.RecordObjects(objects, "Swiched shader to distance field overlay.");

                if(GraphicsSettings.currentRenderPipeline != null)
                {
                    string pipeline = GraphicsSettings.currentRenderPipeline.GetType().ToString();

                    if(pipeline.Contains("High") || pipeline.Contains("HD"))
                    {

                    }
                    else
                    {
                        ChangeShaderToOverlay(dn);
                    }
                }
                else
                {
                    ChangeShaderToOverlay(dn);
                }
            }

            if(dn.enable3DGame && !dn.renderThroughWalls)
            {
                TMP_Text tmp = dn.GetTextMesh();

                if (tmp.spriteAsset != null && tmp.spriteAsset.material != null)
                {
                    //Sprite Shader:
                    if (tmp.spriteAsset.material.shader.name != "TextMeshPro/Sprite Overlay")
                    {
                        Shader spriteOverlay = Shader.Find("TextMeshPro/Sprite Overlay");

                        if (spriteOverlay != null)
                        {
                            tmp.spriteAsset.material.shader = spriteOverlay;
                        }
                    }
                }
                else
                {
                    //Default Sprite Shader:
                    if (TMP_Settings.defaultSpriteAsset != null && TMP_Settings.defaultSpriteAsset.material != null && TMP_Settings.defaultSpriteAsset.material.shader.name != "TextMeshPro/Sprite Overlay")
                    {
                        Shader spriteOverlay = Shader.Find("TextMeshPro/Sprite Overlay");

                        if (spriteOverlay != null)
                        {
                            TMP_Settings.defaultSpriteAsset.material.shader = spriteOverlay;
                        }
                    }
                }
            }
        }
        TextSettings FixTextSettings(TextSettings ts)
        {
            ts.horizontal = Mathf.Max(0, ts.horizontal);

            return ts;
        }
        #endregion

        #region Warnings
        void WarningCheck(bool isMesh)
        {
            //3D Game and no 3D enabled:
            bool gameIs3DWarning = false;
            if(isMesh)
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    if (camera.orthographic == false)
                    {
                        foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                        {
                            if (!dn.enable3DGame)
                            {
                                gameIs3DWarning = true;
                                break;
                            }
                        }
                    }
                }
            }
            if(gameIs3DWarning)
            {
                EditorGUILayout.Space(2);
                DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                GUI.color = new Color(1, 1, 0.8f, 0.7f);
                DNPEditorInternal.ScalingLabel("Your main camera seems to be perspective <i>(3D)</i>.",298);
                DNPEditorInternal.ScalingLabel("Go to the <b>Main</b> tab and enable <b>3D Game</b>.",256);
                DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
            }

            //3D but UI Warning.
            if(isMesh == false)
            {
                foreach(DamageNumber dn in DNPEditorInternal.damageNumbers)
                {
                    if(dn.enable3DGame)
                    {
                        EditorGUILayout.Space(2);
                        DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                        GUI.color = new Color(1, 1, 0.8f, 0.7f);
                        DNPEditorInternal.ScalingLabel("You have <b>3D Game</b> enabled on the <b>GUI</b> version.", 298);
                        DNPEditorInternal.ScalingLabel("This component is for spawning prefabs in a <b>GUI</b> canvas.", 298);
                        DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
                    }
                }
            }

            //Pooling and Runtime Prefab Edits:
            bool runtimePoolingWarning = false;
            if (Application.isPlaying)
            {
                foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                {
                    if (dn.enablePooling)
                    {
                        runtimePoolingWarning = true;
                        break;
                    }
                }
            }
            if (runtimePoolingWarning)
            {
                EditorGUILayout.Space(2);
                DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                GUI.color = new Color(1, 1, 0.8f, 0.7f);
                DNPEditorInternal.ScalingLabel("You are editing this prefab at <b>runtime</b> with <b>pooling</b> enabled.",361);
                DNPEditorInternal.ScalingLabel("Prefab <b>changes</b> will not affect <b>pooled</b> damage numbers.",343);
                DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
            }

            //Scale By Number but no Numbers:
            bool scaleByNumbersWarning = false;
            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                if (dn.enableScaleByNumber && !dn.enableNumber)
                {
                    scaleByNumbersWarning = true;
                    break;
                }
            }
            if (scaleByNumbersWarning)
            {
                EditorGUILayout.Space(2);
                DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                GUI.color = new Color(1, 1, 0.8f, 0.7f);
                DNPEditorInternal.ScalingLabel("<b>Scale By Number</b> will not work if <b>Number</b> is disabled.",327);
                DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
            }

            //Color By Number but no Numbers:
            bool colorByNumbersWarning = false;
            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                if (dn.enableColorByNumber && !dn.enableNumber)
                {
                    colorByNumbersWarning = true;
                    break;
                }
            }
            if (colorByNumbersWarning)
            {
                EditorGUILayout.Space(2);
                DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                GUI.color = new Color(1, 1, 0.8f, 0.7f);
                DNPEditorInternal.ScalingLabel("<b>Color By Number</b> will not work if <b>Number</b> is disabled.", 327);
                DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
            }

            //Collision but no Lerp:
            bool collisionWarning = false;
            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                if (dn.enableCollision && !dn.enableLerp)
                {
                    collisionWarning = true;
                    break;
                }
            }
            if (collisionWarning)
            {
                EditorGUILayout.Space(2);
                DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                GUI.color = new Color(1, 1, 0.8f, 0.7f);
                DNPEditorInternal.ScalingLabel("<b>Collision</b> will not work if <b>Lerp</b> movement is disabled.",317);
                DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
            }

            //Push but no Lerp:
            bool pushWarning = false;
            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                if (dn.enablePush && !dn.enableLerp)
                {
                    pushWarning = true;
                    break;
                }
            }
            if (pushWarning)
            {
                EditorGUILayout.Space(2);
                DNPEditorInternal.StartBox(new Color(1, 1f, 0.8f));
                GUI.color = new Color(1, 1, 0.8f, 0.7f);
                DNPEditorInternal.ScalingLabel("<b>Push</b> will not work if <b>Lerp</b> movement is disabled.",298);
                DNPEditorInternal.CloseBox(new Color(1, 1f, 0.7f));
            }
        }
        #endregion

        #region Properties
        void DisplayFeature(string togglePropertyName, string featureTitle = "", string displayedTitle = "")
        {
            //Create title if needed.
            if (featureTitle == "")
            {
                featureTitle = togglePropertyName.Replace("enable", "");
            }
            if(displayedTitle == "")
            {
                displayedTitle = featureTitle;
            }

            EditorGUILayout.Space(2);

            //Get Toggle Property:
            SerializedProperty toggleProperty = serializedObject.FindProperty(togglePropertyName);

            //Start Box:
            DNPEditorInternal.StartBox(toggleProperty.hasMultipleDifferentValues || toggleProperty.boolValue);

            //Top:
            EditorGUILayout.BeginHorizontal();
            bool showProperties = FeatureButton(toggleProperty, displayedTitle);
            EditorGUILayout.LabelField("", GUILayout.MinWidth(0));

            //Extra Buttons:
            #region Text Position
            if(showProperties)
            {
                switch (togglePropertyName)
                {
                    case ("enableLeftText"):
                        int leftTextPosition = TextPosition(0);
                        if (leftTextPosition != 0)
                        {
                            EditorGUIUtility.keyboardControl = EditorGUIUtility.hotControl = 0; //Unselect Fields

                            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                            {
                                bool originalTextEnabled = dn.enableLeftText;
                                string originalTextString = dn.leftText;
                                TextSettings originalTextSettings = dn.leftTextSettings;

                                switch (leftTextPosition)
                                {
                                    case (1):
                                        dn.enableLeftText = dn.enableRightText;
                                        dn.leftText = dn.rightText;
                                        dn.leftTextSettings = dn.rightTextSettings;

                                        dn.enableRightText = originalTextEnabled;
                                        dn.rightText = originalTextString;
                                        dn.rightTextSettings = originalTextSettings;
                                        break;
                                    case (2):
                                        dn.enableLeftText = dn.enableTopText;
                                        dn.leftText = dn.topText;
                                        dn.leftTextSettings = dn.topTextSettings;

                                        dn.enableTopText = originalTextEnabled;
                                        dn.topText = originalTextString;
                                        dn.topTextSettings = originalTextSettings;
                                        break;
                                    case (3):
                                        dn.enableLeftText = dn.enableBottomText;
                                        dn.leftText = dn.bottomText;
                                        dn.leftTextSettings = dn.bottomTextSettings;

                                        dn.enableBottomText = originalTextEnabled;
                                        dn.bottomText = originalTextString;
                                        dn.bottomTextSettings = originalTextSettings;
                                        break;
                                }
                            }
                        }
                        break;
                    case ("enableRightText"):
                        int rightTextPosition = TextPosition(1);
                        if (rightTextPosition != 1)
                        {
                            EditorGUIUtility.keyboardControl = EditorGUIUtility.hotControl = 0; //Unselect Fields

                            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                            {
                                bool originalTextEnabled = dn.enableRightText;
                                string originalTextString = dn.rightText;
                                TextSettings originalTextSettings = dn.rightTextSettings;

                                switch (rightTextPosition)
                                {
                                    case (0):
                                        dn.enableRightText = dn.enableLeftText;
                                        dn.rightText = dn.leftText;
                                        dn.rightTextSettings = dn.leftTextSettings;

                                        dn.enableLeftText = originalTextEnabled;
                                        dn.leftText = originalTextString;
                                        dn.leftTextSettings = originalTextSettings;
                                        break;
                                    case (2):
                                        dn.enableRightText = dn.enableTopText;
                                        dn.rightText = dn.topText;
                                        dn.rightTextSettings = dn.topTextSettings;

                                        dn.enableTopText = originalTextEnabled;
                                        dn.topText = originalTextString;
                                        dn.topTextSettings = originalTextSettings;
                                        break;
                                    case (3):
                                        dn.enableRightText = dn.enableBottomText;
                                        dn.rightText = dn.bottomText;
                                        dn.rightTextSettings = dn.bottomTextSettings;

                                        dn.enableBottomText = originalTextEnabled;
                                        dn.bottomText = originalTextString;
                                        dn.bottomTextSettings = originalTextSettings;
                                        break;
                                }
                            }
                        }
                        break;
                    case ("enableTopText"):
                        int topTextPosition = TextPosition(2);
                        if (topTextPosition != 2)
                        {
                            EditorGUIUtility.keyboardControl = EditorGUIUtility.hotControl = 0; //Unselect Fields

                            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                            {
                                bool originalTextEnabled = dn.enableTopText;
                                string originalTextString = dn.topText;
                                TextSettings originalTextSettings = dn.topTextSettings;

                                switch (topTextPosition)
                                {
                                    case (0):
                                        dn.enableTopText = dn.enableLeftText;
                                        dn.topText = dn.leftText;
                                        dn.topTextSettings = dn.leftTextSettings;

                                        dn.enableLeftText = originalTextEnabled;
                                        dn.leftText = originalTextString;
                                        dn.leftTextSettings = originalTextSettings;
                                        break;
                                    case (1):
                                        dn.enableTopText = dn.enableRightText;
                                        dn.topText = dn.rightText;
                                        dn.topTextSettings = dn.rightTextSettings;

                                        dn.enableRightText = originalTextEnabled;
                                        dn.rightText = originalTextString;
                                        dn.rightTextSettings = originalTextSettings;
                                        break;
                                    case (3):
                                        dn.enableTopText = dn.enableBottomText;
                                        dn.topText = dn.bottomText;
                                        dn.topTextSettings = dn.bottomTextSettings;

                                        dn.enableBottomText = originalTextEnabled;
                                        dn.bottomText = originalTextString;
                                        dn.bottomTextSettings = originalTextSettings;
                                        break;
                                }
                            }
                        }
                        break;
                    case ("enableBottomText"):
                        int bottomTextPosition = TextPosition(3);
                        if (bottomTextPosition != 3)
                        {
                            EditorGUIUtility.keyboardControl = EditorGUIUtility.hotControl = 0; //Unselect Fields

                            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                            {
                                bool originalTextEnabled = dn.enableBottomText;
                                string originalTextString = dn.bottomText;
                                TextSettings originalTextSettings = dn.bottomTextSettings;

                                switch (bottomTextPosition)
                                {
                                    case (0):
                                        dn.enableBottomText = dn.enableLeftText;
                                        dn.bottomText = dn.leftText;
                                        dn.bottomTextSettings = dn.leftTextSettings;

                                        dn.enableLeftText = originalTextEnabled;
                                        dn.leftText = originalTextString;
                                        dn.leftTextSettings = originalTextSettings;
                                        break;
                                    case (1):
                                        dn.enableBottomText = dn.enableRightText;
                                        dn.bottomText = dn.rightText;
                                        dn.bottomTextSettings = dn.rightTextSettings;

                                        dn.enableRightText = originalTextEnabled;
                                        dn.rightText = originalTextString;
                                        dn.rightTextSettings = originalTextSettings;
                                        break;
                                    case (2):
                                        dn.enableBottomText = dn.enableTopText;
                                        dn.bottomText = dn.topText;
                                        dn.bottomTextSettings = dn.topTextSettings;

                                        dn.enableTopText = originalTextEnabled;
                                        dn.topText = originalTextString;
                                        dn.topTextSettings = originalTextSettings;
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            
            #endregion

            if (showProperties)
            {
                ResetButton(featureTitle);
            }
            bool showHints = DNPEditorInternal.HintButton(featureTitle);
            EditorGUILayout.EndHorizontal();

            //Hints:
            if(showHints)
            {
                DNPEditorInternal.Lines();
                GUI.color = new Color(1, 1, 1, 0.7f);
                switch(featureTitle)
                {
                    //Main:
                    case ("3D Game"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Handles <b>facing</b> the camera.");
                        DNPEditorInternal.Label("- Handles <b>screen size</b> over distance.");
                        DNPEditorInternal.Label("- Can handle rendering <b>through walls</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Enable this feature in <b>3D</b> projects.");
                        break;

                    //Text Content:
                    case ("Number"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Displays a <b>number</b>.");
                        DNPEditorInternal.Label("- Has several <b>text formatting</b> options.");
                        DNPEditorInternal.Label("- Number can be displayed as an <b>integer</b> or in <b>decimals</b>.");
                        DNPEditorInternal.Label("- Large numbers can be <b>dot separated</b> or <b>suffix shortened</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- If you want <b>text-only</b> simply disable this and enable <b>Left Text</b>.");
                        DNPEditorInternal.Label("- Text positions are <b>relative</b> to the number.");
                        DNPEditorInternal.Label("- Numbers will be added up if <b>Combination</b> is enabled.");
                        DNPEditorInternal.Label("- Numbers will be used by <b>Scale by Number</b>.");
                        break;
                    case ("Left Text"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Displays <b>text</b> on the left side.");
                        DNPEditorInternal.Label("- Has several <b>text formatting</b> options.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- If <b>number</b> is disabled this will be <b>centered</b>.");
                        DNPEditorInternal.Label("- You can use the <b>arrow</b> buttons to swap the position.");
                        DNPEditorInternal.Label("- The <b>Spawn(...)</b> function has overrides which set this text.");
                        break;
                    case ("Right Text"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Displays <b>text</b> on the right side.");
                        DNPEditorInternal.Label("- Has several <b>text formatting</b> options.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- If <b>number</b> is disabled this will be <b>centered</b>.");
                        DNPEditorInternal.Label("- You can use the <b>arrow</b> buttons to swap the position.");
                        break;
                    case ("Top Text"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Displays <b>text</b> on the top side.");
                        DNPEditorInternal.Label("- Has several <b>text formatting</b> options.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- You can use the <b>arrow</b> buttons to swap the position.");
                        break;
                    case ("Bottom Text"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Displays <b>text</b> on the bottom side.");
                        DNPEditorInternal.Label("- Has several <b>text formatting</b> options.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- You can use the <b>arrow</b> buttons to swap the position.");
                        break;
                    case ("Color By Number"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Changes the <b>color</b> based on the <b>number</b>.");
                        DNPEditorInternal.Label("- <b>Larger</b> and <b>smaller</b> numbers can have different <b>tints</b>.");
                        break;

                    //Movement:
                    case ("Lerp"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Moves towards a random <b>offset</b>.");
                        DNPEditorInternal.Label("- <b>Slows</b> down as it gets closer to it's target position.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- This is the most <b>popular</b> and <b>readable</b> option.");
                        DNPEditorInternal.Label("- Some features require this and warn you if <b>Lerp</b> is disabled.");
                        DNPEditorInternal.Label("- Offset is relative to <b>view</b> direction.");
                        break;
                    case ("Velocity"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Moves at a random <b>velocity</b>.");
                        DNPEditorInternal.Label("- Has <b>drag</b> and <b>gravity</b> options.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Movement is relative to <b>view</b> direction.");
                        break;
                    case ("Shaking"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Moves <b>back</b> and <b>forth</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Can be used for <b>shaking</b> or <b>vibration</b> effects.");
                        break;
                    case ("Following"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Follows the <b>target</b> transform.");
                        DNPEditorInternal.Label("- Maintains <b>relative position</b> to target.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Lower speeds are more <b>smooth</b>.");
                        DNPEditorInternal.Label("- Higher speeds are more <b>static</b>.");
                        DNPEditorInternal.Label("- Drag can be used to <b>fade out</b> the following.");
                        break;

                    //Fade In:
                    case ("Offset In"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Moves 2 meshs <b>together</b> from a <b>offset</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Creates a <b>fusion-like</b> effect.");
                        break;
                    case ("Scale In"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Fades in from a customizable <b>scale</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Is used for <b>shrinking</b> or <b>growing</b> when fading in.");
                        break;
                    case ("Cross Scale In"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- <b>Scales</b> one mesh and <b>divides</b> the scale of the other.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- <b>Inverse</b> scales one mesh and <b>normally</b> scales the other.");
                        break;
                    case ("Shake In"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- <b>Shakes</b> as it fades in.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Used for <b>vibration</b> or <b>motion</b> when fading in.");
                        break;

                    //Fade Out:
                    case ("Offset Out"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Moves 2 meshs <b>apart</b> to a <b>offset</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Creates a <b>diffusion-like</b> effect.");
                        break;
                    case ("Scale Out"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Fades out to a customizable <b>scale</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Is used for <b>shrinking</b> or <b>growing</b> when fading out.");
                        break;
                    case ("Cross Scale Out"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- <b>Scales</b> one mesh and <b>divides</b> the scale of the other.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- <b>Inverse</b> scales one mesh and <b>normally</b> scales the other.");
                        break;
                    case ("Shake Out"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- <b>Shakes</b> as it fades out.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Used for <b>vibration</b> or <b>motion</b> when fading out.");
                        break;

                    //Rotation & Scale:
                    case ("Start Rotation"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Spawns at a random <b>rotation</b>.");
                        break;
                    case ("Rotate Over Time"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Rotates at a random <b>rotation speed</b> over time.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Speed over time can be customized in a <b>curve</b>.");
                        break;
                    case ("Scale By Number"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Scales based on the <b>number's size</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Bigger numbers are <b>larger</b>.");
                        DNPEditorInternal.Label("- Will also work with <b>Combination</b>.");
                        break;
                    case ("Scale Over Time"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Changes scale over <b>time</b>.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Scale over time can be customized in a <b>curve</b>.");
                        break;

                    //Spam Control:
                    case ("Combination"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- <b>Combines</b> with other damage numbers within range.");
                        DNPEditorInternal.Label("- <b>Adds up</b> the combined numbers.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- <b>ABSORB_NEW</b> makes the oldest damage number absorb new ones.");
                        DNPEditorInternal.Label("- <b>REPLACE_OLD</b> makes the newest damage number absorb older ones.");
                        DNPEditorInternal.Label("- Animation can be customized using <b>curves</b>.");
                        DNPEditorInternal.Label("- Absorber can get a <b>scale up</b> upon absorbing a damage number.");
                        break;
                    case ("Destruction"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- <b>Destroys</b> older damage numbers.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Has a custom <b>fade out</b> animation for destroyed damage numbers.");
                        break;
                    case ("Collision"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Collides with other damage numbers to <b>spread</b> out.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- If desired direction is set to something like <b>(0,10,0)</b> they will spread up.");
                        break;
                    case ("Push"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Simply <b>moves</b> damage numbers by a custom offset within range.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- This is a more efficient version of <b>Collision</b>.");
                        DNPEditorInternal.Label("- Use this on <b>closely spawned</b> damage numbers only.");
                        break;

                    //Performance:
                    case ("Pooling"):
                        DNPEditorInternal.Label("<b>Function:</b>");
                        DNPEditorInternal.Label("- Improves <b>spawn performance</b> by recycling damage numbers.");
                        DNPEditorInternal.Label("");
                        DNPEditorInternal.Label("<b>Information:</b>");
                        DNPEditorInternal.Label("- Pool size should be <b>higher</b> if you spawn more damage numbers per second.");
                        DNPEditorInternal.Label("- New damage numbers are <b>added to the pool</b> if it's not full yet.");
                        DNPEditorInternal.Label("- Each prefab has it's <b>own</b> pool.");
                        DNPEditorInternal.Label("- Use <b>PrewarmPool()</b> on a prefab to prepare it's pool.");
                        break;
                }
                GUI.color = Color.white;
            }

            //Properties:
            if(showProperties)
            {
                DNPEditorInternal.Lines();

                switch (featureTitle)
                {
                    //Main:
                    case ("3D Game"):
                        SerializedProperty faceCameraView = serializedObject.FindProperty("faceCameraView");
                        EditorGUILayout.PropertyField(faceCameraView);
                        if(faceCameraView.boolValue || faceCameraView.hasMultipleDifferentValues)
                        {
                            SerializedProperty lookAtCamera = serializedObject.FindProperty("lookAtCamera");
                            EditorGUILayout.PropertyField(lookAtCamera, new GUIContent("Look At (For VR)"));

                            if(lookAtCamera.boolValue || lookAtCamera.hasMultipleDifferentValues)
                            {
                                GUI.color = new Color(1, 1, 1, 0.7f);
                                DNPEditorInternal.Label("- Costs <b>more performance</b> and should only be used in <b>VR</b>.");
                                GUI.color = Color.white;
                            }
                        }

                        DNPEditorInternal.Lines();

                        //Check Materials:
                        bool hasOverlayMaterials = false;
                        bool hasBadMaterials = false;
                        foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                        {
                            dn.GetReferencesIfNecessary();
                            Material[] materials = dn.GetSharedMaterials();

                            if(materials != null)
                            {
                                foreach (Material mat in materials)
                                {
                                    if(mat != null && mat.shader.name.EndsWith("verlay"))
                                    {
                                        hasOverlayMaterials = true;
                                    }
                                    else
                                    {
                                        hasBadMaterials = true;
                                    }
                                }
                            }
                        }

                        if(hasOverlayMaterials && !hasBadMaterials)
                        {
                            GUI.enabled = false;
                        }
                        SerializedProperty renderThroughWalls = serializedObject.FindProperty("renderThroughWalls");
                        EditorGUILayout.PropertyField(renderThroughWalls);
                        if (hasOverlayMaterials && !hasBadMaterials)
                        {
                            GUI.enabled = true;
                            renderThroughWalls.boolValue = false;
                            PropertyOverlay(DNPEditorInternal.CheckmarkString(true) + "  Overlay Shader.");
                        }else
                        {
                            if(DNPEditorInternal.currentWidth < 344)
                            {
                                if (DNPEditorInternal.currentWidth < 277)
                                {
                                    PropertyOverlay(DNPEditorInternal.CheckmarkString(false) + "  Shader");
                                }
                                else
                                {
                                    PropertyOverlay(DNPEditorInternal.CheckmarkString(false) + "  Overlay Shader");
                                }
                            }
                            else
                            {
                                PropertyOverlay(DNPEditorInternal.CheckmarkString(false) + "  Not using overlay shader.");
                            }
                        }

                        if(renderThroughWalls.boolValue || renderThroughWalls.hasMultipleDifferentValues)
                        {
                            GUI.color = new Color(1, 1, 1, 0.7f);
                            EditorGUILayout.Space();
                            DNPEditorInternal.ScalingLabel("This option exists in-case the overlay shader does <b>not work</b>.", 371);
                            DNPEditorInternal.ScalingLabel("Make sure to try the <b>'Distance Field Overlay'</b> shader first.",354);
                            DNPEditorInternal.ScalingLabel("It's better for <b>performance</b> to use the shader instead.",330);
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginHorizontal();
                            DNPEditorInternal.ScalingLabel("Go to the material or use the button.",333);
                            GUI.color = Color.white;

                            bool buttonPressed = false;
                            if(DNPEditorInternal.currentWidth < 283)
                            {
                                buttonPressed = GUILayout.Button("Try");
                            }
                            else
                            {
                                buttonPressed = GUILayout.Button("Try Shader");
                            }

                            if (buttonPressed)
                            {
                                foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                                {
                                    ChangeShaderToOverlay(dn);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        DNPEditorInternal.Lines();

                        SerializedProperty consistentScale = serializedObject.FindProperty("consistentScreenSize");
                        EditorGUILayout.PropertyField(consistentScale);
                        GUI.enabled = consistentScale.boolValue || consistentScale.hasMultipleDifferentValues;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceScalingSettings"));
                        EditorGUILayout.EndHorizontal();
                        GUI.enabled = true;
                        DNPEditorInternal.Lines();

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraOverride"));
                        bool noCameraOverride = true;
                        foreach(DamageNumber dn in DNPEditorInternal.damageNumbers)
                        {
                            if(dn.cameraOverride != null)
                            {
                                noCameraOverride = false;
                                break;
                            }
                        }
                        if(noCameraOverride)
                        {
                            string overlayString = "Main Camera";

                            if (DNPEditorInternal.currentWidth < 404)
                            {
                                overlayString = "<size=11>Main Camera</size>";

                                if(DNPEditorInternal.currentWidth < 389)
                                {
                                    overlayString = "";
                                }
                            }

                            if(DNPEditorInternal.currentWidth > 293)
                            {
                                PropertyOverlay(DNPEditorInternal.CheckmarkString(true) + "  " + overlayString + "      ");
                            }
                        }
                        else
                        {
                            GUI.color = new Color(1, 1, 1, 0.7f);
                            DNPEditorInternal.ScalingLabel("Only required if your <b>main camera</b> is not the target camera.", 362);
                            GUI.color = Color.white;
                        }
                        break;

                    //Text Content:
                    case ("Number"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("number"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("numberSettings"));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("digitSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Left Text"):
                        HandleTextProperty(serializedObject.FindProperty("leftText"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftTextSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Right Text"):
                        HandleTextProperty(serializedObject.FindProperty("rightText"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightTextSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Top Text"):
                        HandleTextProperty(serializedObject.FindProperty("topText"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("topTextSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Bottom Text"):
                        HandleTextProperty(serializedObject.FindProperty("bottomText"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomTextSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Color By Number"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorByNumberSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;

                    //Movement:
                    case ("Lerp"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("lerpSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Velocity"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("velocitySettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Shaking"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shakeSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Following"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("followedTarget"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("followSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;

                    //Fade In:
                    case ("Offset In"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetFadeIn"), new GUIContent("Offset"));
                        break;
                    case ("Scale In"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleFadeIn"), new GUIContent("Scale"));
                        break;
                    case ("Cross Scale In"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("crossScaleFadeIn"), new GUIContent("Cross Scale"));
                        break;
                    case ("Shake In"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shakeOffsetFadeIn"), new GUIContent("Shake Offset"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shakeFrequencyFadeIn"), new GUIContent("Shake Frequency"));
                        break;


                    //Fade Out:
                    case ("Offset Out"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetFadeOut"), new GUIContent("Offset"));
                        break;
                    case ("Scale Out"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleFadeOut"), new GUIContent("Scale"));
                        break;
                    case ("Cross Scale Out"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("crossScaleFadeOut"), new GUIContent("Cross Scale"));
                        break;
                    case ("Shake Out"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shakeOffsetFadeOut"), new GUIContent("Shake Offset"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shakeFrequencyFadeOut"), new GUIContent("Shake Frequency"));
                        break;

                    //Rotation & Scale:
                    case ("Start Rotation"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("minRotation"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationRandomFlip"), new GUIContent("Random Flip", "Randomly flips the Rotation.\nUseful for avoiding small rotations.\nSet Min and Max to a positive value."));
                        break;
                    case ("Rotate Over Time"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("minRotationSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotationSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeedRandomFlip"), new GUIContent("Random Flip", "Randomly flips the Rotation Speed.\nUseful for avoiding small rotation speeds.\nSet Min and Max to a positive value."));

                        SerializedProperty rotateOverTimeProperty = serializedObject.FindProperty("rotateOverTime");
                        Color rotateOverTimeColor = Color.Lerp(Color.red, Color.green, rotateOverTimeProperty.animationCurveValue.Evaluate(1));
                        EditorGUILayout.CurveField(rotateOverTimeProperty, rotateOverTimeColor, new Rect(0, 0, 1, 1));
                        break;
                    case ("Scale By Number"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleByNumberSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Scale Over Time"):
                        SerializedProperty scaleOverTimeProperty = serializedObject.FindProperty("scaleOverTime");
                        Color scaleOverTimeColor = Color.Lerp(Color.red, Color.green, scaleOverTimeProperty.animationCurveValue.Evaluate(1));
                        EditorGUILayout.CurveField(scaleOverTimeProperty, scaleOverTimeColor, new Rect(0, 0, 1, 5f));
                        break;
                    //Spam Control:
                    case ("Combination"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("  ", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("combinationSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Destruction"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("  ", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("destructionSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Collision"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("  ", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ("Push"):
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("  ", GUILayout.Width(9));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pushSettings"));
                        EditorGUILayout.EndHorizontal();
                        break;

                    //Performance:
                    case ("Pooling"):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("poolSize"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableOnSceneLoad"));
                        break;
                }
            }

            //Close Box:
            DNPEditorInternal.CloseBox(showProperties);
        }

        void HandleTextProperty(SerializedProperty textField)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(textField);

            if (textField.stringValue.Length > 5)
            {
                char[] chars = textField.stringValue.ToCharArray();
                bool unicodeCheck = false;
                int unicodeIndex = 0;
                for (int i = 0; i < chars.Length; i++)
                {
                    char c = chars[i];

                    if (c == '\\')
                    {
                        //Check if there is a unicode sequence.
                        unicodeCheck = true;
                        unicodeIndex = i;
                    }
                    else
                    {
                        if (unicodeCheck)
                        {
                            if (i == unicodeIndex + 1)
                            {
                                if (char.ToLower(c) != 'u')
                                {
                                    unicodeCheck = false;
                                }
                            }
                            else if (i < unicodeIndex + 6)
                            {
                                int cInt = (int)c;
                                if (!char.IsNumber(c) && !(cInt > 96 && cInt < 103))
                                {
                                    unicodeCheck = false;
                                }
                            }
                        }
                    }
                }

                if(unicodeCheck && unicodeIndex < chars.Length - 5)
                {
                    if(GUILayout.Button("Unicode", GUILayout.Width(60)))
                    {
                        //Get hex code.
                        string unicodeHex = "";
                        for (int i = unicodeIndex + 2; i < unicodeIndex + 6; i++)
                        {
                            unicodeHex += chars[i];
                        }
                        int hexCode = int.Parse(unicodeHex, System.Globalization.NumberStyles.HexNumber);

                        //Get unicode.
                        string unicode = char.ConvertFromUtf32(hexCode);

                        //Combine.
                        string newString = "";
                        for(int i = 0; i < unicodeIndex; i++)
                        {
                            newString += chars[i];
                        }
                        newString += unicode;
                        for (int i = unicodeIndex + 6; i < chars.Length; i++)
                        {
                            newString += chars[i];
                        }

                        //Assign.
                        textField.stringValue = newString;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void ChangeShaderToOverlay(DamageNumber damageNumber)
        {
            damageNumber.GetReferencesIfNecessary();
            Material[] materials = damageNumber.GetSharedMaterials();

            if (materials != null)
            {
                foreach (Material mat in materials)
                {
                    if (!mat.shader.name.EndsWith("verlay"))
                    {
                        Shader shader = Shader.Find(mat.shader.name + " Overlay");
                        if (shader == null)
                        {
                            shader = Shader.Find("TextMeshPro/Distance Field - Overlay");
                        }
                        if (shader == null)
                        {
                            shader = Shader.Find("TextMeshPro/Distance Field (Overlay)");
                        }
                        if (shader == null)
                        {
                            shader = Shader.Find("TextMeshPro/Distance Field Overlay");
                        }

                        mat.shader = shader;
                        damageNumber.renderThroughWalls = false;
                    }
                }
            }
        }

        bool FeatureButton(SerializedProperty property, string featureTitle)
        {
            //Get name for toggle button.
            string buttonName = property.hasMultipleDifferentValues ? "<b>− − −</b>" : (property.boolValue ? "<b> " + featureTitle + "</b>" : featureTitle);

            bool toggled = GUILayout.Button(buttonName, DNPEditorInternal.buttonStyle, GUILayout.Width(140));

            if (toggled)
            {
                //Record changes.
                List<Object> objectsList = new List<Object>();
                foreach(Object targetObject in targets)
                {
                    objectsList.Add(targetObject);
                }
                foreach (DamageNumber damageNumber in DNPEditorInternal.damageNumbers)
                {
                    if(damageNumber != null)
                    {
                        damageNumber.GetReferencesIfNecessary();
                        foreach (TMP_Text tmpText in damageNumber.GetTextMeshs())
                        {
                            if(tmpText != null)
                            {
                                objectsList.Add(tmpText);
                            }
                        }
                    }
                }
                Object[] objectsArray = new Object[objectsList.Count];
                for(int i = 0; i < objectsList.Count; i++)
                {
                    objectsArray[i] = objectsList[i];
                }
                Undo.RecordObjects(objectsArray, "Toggled " + featureTitle);

                //Toggle feature.
                property.boolValue = !property.boolValue;

                //Avoid Conflicts:
                if(property.boolValue)
                {
                    switch (property.name)
                    {
                        case ("enableDestruction"):
                            serializedObject.FindProperty("enableCombination").boolValue = false;
                            break;
                        case ("enableCombination"):
                            serializedObject.FindProperty("enableDestruction").boolValue = false;
                            break;
                    }
                }
            }

            if (property.boolValue || property.hasMultipleDifferentValues)
            {
                GUI.color = new Color(1, 1, 1, 0.8f);
                GUI.Toolbar(GUILayoutUtility.GetLastRect(), 0, new string[] { buttonName }, DNPEditorInternal.buttonStyle);
                GUI.color = new Color(1, 1, 1, 1f);

                return true;
            }

            return false;
        }
        void DisplayTextMain(bool isMesh)
        {
            EditorGUILayout.Space(2);
            DNPEditorInternal.StartBox();

            bool editingPrefabPreview = DNPEditorInternal.EditingPrefabPreview(target);
            if (editingPrefabPreview)
            {
                GUI.enabled = false;
            }

            //Font:
            bool mixedFontAssets = false;
            TMP_FontAsset fontAsset = null;

            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                dn.GetReferencesIfNecessary();

                if(isMesh)
                {
                    TMP_Text tmp = dn.GetTextMesh();

                    if (fontAsset != null && tmp.font != null && tmp.font != fontAsset)
                    {
                        mixedFontAssets = true;
                    }
                    else
                    {
                        fontAsset = tmp.font;
                    }
                }
                else
                {
                    foreach (TMP_Text tmp in dn.GetTextMeshs())
                    {
                        if (fontAsset != null && tmp.font != null && tmp.font != fontAsset)
                        {
                            mixedFontAssets = true;
                        }
                        else
                        {
                            fontAsset = tmp.font;
                        }
                    }
                }
            }

            EditorGUI.showMixedValue = mixedFontAssets;
            EditorGUI.BeginChangeCheck();
            TMP_FontAsset newFontAsset = (TMP_FontAsset) EditorGUILayout.ObjectField(new GUIContent("Font", "The font used by text mesh pro."), fontAsset, typeof(TMP_FontAsset), false);
            if(EditorGUI.EndChangeCheck())
            {
                foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                {
                    Undo.RecordObjects(dn.GetTextMeshs(), "Changed font.");

                    foreach (TMP_Text tmp in dn.GetTextMeshs())
                    {
                        tmp.font = newFontAsset;
                    }
                }

                DNPEditorInternal.ResetMaterials();
            }
            EditorGUI.showMixedValue = false;

            //Color:
            bool mixedColor = false;
            Color vertexColor = Color.white;
            bool firstColor = true;
            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                foreach (TMP_Text tmp in dn.GetTextMeshs())
                {
                    if (firstColor)
                    {
                        firstColor = false;
                        vertexColor = tmp.color;
                    }
                    else
                    {
                        if (vertexColor != tmp.color)
                        {
                            mixedColor = true;
                        }
                    }
                }
            }

            EditorGUI.showMixedValue = mixedColor;
            EditorGUI.BeginChangeCheck();
            Color newColor = EditorGUILayout.ColorField(new GUIContent("Color", "The vertex color used by text mesh pro."), vertexColor);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
                {
                    Undo.RecordObjects(dn.GetTextMeshs(), "Changed color.");

                    foreach (TMP_Text tmp in dn.GetTextMeshs())
                    {
                        tmp.color = newColor;
                    }
                }
            }

            GUI.enabled = true;

            //Info:
            DNPEditorInternal.Lines();
            GUI.color = new Color(1, 1, 1, 0.7f);
            if (editingPrefabPreview)
            {
                DNPEditorInternal.ScalingLabel("You need to <b>open</b> the <b>prefab</b> to access these settings.", 355);
                DNPEditorInternal.OpenPrefabButton(target);
            }
            else
            {
                DNPEditorInternal.ScalingLabel("Check out the <b>TextMeshPro</b> component for more settings.", 355);
                GUI.color = Color.white;
            }

            DNPEditorInternal.CloseBox();
        }
        void DisplayMainSettings()
        {
            EditorGUILayout.Space(2);
            DNPEditorInternal.StartBox();

            SerializedProperty permanentProperty = serializedObject.FindProperty("permanent");
            GUI.enabled = !permanentProperty.boolValue || permanentProperty.hasMultipleDifferentValues;

            EditorGUILayout.BeginHorizontal();

            SerializedProperty lifetimeProperty = serializedObject.FindProperty("lifetime");
            EditorGUILayout.PropertyField(lifetimeProperty);

            bool notEnoughLifetime = false;
            foreach(DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                if(dn.lifetime < dn.durationFadeIn)
                {
                    notEnoughLifetime = true;
                    break;
                }
            }
            if (notEnoughLifetime)
            {
                PropertyOverlay("<color=#FF0000BB>Not enough time to fade in.</color>");
            }
            else
            {
                TimePropertyOverlay(lifetimeProperty.floatValue);
            }

            ResetButton("Lifetime");

            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            SerializedProperty timescaleProperty = serializedObject.FindProperty("unscaledTime");
            EditorGUILayout.PropertyField(timescaleProperty);
            ResetButton("UnscaledTime");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(permanentProperty);
            ResetButton("Permanent");
            EditorGUILayout.EndHorizontal();

            //Information:

            DNPEditorInternal.Lines();
            GUI.color = new Color(1, 1, 1, 0.7f);
            if (permanentProperty.boolValue)
            {
                DNPEditorInternal.Label("- Will not <b>fade out</b> on it's own.");
            }
            else
            {
                DNPEditorInternal.Label("- Will <b>fade out</b> after <b>" + (Mathf.Round(lifetimeProperty.floatValue * 10) * 0.1f) + "</b> seconds.");
            }
            GUI.color = Color.white;

            DNPEditorInternal.CloseBox();
        }
        void DisplayFadeMain(string inOrOut)
        {
            EditorGUILayout.Space(2);
            DNPEditorInternal.StartBox();

            EditorGUILayout.BeginHorizontal();

            SerializedProperty serializedProperty = serializedObject.FindProperty("durationFade" + inOrOut);
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent("Duration", "The duration it takes to fade " + inOrOut.ToLower() + "."));
            TimePropertyOverlay(serializedProperty.floatValue);
            ResetButton("Fade " + inOrOut + " Main");

            EditorGUILayout.EndHorizontal();

            DNPEditorInternal.CloseBox();
        }
        void DisplaySpamControlMain(bool isMesh)
        {
            EditorGUILayout.Space(2);

            //Check if Spam Group is required.
            bool requiresSpamGroup = false;
            bool hasSpamGroup = false;
            SerializedProperty serializedProperty = serializedObject.FindProperty("spamGroup");
            SerializedProperty combinationProperty = serializedObject.FindProperty("enableCombination");
            SerializedProperty destructionProperty = serializedObject.FindProperty("enableDestruction");
            SerializedProperty collisionProperty = serializedObject.FindProperty("enableCollision");
            SerializedProperty pushProperty = serializedObject.FindProperty("enablePush");
            if (combinationProperty.boolValue || combinationProperty.hasMultipleDifferentValues)
            {
                requiresSpamGroup = true;
            }
            if (destructionProperty.boolValue || destructionProperty.hasMultipleDifferentValues)
            {
                requiresSpamGroup = true;
            }
            if (collisionProperty.boolValue || collisionProperty.hasMultipleDifferentValues)
            {
                requiresSpamGroup = true;
            }
            if (pushProperty.boolValue || pushProperty.hasMultipleDifferentValues)
            {
                requiresSpamGroup = true;
            }
            if (requiresSpamGroup && serializedProperty.stringValue.Replace(" ", "") != "")
            {
                requiresSpamGroup = false;
                hasSpamGroup = true;
            }

            DNPEditorInternal.StartBox(requiresSpamGroup ? new Color(1, 1f, 0.8f) : Color.white);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(serializedProperty);

            if(requiresSpamGroup)
            {
                PropertyOverlay("<color=#FF0000BB>Required</color>");
            }

            ResetButton("Spam Control Main");

            EditorGUILayout.EndHorizontal();

            if (requiresSpamGroup)
            {
                DNPEditorInternal.Lines();
                GUI.color = new Color(1, 1, 1, 0.7f);
                DNPEditorInternal.ScalingLabel("This field is <b>required</b> by the features below.",272);
            }
            else if(hasSpamGroup)
            {
                DNPEditorInternal.Lines();
                GUI.color = new Color(1, 1, 1, 0.7f);
                if(isMesh)
                {
                    DNPEditorInternal.ScalingLabel("Damage numbers within the same <b>group</b> interact with each other.", 395);
                }
                else
                {
                    DNPEditorInternal.ScalingLabel("Damage numbers with the same <b>group</b> & <b>parent</b> interact with each other.", 440);
                }
            }

            DNPEditorInternal.CloseBox(requiresSpamGroup ? new Color(1, 1f, 0.7f) : Color.white);
        }
        int TextPosition(int current)
        {
            //Change Position:
            EditorGUILayout.BeginHorizontal();


            GUI.color = new Color(1, 1, 1, 0.7f);
            GUI.color = Color.white;

            int swapWith = GUILayout.Toolbar(current, new GUIContent[] { new GUIContent("←", "Move this text to the left."), new GUIContent("→", "Move this text to the right."), new GUIContent("↑", "Move this text to the top."), new GUIContent("↓", "Move this text to the bottom.") }, GUILayout.Width(DNPEditorInternal.currentWidth > 330 ? 90 : 0));

            GUI.color = new Color(1, 1, 1, 0.7f);
            GUILayout.Label(" | ", DNPEditorInternal.centerTextStyle, GUILayout.Width(10), GUILayout.Height(18));
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            return swapWith;
        }
        void DisplayPerformanceMain()
        {
            EditorGUILayout.Space(4);

            //Delays:
            DNPEditorInternal.StartBox();

            EditorGUILayout.BeginHorizontal();
            DisplayDelay("updateDelay", "Update Delay", "The delay between updates.\nGreatly affects performance.");
            ResetButton("Update Delay");
            EditorGUILayout.EndHorizontal();

            DNPEditorInternal.CloseBox();
        }
        void DisplayPerformanceHints()
        {
            EditorGUILayout.Space(4);

            DNPEditorInternal.StartBox();

            GUI.color = new Color(1, 1, 1, 0.7f);

            DNPEditorInternal.Label("<b><size=13>Performance Hints:</size></b>");

            EditorGUILayout.Space(5);

            DNPEditorInternal.ScalingLabel("Always make sure to enable <b>Pooling</b>.", 1);
            DNPEditorInternal.ScalingLabel("You can also decrease the <b>Update FPS</b>.", 261);
            DNPEditorInternal.ScalingLabel("Enabling <b>Combination</b> and <b>Destruction</b> also boosts performance.", 398);
            DNPEditorInternal.ScalingLabel("You can find those features in the <b>Spam Control</b> section.", 350);

            GUI.color = Color.white;

            DNPEditorInternal.CloseBox();
        }
        void DisplayMovementHints(bool isMesh)
        {
            if(!isMesh)
            {
                EditorGUILayout.Space(4);

                DNPEditorInternal.StartBox();

                GUI.color = new Color(1, 1, 1, 0.7f);

                DNPEditorInternal.Label("<b><size=13>GUI Hints:</size></b>");

                EditorGUILayout.Space(5);

                DNPEditorInternal.ScalingLabel("The <b>GUI</b> version handles movement in <b>anchored</b> position.", 363);
                DNPEditorInternal.ScalingLabel("An <b>offset</b> or <b>velocity</b> of <b>1</b> equals <b>100</b> in <b>anchored</b> space.",352f);
                DNPEditorInternal.ScalingLabel("This <b>x100</b> conversion is for keeping values <b>small</b> and <b>consistent</b>.",399f);

                GUI.color = Color.white;

                DNPEditorInternal.CloseBox();
            }
        }
        void DisplaySpamControlHints(bool isMesh)
        {
            if (!isMesh)
            {
                EditorGUILayout.Space(4);

                DNPEditorInternal.StartBox();

                GUI.color = new Color(1, 1, 1, 0.7f);

                DNPEditorInternal.Label("<b><size=13>GUI Hints:</size></b>");

                EditorGUILayout.Space(5);

                DNPEditorInternal.ScalingLabel("The <b>GUI</b> version handles everything in <b>anchored</b> position.", 363);
                DNPEditorInternal.ScalingLabel("A <b>distance</b> or <b>radius</b> of <b>1</b> equals <b>100</b> in <b>anchored</b> space.", 358f);
                DNPEditorInternal.ScalingLabel("This <b>x100</b> conversion is for keeping values <b>small</b> and <b>consistent</b>.", 399f);

                GUI.color = Color.white;

                DNPEditorInternal.CloseBox();
            }
        }
        void DisplayDelay(string propertyName, string displayName, string displayTooltip)
        {
            //Property:
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            //Delay:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property, new GUIContent(displayName, displayTooltip));

            //Delay Overlay:
            string overlayContent;
            if (property.hasMultipleDifferentValues)
            {
                overlayContent = "";
            }
            else
            {
                if (property.floatValue > 0)
                {
                    if(DNPEditorInternal.currentWidth < 400f)
                    {
                        overlayContent = "s";
                    }
                    else
                    {
                        overlayContent = "Delay";
                    }
                }
                else
                {
                    overlayContent = "No Delay";
                }
            }
            PropertyOverlay(overlayContent);

            GUI.color = new Color(1, 1, 1, 0.5f);
            EditorGUILayout.LabelField("➜", DNPEditorInternal.centerTextStyle, GUILayout.Width(DNPEditorInternal.currentWidth > 334 ? 15 : 0));

            GUI.color = Color.white;

            if (property.hasMultipleDifferentValues)
            {
                GUI.enabled = false;
                GUILayout.TextField(" - - - ", GUILayout.Width(100));
                GUI.enabled = true;
            }
            else
            {
                if(property.floatValue > 0.10001f)
                {
                    GUI.color = new Color(1f, 1f, 0.7f, 1f);
                }

                int currentFPS = property.floatValue > 0 ? Mathf.RoundToInt(1f / property.floatValue) : 0;


                int fpsWidth = 90;
                if(DNPEditorInternal.currentWidth < 400)
                {
                    fpsWidth = (int) Mathf.Clamp(DNPEditorInternal.currentWidth - 300,50,90);
                }

                int newFPS = EditorGUILayout.IntField(currentFPS, GUILayout.Width(DNPEditorInternal.currentWidth > 294 ? fpsWidth : 0));

                if (currentFPS != newFPS)
                {
                    if (newFPS > 0)
                    {
                        property.floatValue = Mathf.Round(100000f / (float)newFPS) * 0.00001f;
                    }
                    else if (newFPS <= 0)
                    {
                        property.floatValue = 0f;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //FPS Overlay:
            GUI.color = new Color(1, 1, 1, 0.7f);
            if(property.hasMultipleDifferentValues)
            {
                overlayContent = "";
            }
            else
            {
                if(property.floatValue > 0)
                {
                    overlayContent = "FPS";
                }
                else
                {
                    if(DNPEditorInternal.currentWidth < 400)
                    {
                        overlayContent = "";
                    }
                    else
                    {
                        overlayContent = "Every Frame";
                    }
                }
            }
            PropertyOverlay(DNPEditorInternal.currentWidth > 310 ? overlayContent : "");
        }
        void ResetButton(string category)
        {
            GUIContent resetButton = new GUIContent();
            resetButton.text = "R";
            resetButton.tooltip = "Reset this feature.";

            if (GUILayout.Button(resetButton, GUILayout.Width(21)))
            {
                EditorGUIUtility.keyboardControl = EditorGUIUtility.hotControl = 0;
                ResetCategory(category);
            }
        }
        void ResetCategory(string category)
        {
            Undo.RecordObject(target, "Reset " + category + " settings.");

            foreach (DamageNumber dn in DNPEditorInternal.damageNumbers)
            {
                //Reset Feature:
                switch (category)
                {
                    //Main:
                    case ("Lifetime"):
                        dn.lifetime = 2f;
                        break;
                    case ("Permanent"):
                        dn.permanent = false;
                        break;
                    case ("UnscaledTime"):
                        dn.unscaledTime = false;
                        break;
                    case ("3D Game"):
                        dn.faceCameraView = true;
                        dn.renderThroughWalls = true;
                        dn.consistentScreenSize = false;
                        dn.distanceScalingSettings = new DistanceScalingSettings(0);
                        dn.cameraOverride = null;
                        break;

                    //Text Content:
                    case ("Number"):
                        dn.number = 1;
                        dn.numberSettings = new TextSettings(0);
                        dn.digitSettings = new DigitSettings(0);
                        break;
                    case ("Left Text"):
                        dn.leftText = "";
                        dn.leftTextSettings = new TextSettings(0f);
                        break;
                    case ("Right Text"):
                        dn.rightText = "";
                        dn.rightTextSettings = new TextSettings(0f);
                        break;
                    case ("Top Text"):
                        dn.topText = "";
                        dn.topTextSettings = new TextSettings(0f);
                        break;
                    case ("Bottom Text"):
                        dn.bottomText = "";
                        dn.bottomTextSettings = new TextSettings(0f);
                        break;
                    case ("Color By Number"):
                        dn.colorByNumberSettings = new ColorByNumberSettings(0f);
                        break;

                    //Movement:
                    case ("Lerp"):
                        dn.lerpSettings = new LerpSettings(0);
                        break;
                    case ("Velocity"):
                        dn.velocitySettings = new VelocitySettings(0);
                        break;
                    case ("Shaking"):
                        dn.shakeSettings = new ShakeSettings(new Vector2(0.005f, 0.005f));
                        break;
                    case ("Following"):
                        dn.followedTarget = null;
                        dn.followSettings = new FollowSettings(0);
                        break;

                    //Fade In:
                    case ("Fade In Main"):
                        dn.durationFadeIn = 0.2f;
                        break;
                    case ("Offset In"):
                        dn.offsetFadeIn = new Vector2(0.5f, 0);
                        break;
                    case ("Scale In"):
                        dn.scaleFadeIn = new Vector2(2f, 2f);
                        break;
                    case ("Shake In"):
                        dn.shakeOffsetFadeIn = new Vector2(0f, 1.5f);
                        dn.shakeFrequencyFadeIn = 4f;
                        break;
                    case ("Cross Scale In"):
                        dn.crossScaleFadeIn = new Vector2(1f, 1.5f);
                        break;

                    //Fade Out:
                    case ("Fade Out Main"):
                        dn.durationFadeOut = 0.2f;
                        break;
                    case ("Offset Out"):
                        dn.enableOffsetFadeOut = true;
                        dn.offsetFadeOut = new Vector2(0.5f, 0);
                        break;
                    case ("Scale Out"):
                        dn.scaleFadeOut = new Vector2(2f, 2f);
                        break;
                    case ("Shake Out"):
                        dn.shakeOffsetFadeOut = new Vector2(0f, 1.5f);
                        dn.shakeFrequencyFadeOut = 4f;
                        break;
                    case ("Cross Scale Out"):
                        dn.crossScaleFadeOut = new Vector2(1f, 1.5f);
                        break;

                    //Rotation & Scale:
                    case ("Start Rotation"):
                        dn.minRotation = -4;
                        dn.maxRotation = 4;
                        break;
                    case ("Rotate Over Time"):
                        dn.minRotationSpeed = -15f;
                        dn.maxRotationSpeed = 15;
                        dn.rotateOverTime = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(0.4f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0) });
                        break;
                    case ("Scale By Number"):
                        dn.scaleByNumberSettings = new ScaleByNumberSettings(0);
                        break;
                    case ("Scale Over Time"):
                        dn.scaleOverTime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0.7f));
                        break;

                    //Spam Control:
                    case ("Spam Control Main"):
                        dn.spamGroup = "";
                        break;
                    case ("Combination"):
                        dn.combinationSettings = new CombinationSettings(0);
                        break;
                    case ("Destruction"):
                        dn.destructionSettings = new DestructionSettings(0);
                        break;
                    case ("Collision"):
                        dn.collisionSettings = new CollisionSettings(0);
                        break;
                    case ("Push"):
                        dn.pushSettings = new PushSettings(0);
                        break;

                    //Performance:
                    case ("Pooling"):
                        dn.poolSize = 50;
                        break;
                    case ("Update Delay"):
                        dn.updateDelay = 0.0125f;
                        break;
                }
            }
        }
        void TimePropertyOverlay(float seconds, string zeroString = "Instantly")
        {
            if(seconds > 0)
            {
                PropertyOverlay("Seconds");
            }
            else
            {
                PropertyOverlay(zeroString);
            }
        }
        void PropertyOverlay(string text)
        {
            GUI.color = new Color(1, 1, 1, 0.7f);
            GUI.Label(GUILayoutUtility.GetLastRect(), text + " ", DNPEditorInternal.rightTextStyle);
            GUI.color = Color.white;
        }
        #endregion
    }
}

#endif