using UnityEngine;

namespace Michsky.MUIP
{
    public class MUIPInternalTools : MonoBehaviour
    {
        public static string modalWindowStateName = "Fade-out";
        public static string windowManagerStateName = "WM Window Out";

        public static float GetAnimatorClipLength(Animator _animator, string _clipName)
        {
            float _lengthValue = -1;
            RuntimeAnimatorController _rac = _animator.runtimeAnimatorController;

            for (int i = 0; i < _rac.animationClips.Length; i++)
            {
                if (_rac.animationClips[i].name == _clipName)
                {
                    _lengthValue = _rac.animationClips[i].length;
                    break;
                }
            }

            return _lengthValue;
        }
    }
}