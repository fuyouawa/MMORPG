#if UNITY_2019_4_OR_NEWER
/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using GraphProcessor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QFramework
{
    [Serializable, NodeMenuItem("DocKit/ImageNode 图像节点")]
    public class DocumentImageNode : DocKitNode
    {
        [HideInInspector] public Texture2D CNTexture;
        [HideInInspector] public Texture2D ENTexture;

        public Texture2D Texture // for node view
        {
            get => LocaleKitEditor.IsCN.Value ? CNTexture : ENTexture;
            set
            {
                if (LocaleKitEditor.IsCN.Value)
                {
                    CNTexture = value;

                    if (!ENTexture)
                    {
                        ENTexture = CNTexture;
                    }
                }
                else
                {
                    ENTexture = value;

                    if (!CNTexture)
                    {
                        CNTexture = ENTexture;
                    }
                }
            }
        }

        public override string name => LocaleKitEditor.IsCN.Value ? "图像" : "Image";

        public override string GetMarkdownString()
        {
            var path = AssetDatabase.GetAssetPath(Texture);
            var fileName = path.GetFileName();
            return "![" + fileName + "](./" + graph.name + "/Editor/" + fileName + ")";
        }
    }

    [NodeCustomEditor(typeof(DocumentImageNode))]
    public class DocumentImageNodeView : BaseNodeView
    {
        public override void Disable()
        {
            owner.graph.Save();
            base.Disable();
        }

        public override void Enable()
        {
            base.Enable();

            var node = nodeTarget as DocumentImageNode;


            new IMGUIContainer(() => LocaleKitEditor.DrawSwitchToggle(Color.white))
                .AddToParent(new VisualElement()
                    .Padding(6, 0, 0, 0)
                    .AddToParent(rightTitleContainer)
                );

            var image = new Image()
            {
                scaleMode = ScaleMode.ScaleToFit
            };


            var objectField = new ObjectField
            {
                objectType = typeof(Texture2D)
            };
            objectField.RegisterValueChangedCallback(o =>
            {
                node.Texture = o.newValue as Texture2D;
                UpdatePreview(node, image);
            });

            objectField.SetValueWithoutNotify(node.Texture);

            controlsContainer.Add(new Button(() =>
            {
                var filePath = EditorUtility.OpenFilePanelWithFilters("选择图片", "~/Desktop", new[]
                {
                    "Image files", "png,jpg,jpeg"
                });

                if (!string.IsNullOrEmpty(filePath))
                {
                    var graphPath = AssetDatabase.GetAssetPath(this.owner.graph);
                    var parentFolder = graphPath.GetFolderPath();
                    var imageFolder = parentFolder + "/" + this.owner.graph.name + "/Editor/";
                    imageFolder.CreateDirIfNotExists();

                    var savedImageFilePath = imageFolder + filePath.GetFileName().Replace(" ", "_"); // 所有的空格都改成 _

                    if (File.Exists(savedImageFilePath))
                    {
                        // 重名即根据id重新生成名字
                        savedImageFilePath = imageFolder + filePath.GetFileNameWithoutExtend() +
                                             ((DocumentGraph)owner.graph).GenerateID() +
                                             filePath.GetFileExtendName();
                    }


                    File.Copy(filePath, savedImageFilePath);


                    AssetDatabase.Refresh();
                    // 更改图像导入设置
                    var textureImporter = AssetImporter.GetAtPath(savedImageFilePath) as TextureImporter;
                    textureImporter.textureType = TextureImporterType.GUI;
                    textureImporter.SaveAndReimport();

                    var newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(savedImageFilePath);

                    objectField.value = newTexture;
                    objectField.SetValueWithoutNotify(newTexture);
                    UpdatePreview(node, image);
                }
            }).Self(btn => { btn.text = "..."; }));

            controlsContainer.Add(objectField);
            controlsContainer.Add(image);

            IUnRegister unRegister = null;
            unRegister = LocaleKitEditor.IsCN.RegisterWithInitValue(_ =>
            {
                try
                {
                    UpdatePreview(node, image);
                    UpdateTitle();
                    objectField.value = node.Texture;
                }
                catch
                {
                    unRegister?.UnRegister();
                }
            });
        }


        private void UpdatePreview(DocumentImageNode node, Image image)
        {
            if (node.Texture)
            {
                image.image = node.Texture;
                image.visible = true;
            }
            else
            {
                image.visible = false;
            }

            // this.PushSaveCommand(() =>
            // {
            owner.graph.Save();
            // });
        }

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif