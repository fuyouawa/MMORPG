using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DamageNumbersPro.Internal;
using TMPro;

namespace DamageNumbersPro
{
    [DisallowMultipleComponent]
    public class DamageNumberGUI : DamageNumber
    {
        /* 
         * Contact me if you need any support.
         * Email: ekincantascontact@gmail.com
         * Discord: https://discord.com/invite/nWbRkN8Zxr
         * 
         * Check the manual for more information.
         * Manual: https://ekincantas.com/damage-numbers-pro/
         * 
         * Thank you for using my asset.
         * If you want to add your own code please use the functions below.
         * I recommend creating a duplicate of this script first or creating your own script which derives from DamageNumber.
         * Otherwise you may loose your custom code when you update damage numbers pro.
         * 
         * Good Luck
         */

        //Custom Events:
        protected override void OnPreSpawn()
        {
            //Fixes an issue where the previous mesh was visible for 1 frame.
            if (textMeshProA != null)
            {
                textMeshProA.enabled = textMeshProB.enabled = false;
            }
        }
        protected override void OnStart()
        {
            //Only damage numbers of the same parent can interact with each other.
            if(spamGroup != "" && transform.parent != null)
            {
                spamGroup += transform.parent.GetInstanceID();
            }

            //GUI Alpha Fix:
            skippedFrames = 0;
            skipFrames = true;
            realStartTime = Time.unscaledTime;
        }
        protected override void OnLateUpdate()
        {
            //GUI Alpha Fix:
            if (skipFrames)
            {
                transform.localScale = Vector3.one * 0.0001f;
                currentFade = 0;

                if (skippedFrames > 2 && Time.unscaledTime > realStartTime + 0.03f)
                {
                    skipFrames = false;
                    transform.localScale = originalScale;
                    currentFade = 0;
                }
            }
        }
        protected override void OnStop()
        {

        }
        protected override void OnUpdate(float deltaTime)
        {
            //GUI Alpha Fix:
            skippedFrames++;
        }
        protected override void OnAbsorb(float number, float newSum)
        {

        }
        protected override void OnTextUpdate()
        {

        }

        /*
         * The code below is required for the GUI version of damage numbers pro.
         * So you should not change it too much.
         * But you can use the events above to add your custom behavior.
         */

        //References:
        RectTransform myRect;
        TextMeshProUGUI textMeshProA;
        TextMeshProUGUI textMeshProB;
        RectTransform textRectA;
        RectTransform textRectB;
        List<TMP_SubMeshUI> subMeshs;

        //Internal:
        float realStartTime;
        bool skipFrames;
        int skippedFrames;

        //Components:
        public override void GetReferencesIfNecessary()
        {
            if(textMeshProA == null)
            {
                GetReferences();
            }
        }
        public override void GetReferences()
        {
            baseAlpha = 0.9f;

            myRect = GetComponent<RectTransform>();
            transformA = transform.Find("TMPA");
            transformB = transform.Find("TMPB");
            textMeshProA = transformA.GetComponent<TextMeshProUGUI>();
            textMeshProB = transformB.GetComponent<TextMeshProUGUI>();
            textRectA = transformA.GetComponent<RectTransform>();
            textRectB = transformB.GetComponent<RectTransform>();
        }
        public override TMP_Text[] GetTextMeshs()
        {
            return new TMP_Text[] { textMeshProA, textMeshProB };
        }
        public override TMP_Text GetTextMesh()
        {
            return textMeshProA;
        }

        //Materials:
        public override Material[] GetSharedMaterials()
        {
            return textMeshProA.fontSharedMaterials;
        }
        public override Material[] GetMaterials()
        {
            return textMeshProA.fontMaterials;
        }
        public override Material GetSharedMaterial()
        {
            return textMeshProA.fontSharedMaterial;
        }
        public override Material GetMaterial()
        {
            return textMeshProA.fontMaterial;
        }

        //Text:
        protected override void SetTextString(string fullString)
        {
            textMeshProA.text = textMeshProB.text = fullString;

            if(!textMeshProA.enabled)
            {
                textMeshProA.enabled = textMeshProB.enabled = true;
            }

            textMeshProA.ForceMeshUpdate();
            textMeshProB.ForceMeshUpdate();
            textMeshProA.canvasRenderer.SetMesh(textMeshProA.mesh);
            textMeshProB.canvasRenderer.SetMesh(textMeshProB.mesh);

            meshs = new List<Mesh>();
            meshs.Add(textMeshProA.mesh);
            meshs.Add(textMeshProB.mesh);

            //Sub Meshs:
            subMeshs = new List<TMP_SubMeshUI>();
            foreach(TMP_SubMeshUI subMesh in textMeshProA.GetComponentsInChildren<TMP_SubMeshUI>())
            {
                subMeshs.Add(subMesh);
                meshs.Add(subMesh.mesh);
            }
            foreach (TMP_SubMeshUI subMesh in textMeshProB.GetComponentsInChildren<TMP_SubMeshUI>())
            {
                subMeshs.Add(subMesh);
                meshs.Add(subMesh.mesh);
            }
        }

        //Position:
        public override Vector3 GetPosition()
        {
            return myRect.anchoredPosition3D;
        }
        public override void SetPosition(Vector3 newPosition)
        {
            GetReferencesIfNecessary();
            myRect.anchoredPosition3D = newPosition;
        }
        public override void SetAnchoredPosition(Transform rectParent, Vector2 anchoredPosition)
        {
            //Old Transform:
            Vector3 oldScale = transform.localScale;

            //Set Parent and Position:
            GetReferencesIfNecessary();
            myRect.SetParent(rectParent, false);
            myRect.anchoredPosition = anchoredPosition;

            //New Transform:
            transform.localScale = oldScale;
            transform.eulerAngles = textMeshProA.canvas.transform.eulerAngles;
        }
        public override void SetAnchoredPosition(Transform rectParent, Transform rectPosition, Vector2 relativeAnchoredPosition)
        {
            //Old Transform:
            Vector3 oldScale = transform.localScale;

            //Set Parent and Position:
            GetReferencesIfNecessary();
            myRect.SetParent(rectParent, false);
            myRect.position = rectPosition.position;
            myRect.anchoredPosition += relativeAnchoredPosition;

            //New Transform:
            transform.localScale = oldScale;
            transform.eulerAngles = textMeshProA.canvas.transform.eulerAngles;
        }

        protected override void SetLocalPositionA(Vector3 localPosition)
        {
            textRectA.anchoredPosition = localPosition * 50;
        }
        protected override void SetLocalPositionB(Vector3 localPosition)
        {
            textRectB.anchoredPosition = localPosition * 50;
        }
        protected override float GetPositionFactor()
        {
            return 100f;
        }

        //Other:
        protected override void OnFade(float currentFade)
        {
            textMeshProA.canvasRenderer.SetMesh(textMeshProA.mesh);
            textMeshProB.canvasRenderer.SetMesh(textMeshProB.mesh);

            foreach (TMP_SubMeshUI subMesh in subMeshs)
            {
                subMesh.canvasRenderer.SetMesh(subMesh.mesh);
            }
        }
        protected override void UpdateRotationZ()
        {
            SetRotationZ(textMeshProA.transform);
            SetRotationZ(textMeshProB.transform);
        }
        public override void CheckAndEnable3D()
        {
            enable3DGame = false;
        }
        public override bool IsMesh()
        {
            return false;
        }
        public override Vector3 GetUpVector()
        {
            return Vector3.up;
        }
        public override Vector3 GetRightVector()
        {
            return Vector3.right;
        }
        public override Vector3 GetFreshUpVector()
        {
            return Vector3.up;
        }
        public override Vector3 GetFreshRightVector()
        {
            return Vector3.right;
        }
    }
}
