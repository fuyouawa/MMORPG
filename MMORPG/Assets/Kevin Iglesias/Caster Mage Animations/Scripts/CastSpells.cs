using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KevinIglesias{

    public enum CastHand {RightHand, LeftHand}
    
    public class CastSpells : MonoBehaviour {

        public Transform rightHand;
        public Transform leftHand;

        public Vector3 handOffset;

        public float spellOffset;
        
        public GameObject spellPrefab;
        public GameObject castEffectPrefab;

        [HideInInspector]
        public GameObject castEffectR;
        [HideInInspector]
        public GameObject castEffectL;

        
        public void ThrowFireball(CastHand hand, float delay)
        {
            StartCoroutine(SpawnFireball(hand, delay));
        }
        
        public void ThrowNova(float delay)
        {
            StartCoroutine(SpawnNova(delay));
        }
        
        public void ThrowHealing(CastHand hand, float delay)
        {
            StartCoroutine(SpawnHealing(hand, delay));
        }
        
        public void ThrowShockwave(CastHand hand, float delay)
        {
            StartCoroutine(SpawnShockwave(hand, delay));
        }
        
        public IEnumerator SpawnFireball(CastHand hand, float delay)
        {
            Transform handT;
            
            if(hand == CastHand.RightHand)
            {
                handT = rightHand;
            }else{
                handT = leftHand;
            }
            
            yield return new WaitForSeconds(delay);
           
            GameObject newFireball = Instantiate(spellPrefab, handT.position, Quaternion.identity);
            
            newFireball.transform.rotation *= Quaternion.Euler(0,180f,0);
            
            newFireball.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            newFireball.transform.localPosition = new Vector3(newFireball.transform.localPosition.x, newFireball.transform.localPosition.y, newFireball.transform.localPosition.z+spellOffset);

            StartCoroutine(AppearFireball(newFireball.transform));
            StartCoroutine(MoveFireball(newFireball.transform));
            
            
            if(hand == CastHand.RightHand)
            {
                if(castEffectR != null)
                {
                    Destroy(castEffectR);
                }
            }else{
                if(castEffectL != null)
                {
                    Destroy(castEffectL);
                }
            }
        }
        
        public IEnumerator SpawnHealing(CastHand hand, float delay)
        {
            Transform handT;
            
            if(hand == CastHand.RightHand)
            {
                handT = rightHand;
            }else{
                handT = leftHand;
            }
            
            yield return new WaitForSeconds(delay);
           
            GameObject newHealing = Instantiate(spellPrefab, handT.position, Quaternion.identity);
            
            Transform t = newHealing.transform;
            
            t.rotation *= Quaternion.Euler(0,180f,0);
            
            t.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, t.localPosition.z);

            if(hand == CastHand.RightHand)
            {
                if(castEffectR != null)
                {
                    Destroy(castEffectR);
                }
            }else{
                if(castEffectL != null)
                {
                    Destroy(castEffectL);
                }
            }
            
            
            Vector3 startSize = t.localScale;
            
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 8f;
                t.localScale = Vector3.Lerp(startSize, Vector3.one, i);
                yield return 0;
            }
            
            yield return new WaitForSeconds(1f);

            Destroy(t.gameObject);
            
            
            
        }
        
        public void SpawnEffect(CastHand hand)
        {
            if(hand == CastHand.RightHand)
            {
                castEffectR = Instantiate(castEffectPrefab, rightHand);
                castEffectR.transform.localPosition = castEffectR.transform.localPosition+handOffset;
            
            }else{
                castEffectL = Instantiate(castEffectPrefab, leftHand);
                castEffectL.transform.localPosition = castEffectL.transform.localPosition+handOffset;
            
            }
        }
        
        
        
        public IEnumerator SpawnNova(float delay)
        {
            yield return new WaitForSeconds(delay);
           
            GameObject newNova = Instantiate(spellPrefab, new Vector3(this.transform.position.x, spellPrefab.transform.position.y, this.transform.position.z), Quaternion.identity);

            Transform t = newNova.transform;
            
            t.localScale = Vector3.zero;
            
            Vector3 startSize = t.localScale;
            
            
           
            if(castEffectR != null)
            {
                Destroy(castEffectR);
            }

            if(castEffectL != null)
            {
                Destroy(castEffectL);
            }
            
            
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 5f;
                t.localScale = Vector3.Lerp(startSize, Vector3.one*4, i);
                yield return 0;
            }
            
            Renderer r = newNova.GetComponent<Renderer>();
            
            Color initColor = r.material.GetColor("_Color");
            Color endColor = new Color(initColor.r, initColor.g, initColor.b, 0);
            
            i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 5f;
                r.material.SetColor("_Color", Color.Lerp(initColor, endColor, i));
                yield return 0;
            }
            
            Destroy(t.gameObject);
        }
        
        public IEnumerator SpawnShockwave(CastHand hand, float delay)
        {
            Transform handT;
            
            if(hand == CastHand.RightHand)
            {
                handT = rightHand;
            }else{
                handT = leftHand;
            }
            
            yield return new WaitForSeconds(delay);
           
            GameObject newShockwave = Instantiate(spellPrefab, handT.position, Quaternion.identity);

            Transform t = newShockwave.transform;
            
            t.position = new Vector3(t.position.x, 0.001f, t.position.z);
            
            if(hand == CastHand.RightHand)
            {
                if(castEffectR != null)
                {
                    Destroy(castEffectR);
                }
            }else{
                if(castEffectL != null)
                {
                    Destroy(castEffectL);
                }
            }
            
            
            
            yield return new WaitForSeconds(3f);
            
            Renderer r = newShockwave.GetComponent<Renderer>();
            
            Color initColor = r.material.GetColor("_Color");
            Color endColor = new Color(initColor.r, initColor.g, initColor.b, 0);
            
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 5f;
                r.material.SetColor("_Color", Color.Lerp(initColor, endColor, i));
                yield return 0;
            }

            

            Destroy(t.gameObject);
            
        }

        
        IEnumerator AppearFireball(Transform t)
        {
            Vector3 startSize = t.localScale;
            
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 8f;
                t.localScale = Vector3.Lerp(startSize, Vector3.one, i);
                yield return 0;
            }

        }

        IEnumerator MoveFireball(Transform t)
        {

            Vector3 initPosition = t.localPosition;
        
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 0.33f;
                t.localPosition = Vector3.Lerp(initPosition, new Vector3(initPosition.x, initPosition.y, 50), i);
                yield return 0;
            }
            
            Destroy(t.gameObject);

        }
        
    }
}
