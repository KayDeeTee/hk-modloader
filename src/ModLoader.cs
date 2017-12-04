using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;
using System.Collections;
namespace ModLoader
{
    public class ModLoader : Mod
    {

        private static string version = "0.0.1";
        private static UIManager uim;
        private static FauxUIManager fauxUIM;
        public static MenuScreen modMenuScreen;

        public override string GetVersion()
        {
            return version;
        }

        public override bool IsCurrent()
        {
            return true;
        }

        public override void Initialize()
        {
            Log("Initializing ModLoader");

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += sceneLoaded;
            GameObject go = new GameObject();
            fauxUIM = go.AddComponent<FauxUIManager>();

            Log("Initialized ModLoader");
        }

        public void dataDump(GameObject go, int depth)
        {
            Log(new String('-', depth) + go.name);
            foreach (Component comp in go.GetComponents<Component>())
            {
                Log(new String('+', depth) + comp.GetType().ToString());
            }
            foreach (Transform child in go.transform)
            {
                dataDump(child.gameObject, depth + 1);
            }
        }

        void sceneLoaded(Scene scene, LoadSceneMode lsm)
        {
            if (uim == null)
            {
                uim = UIManager.instance;

                //ADD MODS TO OPTIONS MENU
                MenuButton defButton = (MenuButton)uim.optionsMenuScreen.defaultHighlight;
                MenuButton modButton = GameObject.Instantiate(defButton.gameObject).GetComponent<MenuButton>();

                Navigation nav = modButton.navigation;
                nav.selectOnUp = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();
                nav.selectOnDown = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();
                modButton.navigation = nav;

                nav = modButton.FindSelectableOnUp().navigation;
                nav.selectOnDown = modButton;
                modButton.FindSelectableOnUp().navigation = nav;

                nav = modButton.FindSelectableOnDown().navigation;
                nav.selectOnUp = modButton;
                modButton.FindSelectableOnDown().navigation = nav;

                modButton.name = "Mods";

                modButton.transform.SetParent(modButton.FindSelectableOnUp().transform.parent);

                modButton.transform.localPosition = new Vector2(0, -120);
                modButton.transform.localScale = modButton.FindSelectableOnUp().transform.localScale;

                GameObject.Destroy(modButton.gameObject.GetComponent<AutoLocalizeTextUI>());
                modButton.gameObject.transform.FindChild("Text").GetComponent<Text>().text = "Mods";
                //ADD MODS TO OPTIONS MENU

                //SETUP MOD MENU
                GameObject go = GameObject.Instantiate(uim.optionsMenuScreen.gameObject);
                modMenuScreen = go.GetComponent<MenuScreen>();
                modMenuScreen.title = modMenuScreen.gameObject.transform.FindChild("Title").GetComponent<CanvasGroup>();
                modMenuScreen.topFleur = modMenuScreen.gameObject.transform.FindChild("TopFleur").GetComponent<Animator>();
                modMenuScreen.content = modMenuScreen.gameObject.transform.FindChild("Content").GetComponent<CanvasGroup>();

                modMenuScreen.title.gameObject.GetComponent<Text>().text = "Mods";
                GameObject.Destroy(modMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>());

                modMenuScreen.transform.SetParent(uim.optionsMenuScreen.gameObject.transform.parent);
                modMenuScreen.transform.localPosition = uim.optionsMenuScreen.gameObject.transform.localPosition;
                modMenuScreen.transform.localScale = uim.optionsMenuScreen.gameObject.transform.localScale;

                List<string> mods = ModHooks.Instance.LoadedMods;
                //modMenuScreen.content = modMenuScreen.gameObject.transform.GetChild()
                modMenuScreen.defaultHighlight = modMenuScreen.content.gameObject.transform.GetChild(0).GetChild(0).GetComponent<MenuButton>();
                GameObject.Destroy(modMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject);
                GameObject.Destroy(modMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject);
                GameObject.Destroy(modMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject);
                GameObject.Destroy(modMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject);
                GameObject.Destroy(modMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject);

                Selectable deleteAfter = modMenuScreen.defaultHighlight;

                Selectable[] modArray = new Selectable[mods.Count];

                for (int i = 0; i < mods.Count; i++)
                {
                    modArray[i] = GameObject.Instantiate(deleteAfter.gameObject.transform.parent).GetChild(0).GetComponent<MenuButton>();
                    modArray[i].gameObject.transform.parent.SetParent(modMenuScreen.content.transform);

                    modArray[i].gameObject.transform.parent.localScale = deleteAfter.transform.parent.localScale;
                    modArray[i].gameObject.transform.localScale = deleteAfter.transform.localScale;

                    modArray[i].gameObject.transform.parent.localPosition = new Vector2(0, 0);
                    modArray[i].gameObject.transform.localPosition = new Vector2(0, 150);

                    ((MenuButton)modArray[i]).cancelAction = (CancelAction)14;

                    AutoLocalizeTextUI localizeUI = modArray[i].GetComponent<AutoLocalizeTextUI>();
                    localizeUI.textField.text = mods[i];
                    GameObject.Destroy(localizeUI);
                }

                for (int i = 0; i < mods.Count; i++)
                {
                    Navigation navi = modArray[i].navigation;
                    if (i == 0)
                        navi.selectOnUp = deleteAfter.FindSelectableOnUp();
                    else
                        navi.selectOnUp = modArray[i - 1];
                    if (i == mods.Count - 1)
                        navi.selectOnDown = deleteAfter.FindSelectableOnUp();
                    else
                        navi.selectOnDown = modArray[i + 1];
                    modArray[i].navigation = navi;
                }

                modMenuScreen.defaultHighlight = modArray[0];
                Navigation nav2 = modArray[0].FindSelectableOnUp().navigation;
                nav2.selectOnUp = modArray[modArray.Length - 1];
                nav2.selectOnDown = modArray[0];
                modArray[0].FindSelectableOnUp().navigation = nav2;
                ((MenuButton)modArray[0].FindSelectableOnUp()).cancelAction = (CancelAction)14;

                GameObject.Destroy(deleteAfter.gameObject);

                dataDump(modMenuScreen.gameObject, 1);

                //dataDump(uim.optionsMenuScreen.gameObject, 1);
                //SETUP MOD MENU


                //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
                EventTrigger events = modButton.gameObject.GetComponent<EventTrigger>();

                events.triggers = new List<EventTrigger.Entry>();

                EventTrigger.Entry submit = new EventTrigger.Entry();
                submit.eventID = EventTriggerType.Submit;
                submit.callback.AddListener((data) => { fauxUIM.UIloadModMenu(); });
                events.triggers.Add(submit);
                submit.eventID = EventTriggerType.PointerClick;
                events.triggers.Add(submit);

                //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
            }
        }

    }
}
