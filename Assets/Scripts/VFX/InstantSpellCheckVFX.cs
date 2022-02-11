using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VFX
{
    public class InstantSpellCheckVFX : MonoBehaviour
    {
        private Vignette vignette;
        [SerializeField] private Camera camera;
        [SerializeField] private Color success = Color.green;
        [SerializeField] private Color other = Color.red;
        private void Start()
        {
            GetComponent<Volume>().profile.TryGet<Vignette>(out vignette);
            vignette.active = false;
            YoumuController.instance.onInstantSpellCheck += () =>
            {
                StartCoroutine(StartEffect());
            };
        }
        private IEnumerator StartEffect()
        {
            vignette.active = true;
            var effectKeepFrame = 30;
            while (vignette.intensity.value < 1 || effectKeepFrame>0)
            {
                if (vignette.intensity.value<1)
                {
                    vignette.center.value = camera.WorldToViewportPoint(YoumuController.instance.transform.position);
                    vignette.intensity.value += 1 / 30.0f;
                }
                else if (effectKeepFrame>0)
                {
                    effectKeepFrame -= 1;
                }
              
                if (Health.instance.invincible)
                {
                    StartCoroutine(ProgressiveReset(success));
                    yield break;
                }

                yield return null;
            }
            

            StartCoroutine(ProgressiveReset(other));

        }

        private IEnumerator ProgressiveReset(Color recoveryColor)
        {
            vignette.color.value = recoveryColor;
            while (vignette.intensity.value > 0)
            {
                vignette.intensity.value -= 0.005f;
                yield return null;
            }
            vignette.intensity.value = 0f;
            vignette.color.value = other;
            vignette.active = false;
        }
        
        
    }
    
    
}