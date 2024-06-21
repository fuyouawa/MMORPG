using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        public AudioSource PlayerWalkAudio;
        public AudioSource PlayerRunAudio;
        public AudioSource PlayerPickItemAudio;
    }
}
