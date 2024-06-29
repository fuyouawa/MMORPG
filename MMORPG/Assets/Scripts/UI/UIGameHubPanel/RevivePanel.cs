using MMORPG.Event;
using UnityEngine;
using QFramework;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.UI
{
	public partial class RevivePanel : ViewController
    {
        private bool _reviving;
        private float _remainingReviveTime;

        public void BeginRevive(float reviveTime)
        {
            _reviving = true;
            _remainingReviveTime = reviveTime;
        }

        public void EndRevive()
        {
            _reviving = false;
            _remainingReviveTime = 0f;
        }

        void Start()
		{
		}

        void Update()
        {
            if (_reviving)
            {
                _remainingReviveTime -= Time.deltaTime;
                TextReviveTime.text = $"您已死亡, 复活时间:{_remainingReviveTime:0}s";
                if (Mathf.Approximately(_remainingReviveTime, 0))
                {
                    _reviving = false;
                    _remainingReviveTime = 0f;
                }
            }
        }
	}
}
