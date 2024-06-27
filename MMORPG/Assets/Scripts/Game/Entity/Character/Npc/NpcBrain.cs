using MMORPG.Command;
using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Entity;
using MMORPG.Event;
using MMORPG.Tool;
using MMORPG.UI;
using QFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using AnimationState = MMORPG.Common.Proto.Entity.AnimationState;
using MMORPG.Common.Proto.Base;
using System.Collections.Generic;

namespace MMORPG.Game
{
    public class NpcBrain : MonoBehaviour, IController
    {
        public float GroundClearance;

        public ActorController ActorController;
        private static Dictionary<string, GameObject> _tipDict;

        public FSM<AnimationState> FSM = new ();

        private GameObject _tip;

        private void Awake()
        {

        }

        private void Start()
        {
            if (_tipDict == null)
            {
                _tipDict = new();
                _tipDict["疑问"] = Resources.Load<GameObject>("Prefabs/Effect/Npc/VerticalIconQuestionMark3D");
                _tipDict["感叹"] = Resources.Load<GameObject>("Prefabs/Effect/Npc/VerticalIconExclamationMark3D");
                _tipDict["星号"] = Resources.Load<GameObject>("Prefabs/Effect/Npc/VerticalIconStar3D");
            }

            this.RegisterEvent<InteractEvent>(e =>
            {
                if (e.Resp.Error != NetError.Success) return;
                if (e.Resp.DialogueId != 0)
                {
                    LoadTip(e.Resp.DialogueId);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.SendCommand(new QueryDialogueIdCommand(ActorController.Entity.EntityId));

            this.RegisterEvent<QueryDialogueIdEvent>(e =>
            {
                if (e.Resp.Error != NetError.Success) return;
                if (e.Resp.DialogueId != 0)
                {
                    LoadTip(e.Resp.DialogueId);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void LoadTip(int dialogueId)
        {
            // 挂载头顶特效
            if (_tip != null)
            {
                Destroy(_tip);
            }
            var dataManagerSystem = this.GetSystem<IDataManagerSystem>();
            var dialogueDefine = dataManagerSystem.GetDialogueDefine(dialogueId);
            if (dialogueDefine.TipResource == "")
            {
                Destroy(_tip);
                return;
            }
            _tip = Instantiate(_tipDict[dialogueDefine.TipResource], transform);

            // 获取CapsuleCollider来确定NPC的高度
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                float npcHeight = capsuleCollider.height;
                _tip.transform.localPosition = new Vector3(0, npcHeight + 0.4f, 0);
            }
            else
            {
                // 如果没有CapsuleCollider，可以设定一个默认高度或其他处理方式
                _tip.transform.localPosition = new Vector3(0, 2, 0);
            }

            _tip.transform.localScale = new Vector3(2, 2, 2);
        }

        private void Update()
        {
            FSM.Update();
        }

        private void FixedUpdate()
        {
            FSM.FixedUpdate();
        }

        private void OnGUI()
        {
            FSM.OnGUI();
        }

        private void OnDestroy()
        {
            FSM.Clear();
        }

        
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}