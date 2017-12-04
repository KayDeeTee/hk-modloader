using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModLoader
{
    class FauxUIManager : MonoBehaviour
    {
        GameManager gm;

        public void Start()
        {
            gm = GameManager.instance;
            DontDestroyOnLoad(this);
        }

        public IEnumerator ShowMenu(MenuScreen menu)
        {
            gm.inputHandler.StopUIInput();
            if (menu.screenCanvasGroup != null)
            {
                this.StartCoroutine(this.FadeInCanvasGroup(menu.screenCanvasGroup));
            }
            if (menu.title != null)
            {
                this.StartCoroutine(this.FadeInCanvasGroup(menu.title));
            }
            if (menu.topFleur != null)
            {
                yield return this.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
                menu.topFleur.ResetTrigger("hide");
                menu.topFleur.SetTrigger("show");
            }
            yield return this.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
            if (menu.content != null)
            {
                this.StartCoroutine(this.FadeInCanvasGroup(menu.content));
            }
            if (menu.controls != null)
            {
                this.StartCoroutine(this.FadeInCanvasGroup(menu.controls));
            }
            if (menu.bottomFleur != null)
            {
                menu.bottomFleur.ResetTrigger("hide");
                menu.bottomFleur.SetTrigger("show");
            }
            yield return this.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
            gm.inputHandler.StartUIInput();
            yield return null;
            menu.HighlightDefault();
            yield break;
        }

        public IEnumerator FadeInCanvasGroup(CanvasGroup cg)
        {
            float loopFailsafe = 0f;
            cg.alpha = 0f;
            cg.gameObject.SetActive(true);
            while (cg.alpha < 1f)
            {
                cg.alpha += Time.unscaledDeltaTime * 3.2f;
                loopFailsafe += Time.unscaledDeltaTime;
                if (cg.alpha >= 0.95f)
                {
                    cg.alpha = 1f;
                    break;
                }
                if (loopFailsafe >= 2f)
                {
                    break;
                }
                yield return null;
            }
            cg.alpha = 1f;
            cg.interactable = true;
            cg.gameObject.SetActive(true);
            yield return null;
            yield break;
        }

        public IEnumerator FadeOutCanvasGroup(CanvasGroup cg)
        {
            float loopFailsafe = 0f;
            cg.interactable = false;
            while (cg.alpha > 0.05f)
            {
                cg.alpha -= Time.unscaledDeltaTime * 3.2f;
                loopFailsafe += Time.unscaledDeltaTime;
                if (cg.alpha <= 0.05f)
                {
                    break;
                }
                if (loopFailsafe >= 2f)
                {
                    break;
                }
                yield return null;
            }
            cg.alpha = 0f;
            cg.gameObject.SetActive(false);
            yield return null;
            yield break;
        }

        public IEnumerator HideMenu(MenuScreen menu)
        {
            gm.inputHandler.StopUIInput();
            if (menu.title != null)
            {
                this.StartCoroutine(this.FadeOutCanvasGroup(menu.title));
                yield return this.StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
            }
            if (menu.topFleur != null)
            {
                menu.topFleur.ResetTrigger("show");
                menu.topFleur.SetTrigger("hide");
                yield return this.StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
            }
            if (menu.content != null)
            {
                this.StartCoroutine(this.FadeOutCanvasGroup(menu.content));
            }
            if (menu.controls != null)
            {
                this.StartCoroutine(this.FadeOutCanvasGroup(menu.controls));
            }
            if (menu.bottomFleur != null)
            {
                menu.bottomFleur.ResetTrigger("show");
                menu.bottomFleur.SetTrigger("hide");
                yield return this.StartCoroutine(gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
            }
            if (menu.screenCanvasGroup != null)
            {
                yield return this.StartCoroutine(this.FadeOutCanvasGroup(menu.screenCanvasGroup));
            }
            gm.inputHandler.StartUIInput();
            yield break;
        }

        public void UIloadModMenu()
        {
            base.StartCoroutine(this.loadModMenu());
        }

        public IEnumerator loadModMenu()
        {
            Modding.Logger.Log("Loading Mod Menu");
            yield return this.StartCoroutine(HideMenu(UIManager.instance.optionsMenuScreen));
            yield return this.StartCoroutine(ShowMenu(ModLoader.modMenuScreen));
            gm.inputHandler.StartUIInput();
            yield break;
        }

        public IEnumerator quitModMenu()
        {
            Modding.Logger.Log("Quitting Mod Menu");
            yield return this.StartCoroutine(HideMenu(ModLoader.modMenuScreen));
            yield return this.StartCoroutine(this.ShowMenu(UIManager.instance.optionsMenuScreen));
            GameManager.instance.inputHandler.StartUIInput();
            yield break;
        }


    }
}
