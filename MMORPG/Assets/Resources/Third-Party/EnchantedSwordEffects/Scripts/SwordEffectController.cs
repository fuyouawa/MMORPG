using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;



public class SwordEffectController : MonoBehaviour
{
    [System.Serializable]
    public class WeaponEffect{
        public VisualEffect vfx;
        public Material material;
        public Color lightColor;

    }

    public WeaponEffect effect1;
    public WeaponEffect effect2;
    public WeaponEffect effect3;
    public WeaponEffect effect4;
    public WeaponEffect effect5;
    public WeaponEffect effect6;

    public float offset = 0.439f;

    VisualEffect currentVfx;

    public void ActivateVFX1(){
        ActivateVFX(effect1);
    }
    public void ActivateVFX2(){
        ActivateVFX(effect2);
    }
    public void ActivateVFX3(){
        ActivateVFX(effect3);
    }
    public void ActivateVFX4(){
        ActivateVFX(effect4);
    }
    public void ActivateVFX5(){
        ActivateVFX(effect5);
    }
    public void ActivateVFX6(){
        ActivateVFX(effect6);
    }

    private void ActivateVFX(WeaponEffect weaponEffect){
        RemoveExistingEffects();
        Renderer renderer = transform.GetComponent<Renderer>();
        renderer.material = weaponEffect.material;
        for(int i = transform.childCount - 1; i >= 0; i--){
            if(renderer.material != null){
                if(transform.GetChild(i).GetComponent<Renderer>() != null){
                    renderer = transform.GetChild(i).GetComponent<Renderer>();
                    renderer.material = weaponEffect.material;
                }
            }
            if(transform.GetChild(i).GetComponent<Light>() != null){
                Light light = transform.GetChild(i).GetComponent<Light>();
                light.color = weaponEffect.lightColor;

            }
        }
        currentVfx = Instantiate(weaponEffect.vfx, new Vector3(0, 0, 0), transform.rotation, transform);
        currentVfx.transform.localPosition = new Vector3(0, offset, 0);
        currentVfx.Play();


    }

    public void Update(){
        if(currentVfx != null){
            currentVfx.transform.rotation = transform.rotation;
        }
    }

    public void RemoveExistingEffects(){
        for(int i = transform.childCount - 1; i >= 0; i--){
            if(transform.GetChild(i).GetComponent<VisualEffect>() != null){
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }


}
