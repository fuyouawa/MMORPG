#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using DamageNumbersPro;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;

namespace DamageNumbersPro.Internal {
    public static class DNPEditorInternal
    {
        //Public:
        public static int currentTab;
        public static bool repaintViews;
        public static HashSet<string> hints;
        public static DamageNumber[] damageNumbers;
        public static TextMeshPro[] textMeshPros;

        //Private:
        static Transform[] meshAs;
        static Transform[] meshBs;

        //Presets:
        static Dictionary<string,DNPPreset[]> allPresets;

        //GUI Resources:
        public static GUIStyle labelStyle;
        public static GUIStyle buttonStyle;
        public static GUIStyle rightTextStyle;
        public static GUIStyle centerTextStyle;
        public static GUIStyle bottomRightTextStyle;
        public static GUIStyle topRightTextStyle;
        static GUIStyle whiteBoxStyle;
        static Texture2D whiteBoxTexture;
        static Texture bannerTexture;
        public static float currentWidth;

        //External Editor:
        static int currentEditor;
        static bool cleanEditor;
        static Material[] currentMaterials;
        static MaterialEditor materialEditor;
        static Editor textMeshProEditor;
        static bool generatedGUIStyles;

        public static void PrepareInspector(DamageNumberEditor damageNumberEditor)
        {
            //Clean Editors:
            cleanEditor = true;
            generatedGUIStyles = false;

            //Get Damage Numbers:
            damageNumbers = new DamageNumber[damageNumberEditor.targets.Length];
            for (int i = 0; i < damageNumberEditor.targets.Length; i++)
            {
                damageNumbers[i] = (DamageNumber)damageNumberEditor.targets[i];
            }

            //Type:
            bool isMesh = damageNumbers[0].IsMesh();

            //Get Presets:
            allPresets = new Dictionary<string, DNPPreset[]>();
            allPresets.Add("Style", Resources.LoadAll<DNPPreset>("DNP/Style"));
            allPresets.Add("Fade In", Resources.LoadAll<DNPPreset>("DNP/Fade In"));
            allPresets.Add("Fade Out", Resources.LoadAll<DNPPreset>("DNP/Fade Out"));
            allPresets.Add("Behaviour", Resources.LoadAll<DNPPreset>("DNP/Behaviour"));

            //Get Structural Objects:
            textMeshPros = new TextMeshPro[damageNumbers.Length];
            meshAs = new Transform[damageNumbers.Length];
            meshBs = new Transform[damageNumbers.Length];
            if(isMesh)
            {
                for (int n = 0; n < damageNumbers.Length; n++)
                {
                    Transform dnTransform = damageNumbers[n].transform;

                    //TMP:
                    Transform textMeshProTransform = dnTransform.Find("TMP");
                    if (textMeshProTransform != null)
                    {
                        textMeshPros[n] = textMeshProTransform.GetComponent<TextMeshPro>();
                    }

                    //MeshA:
                    meshAs[n] = dnTransform.Find("MeshA");

                    //MeshB:
                    meshBs[n] = dnTransform.Find("MeshB");
                }
            }

            //Get Banner Texture:
            if (damageNumbers != null && damageNumbers.Length > 0 && damageNumbers[0] != null)
            {
                bannerTexture = Resources.Load<Texture>("DNP/Textures/DNP_Banner");
            }

            //Create White Box Texture:
            Color[] pixels = new Color[4];
            for (int n = 0; n < pixels.Length; n++)
            {
                pixels[n] = Color.white;
            }
            whiteBoxTexture = new Texture2D(2, 2);
            whiteBoxTexture.SetPixels(pixels);
            whiteBoxTexture.Apply();

            //Close Hints:
            hints = new HashSet<string>();
        }

        public static void OnInspectorGUI(DamageNumberEditor damageNumberEditor)
        {
            //Prepare Inspector:
            if (damageNumbers == null || damageNumbers.Length == 0 || damageNumbers[0] == null)
            {
                PrepareInspector(damageNumberEditor);
            }

            //Prepare Styles:
            PrepareStyles();

            //Repaint:
            if (repaintViews)
            {
                repaintViews = false;
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }

        #region Inspector Top
        public static void DrawInspectorTop(bool isMesh)
        {
            //Banner:
            Rect bannerRect = DrawBanner(isMesh);

            //Tabs:
            DrawTabs(bannerRect);

            //Distance:
            EditorGUILayout.Space(42);
        }
        static Rect DrawBanner(bool isMesh)
        {
            Rect bannerRect = default;
            if (bannerTexture != null)
            {
                //Banner:
                EditorGUILayout.BeginVertical();
                GUILayout.Label("", GUILayout.Height(100 + 0.38f * Mathf.Min(0,EditorGUIUtility.currentViewWidth - 430)));
                bannerRect = GUILayoutUtility.GetLastRect();
                float ratio = (bannerRect.width / bannerRect.height) / 8f;
                Rect clipRect = new Rect((1 - ratio) * 0.5f, 0, ratio, 1);
                GUI.DrawTextureWithTexCoords(bannerRect, bannerTexture, clipRect, true);
                EditorGUILayout.EndVertical();

                //Version Info:
                Rect infoRect = new Rect(bannerRect);
                infoRect.width -= 3;
                infoRect.y += 4;
                infoRect.height -= 8;
                GUI.Label(infoRect, "<color=FFFFFF><b><size=10>v</size>" + DamageNumberEditor.version + " </b></color>", bottomRightTextStyle);

                //Type Info:
                GUI.Label(infoRect, "<color=FFFFFF><b>" + (isMesh ? "Mesh" : "GUI") + "<size=10> </size></b></color>", topRightTextStyle);

                //Button:
                Rect documentationRect = GUILayoutUtility.GetLastRect();
                documentationRect.x += 9;
                documentationRect.y += 9;
                documentationRect.width = 56;
                documentationRect.height = 18;
                if (GUI.Button(documentationRect, "<b>Manual</b>", buttonStyle))
                {
                    Application.OpenURL("https://ekincantas.com/damage-numbers-pro/");
                }
                documentationRect.y += 21;
                if (GUI.Button(documentationRect, "<b>Discord</b>", buttonStyle))
                {
                    Application.OpenURL("https://discord.gg/nWbRkN8Zxr");
                }

                //Box:
                BoxLastRect();

                EditorGUILayout.Space();

                //Calculate Width for GUI Scaling:
                float newWidth = GUILayoutUtility.GetLastRect().width;
                if(newWidth > 50)
                {
                    currentWidth = newWidth;
                }
            }

            return bannerRect;
        }
        static void DrawTabs(Rect lastRect)
        {
            //Position:
            lastRect.y += lastRect.height - 3;
            lastRect.height = 29;

            //Row 1:
            int lastTab = currentTab;

            currentTab = GUI.Toolbar(lastRect, currentTab, new string[] { "Main", "Text", "Fade In", "Fade Out" }, buttonStyle);
            lastRect.height -= 1;
            BoxRect(lastRect);

            string rotAndScaleText = "Rotation & Size";
            if(currentWidth < 388)
            {
                rotAndScaleText = "<size=11>Rotation & Size</size>";

                if(currentWidth < 356)
                {
                    rotAndScaleText = "<size=10>Rotation & Size</size>";

                    if (currentWidth < 324)
                    {
                        rotAndScaleText = "<size=9>Rotation & Size</size>";
                        
                        if (currentWidth < 303)
                        {
                            rotAndScaleText = "<size=8>Rotation & Size</size>";

                            if (currentWidth < 276)
                            {
                                rotAndScaleText = "<size=7>Rotation & Size</size>";
                            }
                        }
                    }
                }
            }

            string spamText = "Spam Control";
            if(currentWidth < 340)
            {
                spamText = "<size=11>Spam Control</size>";

                if (currentWidth < 324)
                {
                    spamText = "<size=10>Spam Control</size>";

                    if (currentWidth < 304)
                    {
                        spamText = "<size=9>Spam Control</size>";

                        if (currentWidth < 367)
                        {
                            spamText = "<size=8>Spam Control</size>";
                        }
                    }
                }
            }

            string performanceText = "Performance";
            if(currentWidth < 336)
            {
                performanceText = "<size=11>Performance</size>";

                if (currentWidth < 310)
                {
                    performanceText = "<size=10>Performance</size>";

                    if (currentWidth < 292)
                    {
                        performanceText = "<size=9>Performance</size>";
                    }
                }
            }

            string movementText = "Movement";
            if(currentWidth < 293)
            {
                movementText = "<size=11>Movement</size>";

                if (currentWidth < 282)
                {
                    movementText = "<size=10>Movement</size>";

                    if (currentWidth < 263)
                    {
                        movementText = "<size=9>Movement</size>";
                    }
                }
            }

            //Row 2:
            lastRect.y += lastRect.height - 3;
            lastRect.height = 22;
            currentTab = 4 + GUI.Toolbar(lastRect, currentTab - 4, new string[] { movementText, rotAndScaleText, spamText, performanceText }, buttonStyle);
            lastRect.height += 2;

            //On Switch:
            if (currentTab != lastTab)
            {
                EditorGUIUtility.keyboardControl = EditorGUIUtility.hotControl = 0;
                hints = new HashSet<string>(); //Clear Hints.
            }

            //Box:
            BoxRect(lastRect, true, false);
        }
        #endregion

        #region Special Sections
        public static void FinalInformation()
        {
            Color finalInformationColor = new Color(0.93f, 0.95f, 1);

            GUIStyle linkStyle = new GUIStyle(labelStyle);
            linkStyle.normal.textColor = linkStyle.focused.textColor = linkStyle.hover.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.9f, 1f, 1) : new Color(0.1f, 0.2f, 0.4f, 1);
            linkStyle.active.textColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.8f, 1f, 1) : new Color(0.15f, 0.4f, 0.6f, 1);

            EditorGUILayout.Space(2);
            StartBox(finalInformationColor);
            EditorGUILayout.BeginVertical();

            GUI.color = new Color(1, 1, 1f, 0.75f);
            if(currentWidth < 285)
            {
                if(currentWidth < 265)
                {
                    Label("<size=10><b>Thank you for using Damage Numbers Pro.</b></size>");
                }
                else
                {
                    Label("<size=11><b>Thank you for using Damage Numbers Pro.</b></size>");
                }
            }
            else
            {
                Label("<b>Thank you for using Damage Numbers Pro.</b>");
            }
            Label("<b>Contact me if you need any help.</b>");
            EditorGUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(1, 1, 1, 1);

            //Link Shrinking:
            string docLink = "https://ekincantas.com/damage-numbers-pro/";
            if(currentWidth < 420f)
            {
                docLink = "https://ekincantas.com/damage-numbers...";

                if (currentWidth < 398)
                {
                    docLink = "https://ekincantas.com/damage...";

                    if (currentWidth < 340)
                    {
                        docLink = "https://ekincantas.com/...";

                        if(currentWidth < 293)
                        {
                            docLink = "Open Link";
                        }
                    }
                }
            }
            string discordLink = "https://discord.gg/nWbRkN8Zxr";
            if (currentWidth < 335f)
            {
                discordLink = "https://discord.gg/...";

                if (currentWidth < 264)
                {
                    discordLink = "Open Link";
                }
            }

            EditorGUILayout.LabelField("<b>Documentation:</b>", labelStyle, GUILayout.Width(100));
            if (GUILayout.Button("<b>" + docLink + "</b>", linkStyle))
            {
                Application.OpenURL("https://ekincantas.com/damage-numbers-pro/");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(1, 1, 1, 1);
            EditorGUILayout.LabelField("<b>Discord:</b>", labelStyle, GUILayout.Width(100));
            if (GUILayout.Button("<b>" + discordLink + "</b>", linkStyle))
            {
                Application.OpenURL("https://discord.gg/nWbRkN8Zxr");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(1, 1, 1, 1);


            string emailPrefix = "<b>Email:</b>";
            if(currentWidth < 259)
            {
                emailPrefix = "<size=11><b>Email:</b></size>";

                if(currentWidth < 256)
                {
                    emailPrefix = "<size=10><b>Email:</b></size>";

                    if (currentWidth < 254)
                    {
                        emailPrefix = "<size=9><b>Email:</b></size>";
                    }
                }
            }

            EditorGUILayout.LabelField(emailPrefix, labelStyle, GUILayout.Width(100 + Mathf.Min(0,(currentWidth - 320))));
            EditorGUILayout.SelectableLabel("<b>ekincantascontact@gmail.com</b>", linkStyle, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
            CloseBox(finalInformationColor);
            EditorGUILayout.Space(5);
        }

        public static void Externalnspectors(bool isMesh, Object target)
        {
            EditorGUILayout.Space(2);
            Lines();

            Color externalInspectorColor = new Color(0.93f, 0.95f, 1);

            EditorGUILayout.Space(2);
            StartBox(externalInspectorColor);
            GUI.backgroundColor = externalInspectorColor;
            EditorGUILayout.BeginVertical();

            bool editingPrefabPreview = EditingPrefabPreview(target); ;

            if (editingPrefabPreview)
            {
                GUI.color = new Color(1, 1, 1f, 0.75f);
                ScalingLabel("<b>Open</b> the prefab to access the <b>presets</b>, <b>material</b> and <b>text mesh pro</b> tabs.", 440);
                OpenPrefabButton(target);

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
                CloseBox(externalInspectorColor);
                return;
            }

            int previousEditor = currentEditor;
            if (cleanEditor)
            {
                previousEditor = -1; //Force Clean
                cleanEditor = false;
            }

            //Tab Names:
            string textMeshProTab = "TextMeshPro";
            string materialTab = "Material";
            if (damageNumbers != null && damageNumbers.Length > 1)
            {
                textMeshProTab = damageNumbers.Length + " TextMeshPros";
                materialTab = "Materials";
            }
            if(currentWidth < 266)
            {
                textMeshProTab = "<size=11>" + textMeshProTab + "</size>";
            }

            //Tab Rect:
            GUILayout.BeginVertical();
            GUILayout.Space(13);
            GUILayout.EndVertical();

            Rect tabRect = GUILayoutUtility.GetLastRect();
            tabRect.x -= 5;
            tabRect.width += 12;
            tabRect.y -= 5;
            tabRect.height = 25;

            //Draw Tab:
            currentEditor = GUI.Toolbar(tabRect, currentEditor, new string[] { "Presets", materialTab , textMeshProTab });

            //Box Tab Rect:
            tabRect.y += 23;
            tabRect.height = 3;
            tabRect.width -= 3;

            if (currentEditor == 1)
            {
                EditorGUILayout.Space(14);

                //Material:
                if (previousEditor != 1)
                {
                    ResetMaterials();
                }

                if (materialEditor != null)
                {
                    materialEditor.DrawHeader();
                    materialEditor.OnInspectorGUI();
                }
            }
            else if (currentEditor == 2)
            {
                EditorGUILayout.Space(14);

                if (damageNumbers.Length > 1)
                {
                    GUI.color = new Color(1, 1, 1, 0.7f);
                    Label("The fancy inspector does not work for <b>multiple</b> damage numbers.");
                    Label("You can also <b>manually select</b> the text-mesh-pro components.");
                    Label("- Sorry for the inconvenience.");
                    GUI.color = Color.white;
                    EditorGUILayout.Space(8);
                }

                if (isMesh)
                {
                    //Text Mesh Pro:
                    if (previousEditor != 2)
                    {
                        if (textMeshProEditor != null)
                        {
                            Object.DestroyImmediate(textMeshProEditor);
                        }
                        textMeshProEditor = Editor.CreateEditor(textMeshPros, null);
                    }

                    //Editor:
                    if (textMeshProEditor != null)
                    {
                        textMeshProEditor.DrawHeader();
                        if (textMeshPros.Length > 1)
                        {
                            textMeshProEditor.DrawDefaultInspector();
                        }
                        else
                        {
                            textMeshProEditor.OnInspectorGUI();
                        }
                    }
                }
                else
                {
                    //Text Mesh Pro:
                    if (previousEditor != 2)
                    {
                        if (textMeshProEditor != null)
                        {
                            Object.DestroyImmediate(textMeshProEditor);
                        }

                        TextMeshProUGUI[] tmps = new TextMeshProUGUI[damageNumbers.Length];
                        for(int i = 0; i < damageNumbers.Length; i++)
                        {
                            TMP_Text tmpText = damageNumbers[i].GetTextMesh();

                            if(tmpText.GetType() == typeof(TextMeshProUGUI))
                            {
                                tmps[i] = (TextMeshProUGUI)tmpText;
                            }
                            else
                            {
                                return;
                            }
                        }

                        textMeshProEditor = Editor.CreateEditor(tmps, null);
                    }

                    //Editor:
                    if (textMeshProEditor != null)
                    {
                        textMeshProEditor.DrawHeader();
                        if (textMeshPros.Length > 1)
                        {
                            textMeshProEditor.DrawDefaultInspector();
                        }
                        else
                        {
                            textMeshProEditor.OnInspectorGUI();
                        }

                        //Match both TMPs:
                        foreach(DamageNumber dn in damageNumbers)
                        {
                            TMP_Text[] tmps = dn.GetTextMeshs();
                            if(tmps.Length > 1)
                            {
                                EditorUtility.CopySerialized(((TextMeshProUGUI)tmps[0]), ((TextMeshProUGUI)tmps[1]));
                            }
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.Space(14);
                ShowPresets(isMesh);
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
            CloseBox(externalInspectorColor);

            //Line under Tabs:
            GUI.color = externalInspectorColor;
            DrawOutlineBox(tabRect);
            tabRect.width += 3;
            DrawBox(tabRect);
            GUI.color = Color.white;
        }
        public static void ResetMaterials()
        {
            List<Material> allMaterials = new List<Material>();
            foreach (DamageNumber dn in damageNumbers)
            {
                dn.GetReferencesIfNecessary();

                foreach (Material mat in dn.GetSharedMaterials())
                {
                    if (allMaterials.Contains(mat) == false)
                    {
                        allMaterials.Add(mat);
                    }
                }
            }

            currentMaterials = new Material[allMaterials.Count];
            for (int n = 0; n < allMaterials.Count; n++)
            {
                currentMaterials[n] = allMaterials[n];
            }

            if (materialEditor != null)
            {
                Object.DestroyImmediate(materialEditor);
            }

            materialEditor = (MaterialEditor)Editor.CreateEditor(currentMaterials);
        }

        static void ShowPresets(bool isMesh)
        {
            PresetCategory("Style", isMesh);
            EditorGUILayout.Space(12);
            PresetCategory("Fade In", isMesh);
            EditorGUILayout.Space(12);
            PresetCategory("Fade Out", isMesh);
            EditorGUILayout.Space(12);
            PresetCategory("Behaviour", isMesh);
        }
        static void PresetCategory(string category, bool isMesh)
        {
            GUI.color = new Color(1, 1, 1, 0.7f);
            EditorGUILayout.LabelField("<size=14><b> - - - - - - - - - - - - - - - - - - - - - - - - - - - - - " + category + " - - - - - - - - - - - - - - - - - - - - - - - - - - - - - </b></size>", centerTextStyle);
            GUI.color = Color.white;

            DNPPreset[] presets = allPresets[category];

            if(presets == null || presets.Length == 0)
            {
                GUI.color = new Color(1, 1, 1, 0.7f);
                Label("Presets could not be loaded.");
                Label("Maybe you deleted or moved a folder ?");
                GUI.color = Color.white;
                return;
            }

            int buttonsPerRow = 4;
            if(EditorGUIUtility.currentViewWidth < 440)
            {
                buttonsPerRow = 3;

                if(EditorGUIUtility.currentViewWidth < 375)
                {
                    buttonsPerRow = 2;
                }
            }

            int currentCount = 0;
            foreach (DNPPreset preset in presets)
            {
                //Check Applied:
                bool isApplied = true;
                foreach (DamageNumber dn in damageNumbers)
                {
                    if(!preset.IsApplied(dn))
                    {
                        isApplied = false;
                        break;
                    }
                }
                if(isApplied)
                {
                    GUI.enabled = false;
                }

                //Increase Count:
                currentCount++;
                if (currentCount % buttonsPerRow == 1)
                {
                    if(currentCount > 1)
                    {
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                }

                //Apply Button:
                if (GUILayout.Button(preset.name))
                {
                    if(isMesh)
                    {
                        Object[] undoObjects = new Object[damageNumbers.Length + textMeshPros.Length];
                        for (int i = 0; i < damageNumbers.Length; i++)
                        {
                            undoObjects[i] = damageNumbers[i];
                        }
                        for (int i = 0; i < textMeshPros.Length; i++)
                        {
                            undoObjects[i + damageNumbers.Length] = textMeshPros[i];
                        }

                        Undo.RecordObjects(undoObjects, "Applied the [" + preset.name + "] " + category + " Preset.");
                    }
                    else
                    {
                        Object[] undoObjects = new Object[damageNumbers.Length * 3];
                        for (int i = 0; i < damageNumbers.Length; i += 3)
                        {
                            undoObjects[i] = damageNumbers[i];
                            undoObjects[i + 1] = damageNumbers[i].GetTextMeshs()[0];
                            undoObjects[i + 2] = damageNumbers[i].GetTextMeshs()[1];
                        }

                        Undo.RecordObjects(undoObjects, "Applied the [" + preset.name + "] " + category + " Preset.");
                    }

                    foreach(DamageNumber dn in damageNumbers)
                    {
                        preset.Apply(dn);
                    }

                    foreach(DamageNumber dn in damageNumbers)
                    {
                        dn.UpdateText();
                    }
                }

                //Reenable GUI:
                GUI.enabled = true;
            }

            GUI.enabled = false;
            GUI.color = new Color(0, 0, 0, 0);
            int modulo = currentCount % buttonsPerRow;
            if(modulo > 0)
            {
                for (int n = 0; n < buttonsPerRow - modulo; n++)
                {
                    GUILayout.Button("- - - - -");
                }
            }
            GUI.color = Color.white;
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Structure
        public static bool CheckStructure(DamageNumberEditor damageNumberEditor)
        {
            //Type:
            bool isMesh = damageNumbers[0].IsMesh(); ;

            //Check if structure is flawed.
            bool isStructureFlawed = false;
            bool isOutdated = false;
            if(isMesh)
            {
                for (int n = 0; n < damageNumbers.Length; n++)
                {
                    DamageNumber dn = damageNumbers[n];
                    TextMeshPro tmp = textMeshPros[n];
                    Transform meshA = meshAs[n];
                    Transform meshB = meshBs[n];

                    if (tmp == null || meshA == null || meshB == null)
                    {
                        isStructureFlawed = true;

                        if (dn.transform.Find("TextA") != null)
                        {
                            isOutdated = true;
                        }

                        break;
                    }
                }
            }
            else
            {
                for (int n = 0; n < damageNumbers.Length; n++)
                {
                    DamageNumber dn = damageNumbers[n];

                    if (dn.transform.Find("TMPA") == null || dn.transform.Find("TMPB") == null)
                    {
                        isStructureFlawed = true;

                        break;
                    }
                }
            }

            //Create button to fix structure.
            if (isStructureFlawed)
            {
                //Start Box:
                StartBox(new Color(1, 1, 0.8f));

                //Structure Build Button:
                GUI.color = new Color(1, 1, 0.8f);
                if (GUILayout.Button(isOutdated ? "Upgrade Structure" : "Build Structure", GUILayout.Width(140)))
                {
                    if(isMesh)
                    {
                        foreach (DamageNumber dn in damageNumbers)
                        {
                            PrepareMeshStructure(dn.gameObject);
                        }
                    }
                    else
                    {
                        foreach (DamageNumber dn in damageNumbers)
                        {
                            PrepareGUIStructure(dn.gameObject);
                        }
                    }

                    PrepareInspector(damageNumberEditor);
                }

                //Text:
                GUI.color = new Color(1, 1, 1, 0.7f);
                if (isOutdated)
                {
                    EditorGUILayout.LabelField("Version 4.0 has changed the structure of damage numbers.", labelStyle);
                    EditorGUILayout.LabelField("Click the button above to <b>upgrade</b> this damage number.", labelStyle);
                }
                else
                {
                    EditorGUILayout.LabelField("Important components are missing.", labelStyle);
                    EditorGUILayout.LabelField("Click the button above to <b>prepare</b> this damage number.", labelStyle);
                }

                //Close Box:
                CloseBox(new Color(1, 1, 0.7f));
                EditorGUILayout.Space();
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void PrepareMeshStructure(GameObject go)
        {
            //Add Sorting Group:
            if (go.GetComponent<SortingGroup>() == null)
            {
                go.AddComponent<SortingGroup>().sortingOrder = 1000;
            }

            //Rename TextA to TMP:
            Transform textA = go.transform.Find("TextA");
            if (textA != null)
            {
                textA.gameObject.name = "TMP";
            }

            //Destroy TextB:
            if (go.transform.Find("TextB"))
            {
                MonoBehaviour.DestroyImmediate(go.transform.Find("TextB").gameObject, true);
            }

            //Create TMP:
            if (go.transform.Find("TMP") == null)
            {
                NewTextMesh("TMP", go.transform);
            }

            //Create MeshA:
            if (go.transform.Find("MeshA") == null)
            {
                DamageNumber.NewMesh("MeshA", go.transform);
            }

            //Create MeshB:
            if (go.transform.Find("MeshB") == null)
            {
                DamageNumber.NewMesh("MeshB", go.transform);
            }

            //Undo:
            Undo.RegisterCreatedObjectUndo(go, "Create new Damage Number (Mesh).");
        }
        public static void PrepareGUIStructure(GameObject go)
        {
            //Create TMP:
            if (go.transform.Find("TMPA") == null)
            {
                NewTextGUI("TMPA", go.transform);
            }
            if (go.transform.Find("TMPB") == null)
            {
                NewTextGUI("TMPB", go.transform);
            }

            //Add Rect Component:
            if(go.GetComponent<RectTransform>() == null)
            {
                go.AddComponent<RectTransform>();
            }

            //Undo:
            Undo.RegisterCreatedObjectUndo(go, "Create new Damage Number (GUI).");
        }

        public static GameObject NewTextMesh(string tmName, Transform parent)
        {
            //GameObject:
            GameObject newTM = new GameObject();
            newTM.name = tmName;

            //TextMeshPro:
            TextMeshPro tmp = newTM.AddComponent<TextMeshPro>();
            tmp.fontSize = 5;
            tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
            tmp.text = "1";
            tmp.enableWordWrapping = false;

            //Size Delta:
            RectTransform rectTransform = tmp.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(4, 2);
            }

            //Transform:
            newTM.transform.SetParent(parent, true);
            newTM.transform.localPosition = Vector3.zero;
            newTM.transform.localScale = Vector3.one;
            newTM.transform.localEulerAngles = Vector3.zero;

            return newTM;
        }

        public static GameObject NewTextGUI(string tmName, Transform parent)
        {
            //GameObject:
            GameObject newTM = new GameObject();
            newTM.name = tmName;

            //TextMeshPro:
            TextMeshProUGUI tmp = newTM.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 30;
            tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
            tmp.text = "1";
            tmp.enableWordWrapping = false;
            tmp.raycastTarget = false;

            //Size Delta:
            RectTransform rectTransform = tmp.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(4, 2);
            }

            //Transform:
            newTM.transform.SetParent(parent, true);
            newTM.transform.localPosition = Vector3.zero;
            newTM.transform.localScale = Vector3.one;
            newTM.transform.localEulerAngles = Vector3.zero;

            return newTM;
        }
        public static void FixTextMeshPro()
        {
            bool destroyedSomething = false;
            foreach (TextMeshPro textMesh in textMeshPros)
            {
                Transform tmp = textMesh.transform;
                tmp.localPosition = Vector3.zero;

                tmp.gameObject.SetActive(true);
                for (int n = 0; n < tmp.childCount; n++)
                {
                    Object.DestroyImmediate(tmp.GetChild(n).gameObject);
                    destroyedSomething = true;
                }
                tmp.gameObject.SetActive(false);
            }
            
            foreach(Transform meshA in meshAs)
            {
                for(int n = 0; n < meshA.childCount; n++)
                {
                    Transform child = meshA.GetChild(n);
                    if(child.GetComponent<MeshRenderer>() != null)
                    {
                        destroyedSomething = true;
                        Object.DestroyImmediate(child.gameObject);
                    }
                }
            }

            foreach (Transform meshB in meshBs)
            {
                for (int n = 0; n < meshB.childCount; n++)
                {
                    Transform child = meshB.GetChild(n);
                    if (child.GetComponent<MeshRenderer>() != null)
                    {
                        destroyedSomething = true;
                        Object.DestroyImmediate(child.gameObject);
                    }
                }
            }

            if(destroyedSomething)
            {
                foreach(DamageNumber dn in damageNumbers)
                {
                    dn.GetReferences();
                }
            }
        }
        #endregion

        #region Miscellaneous
        public static void BeginInspector()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
        }
        public static void EndInspector()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
            EditorGUILayout.EndHorizontal();
        }
        static void PrepareStyles()
        {
            if(generatedGUIStyles)
            {
                return;
            }

            //Label:
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.richText = true;

            //Button:
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.richText = true;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            //Right Anchor:
            rightTextStyle = new GUIStyle(labelStyle);
            rightTextStyle.alignment = TextAnchor.MiddleRight;

            //Center Anchor:
            centerTextStyle = new GUIStyle(labelStyle);
            centerTextStyle.alignment = TextAnchor.MiddleCenter;

            //Bottom Right Anchor:
            bottomRightTextStyle = new GUIStyle(labelStyle);
            bottomRightTextStyle.alignment = TextAnchor.LowerRight;

            //Top Right Anchor:
            topRightTextStyle = new GUIStyle(labelStyle);
            topRightTextStyle.alignment = TextAnchor.UpperRight;

            //White Box:
            whiteBoxStyle = new GUIStyle(GUI.skin.box);
            whiteBoxStyle.normal.background = whiteBoxStyle.onNormal.background = whiteBoxStyle.active.background =
            whiteBoxStyle.onActive.background = whiteBoxStyle.focused.background = whiteBoxStyle.onFocused.background =
            whiteBoxStyle.hover.background = whiteBoxStyle.onHover.background = whiteBoxTexture;

            //Rich Everything:
            for(int n = 0; n < GUI.skin.customStyles.Length; n++)
            {
                if(GUI.skin.customStyles[n] != null)
                {
                    GUI.skin.customStyles[n].richText = true;
                }
            }

            //Finish:
            generatedGUIStyles = true;
        }
        public static bool HintButton(string category)
        {
            bool toggled = hints.Contains(category);

            string buttonName = toggled ? "<size=8> </size><b>?</b>" : "?";

            if (GUILayout.Button(buttonName, buttonStyle, GUILayout.Width(21)))
            {
                if (hints.Contains(category))
                {
                    hints.Remove(category);
                }
                else
                {
                    hints.Add(category);
                }

                toggled = !toggled;
            }

            if (toggled)
            {
                GUI.color = new Color(1, 1, 1, 0.8f);
                GUI.Toolbar(GUILayoutUtility.GetLastRect(), 0, new string[] { buttonName }, buttonStyle);
                GUI.color = new Color(1, 1, 1, 1f);
            }

            return toggled;
        }
        public static void RepaintInspector()
        {
            repaintViews = true;
            GUI.FocusControl("");
        }

        public static bool EditingPrefabPreview(Object target)
        {
            return EditorUtility.IsPersistent(target) && PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        public static void OpenPrefabButton(Object target)
        {
            GUI.color = Color.white;
            if (GUILayout.Button("Click here to open the prefab."))
            {
                try
                {
                    PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(target));
                }
                catch
                {
                    EditorUtility.DisplayDialog("Something went wrong.", "Please open the prefab manually, by double-clicking it in the project window.", "Okay");
                }
            }
        }
        #endregion

        #region Label Utility
        public static void Lines()
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            EditorGUILayout.LabelField("− − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − − −", GUILayout.Height(7f));
            GUI.color = new Color(1, 1, 1, 1f);
        }
        public static void Label(string text)
        {
            EditorGUILayout.LabelField(text, labelStyle);
        }

        public static void ScalingLabel(string text, float scaleWidth)
        {
            int size = Mathf.FloorToInt(Mathf.Clamp(12 * Mathf.Pow(DNPEditorInternal.currentWidth / scaleWidth, 1.525f), 9, 12));
            Label("<size=" + size + ">" + text + "</size>");
        }

        public static string CheckmarkString(bool state)
        {
            if(EditorGUIUtility.isProSkin)
            {
                return state ? "<size=15><b><color=#00FF00>✓</color></b></size>" : "<size=16><b><color=#FF0000>✗</color></b></size>";
            }
            else
            {
                return state ? "<size=15><b><color=#00BB00>✓</color></b></size>" : "<size=16><b><color=#BB0000>✗</color></b></size>";
            }
        }
        #endregion

        #region Box Utility
        public static void StartBox(Color color, bool isActivated = true)
        {
            GUI.color = color;
            StartBox(isActivated);
            GUI.color = Color.white;
        }
        public static void StartBox(bool isActivated = true)
        {
            //Start Box:
            if (EditorGUIUtility.isProSkin)
            {
                if (isActivated)
                {
                    GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
                }
                else
                {
                    GUI.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1);
                }
            }
            else
            {
                if (isActivated)
                {
                    GUI.backgroundColor = new Color(0.68f, 0.68f, 0.68f, 1);
                }
                else
                {
                    GUI.backgroundColor = new Color(0.72f, 0.72f, 0.72f, 1);
                }
            }
            GUILayout.BeginHorizontal(whiteBoxStyle);
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUI.backgroundColor = Color.white;
        }
        public static void CloseBox(Color color, bool isActivated = true)
        {
            GUI.color = color;
            CloseBox(isActivated);
            GUI.color = Color.white;
        }
        public static void CloseBox(bool isActivated = true)
        {
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            lastRect.width -= 2;
            BoxLastRect(isActivated);
        }
        static void BoxLastRect(bool isActivated = true)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            BoxRect(lastRect, isActivated);
        }
        static void BoxRect(Rect targetRect, bool isActivated = true, bool withTop = true)
        {
            Rect leftBar = new Rect(targetRect);
            leftBar.width = 3;

            Rect rightBar = new Rect(targetRect);
            rightBar.x += rightBar.width - 3;
            rightBar.width = 3;

            Rect topBar = new Rect(targetRect);
            topBar.height = 3;
            topBar.x += 3;
            topBar.width -= 6;

            Rect bottomBar = new Rect(targetRect);
            bottomBar.y += bottomBar.height - 3;
            bottomBar.height = 3;
            bottomBar.x += 3;
            bottomBar.width -= 6;

            DrawOutlineBox(leftBar, isActivated);
            DrawOutlineBox(rightBar, isActivated);
            DrawOutlineBox(bottomBar, isActivated);
            if (withTop)
            {
                DrawOutlineBox(topBar, isActivated);
            }

            DrawBox(leftBar, isActivated);
            DrawBox(rightBar, isActivated);
            DrawBox(bottomBar, isActivated);
            if (withTop)
            {
                DrawBox(topBar, isActivated);
            }
        }
        static void DrawBox(Rect rectPosition, bool isActivated = true)
        {
            Color boxColor = default;

            if (EditorGUIUtility.isProSkin)
            {
                if (isActivated)
                {
                    boxColor = new Color(0.66f, 0.66f, 0.66f, 1);
                }
                else
                {
                    boxColor = new Color(0.38f, 0.38f, 0.38f, 1);
                }
            }
            else
            {
                if (isActivated)
                {
                    boxColor = new Color(0.55f, 0.55f, 0.55f, 1);
                }
                else
                {
                    boxColor = new Color(0.67f, 0.67f, 0.67f, 1);
                }
            }

            DrawBox(rectPosition, boxColor);
        }
        static void DrawBox(Rect rectPosition, Color boxColor)
        {
            GUI.backgroundColor = boxColor;
            GUI.Box(rectPosition, "", whiteBoxStyle);
            GUI.backgroundColor = Color.white;
        }
        static void DrawOutlineBox(Rect rectPosition, bool isActivated = true)
        {
            //Adjust Position:
            rectPosition.width += 2;
            rectPosition.height += 2;

            //Draw Box:
            if (EditorGUIUtility.isProSkin)
            {
                if (isActivated)
                {
                    DrawBox(rectPosition, new Color(0.24f, 0.24f, 0.24f, 1));
                }
                else
                {
                    DrawBox(rectPosition, new Color(0.21f, 0.21f, 0.21f, 1));
                }
            }
            else
            {
                if (isActivated)
                {
                    DrawBox(rectPosition, new Color(0.35f, 0.35f, 0.35f, 1));
                }
                else
                {
                    DrawBox(rectPosition, new Color(0.5f, 0.5f, 0.5f, 1));
                }
            }
        }
        #endregion
    }
}

#endif