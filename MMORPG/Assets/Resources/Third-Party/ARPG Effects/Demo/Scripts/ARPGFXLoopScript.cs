using UnityEngine;
using System.Collections;

namespace ARPGFX
{
    public class ARPGFXLoopScript : MonoBehaviour
    {

        public GameObject chosenEffect;
        public float loopTimeLimit = 2.0f;

        void Start()
        {
            PlayEffect();
        }


        public void PlayEffect()
        {
            StartCoroutine("EffectLoop");
        }


        IEnumerator EffectLoop()
        {
            //GameObject effectPlayer = (GameObject)Instantiate(chosenEffect, transform.position, transform.rotation);

            GameObject effectPlayer = (GameObject)Instantiate(chosenEffect);
            effectPlayer.transform.position = transform.position;
            //effectPlayer.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));



            yield return new WaitForSeconds(loopTimeLimit);

            Destroy(effectPlayer);
            PlayEffect();
        }
    }
}