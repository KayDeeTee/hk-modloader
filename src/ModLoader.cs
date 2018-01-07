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

        public static Selectable[] modArray;
        public static Selectable back;

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

        public static void dataDump(GameObject go, int depth)
        {
            Modding.Logger.Log(new String('-', depth) + go.name);
            foreach (Component comp in go.GetComponents<Component>())
            {
                switch (comp.GetType().ToString())
                {
                    case "UnityEngine.RectTransform":
                        Modding.Logger.Log(new String('+', depth) + comp.GetType().ToString() + " : " + ((RectTransform)comp).sizeDelta + ", " + ((RectTransform)comp).anchoredPosition + ", " + ((RectTransform)comp).anchorMin + ", " + ((RectTransform)comp).anchorMax);
                        break;
                    case "UnityEngine.UI.Text":
                        Modding.Logger.Log(new String('+', depth) + comp.GetType().ToString() + " : " + ((Text)comp).text);
                        break;
                    default:
                        Modding.Logger.Log(new String('+', depth) + comp.GetType().ToString());
                        break;
                }                  
            }
            foreach (Transform child in go.transform)
            {
                dataDump(child.gameObject, depth + 1);
            }
        }


        public static Sprite nullSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadRawTextureData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
        }

        public static Sprite createSprite(byte[] data, int x, int y, int w, int h)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(data);
            tex.anisoLevel = 0;
            int width = tex.width;
            int height = tex.height;
            return Sprite.Create(tex, new Rect(x, y, w, h), Vector2.zero);
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

                back = modMenuScreen.defaultHighlight.FindSelectableOnUp();
                GameObject item = uim.videoMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject;
                GameObject.DestroyImmediate(item.GetComponent<MenuOptionHorizontal>());
                GameObject.DestroyImmediate(item.GetComponent<MenuSetting>());
                GameObject.DestroyImmediate(modMenuScreen.content.GetComponent<VerticalLayoutGroup>());
                GameObject.Destroy(modMenuScreen.defaultHighlight.gameObject.transform.parent.gameObject);

                modArray = new Selectable[mods.Count];

                for (int i = 0; i < mods.Count; i++)
                {
                    GameObject menuItem = GameObject.Instantiate(item.gameObject);
                    modArray[i] = menuItem.AddComponent<FauxMenuOptionHorizontal>();
                    modArray[i].navigation = Navigation.defaultNavigation;
                    //dataDump(modArray[i].gameObject, 1);                    

                    ((FauxMenuOptionHorizontal)modArray[i]).optionList = new string[] { "On", "Off" };
                    ((FauxMenuOptionHorizontal)modArray[i]).optionText = modArray[i].gameObject.transform.GetChild(1).GetComponent<Text>();
                    ((FauxMenuOptionHorizontal)modArray[i]).selectedOptionIndex = 0;
                    ((FauxMenuOptionHorizontal)modArray[i]).localizeText = false;
                    ((FauxMenuOptionHorizontal)modArray[i]).sheetTitle = mods[i];
                    ((FauxMenuOptionHorizontal)modArray[i]).modName = mods[i];

                    GameObject.DestroyImmediate(modArray[i].transform.FindChild("Label").GetComponent<AutoLocalizeTextUI>());
                    modArray[i].transform.FindChild("Label").GetComponent<Text>().text = mods[i];

                    ((FauxMenuOptionHorizontal)modArray[i]).leftCursor = modArray[i].transform.FindChild("CursorLeft").GetComponent<Animator>();
                    ((FauxMenuOptionHorizontal)modArray[i]).rightCursor = modArray[i].transform.FindChild("CursorRight").GetComponent<Animator>();

                    modArray[i].gameObject.name = mods[i];

                    RectTransform rt = menuItem.GetComponent<RectTransform>();

                    rt.SetParent(modMenuScreen.content.transform);
                    rt.localScale = new Vector3(2, 2, 2);

                    rt.sizeDelta = new Vector2(960, 120);
                    rt.anchoredPosition = new Vector2(0, (766/2) - 90 - (150 * i));
                    rt.anchorMin = new Vector2(0.5f, 1.0f);
                    rt.anchorMax = new Vector2(0.5f, 1.0f);

                    //Image img = menuItem.AddComponent<Image>();
                    //img.sprite = nullSprite();


                    ((FauxMenuOptionHorizontal)modArray[i]).cancelAction = CancelAction.DoNothing;
                   

                    //AutoLocalizeTextUI localizeUI = modArray[i].GetComponent<AutoLocalizeTextUI>();
                    //modArray[i].transform.GetChild(0).GetComponent<Text>().text = mods[i];
                    //GameObject.Destroy(localizeUI);
                }

                Navigation[] navs = new Navigation[modArray.Length];
                for (int i = 0; i < ModLoader.modArray.Length; i++)
                {
                    navs[i] = new Navigation();
                    navs[i].mode = Navigation.Mode.Explicit;

                    if (i == 0)
                    {
                        navs[i].selectOnUp = ModLoader.back;
                    }
                    else
                    {
                        navs[i].selectOnUp = ModLoader.modArray[i - 1];
                    }
                    if (i == ModLoader.modArray.Length - 1)
                    {
                        navs[i].selectOnDown = ModLoader.back;
                    }
                    else
                    {
                        navs[i].selectOnDown = ModLoader.modArray[i + 1];
                    }

                    ModLoader.modArray[i].navigation = navs[i];
                }
                ModLoader.modMenuScreen.defaultHighlight = ModLoader.modArray[0];
                Navigation nav2 = ModLoader.back.navigation;
                nav2.selectOnUp = ModLoader.modArray[ModLoader.modArray.Length - 1];
                nav2.selectOnDown = ModLoader.modArray[0];
                ModLoader.back.navigation = nav2;

                ((MenuButton)back).cancelAction = CancelAction.DoNothing;
                EventTrigger back_events = back.gameObject.GetComponent<EventTrigger>();

                back_events.triggers = new List<EventTrigger.Entry>();

                EventTrigger.Entry back_submit = new EventTrigger.Entry();
                back_submit.eventID = EventTriggerType.Submit;
                back_submit.callback.AddListener((data) => { fauxUIM.UIquitModMenu(); });
                back_events.triggers.Add(back_submit);

                EventTrigger.Entry back_click = new EventTrigger.Entry();
                back_click.eventID = EventTriggerType.PointerClick;
                back_click.callback.AddListener((data) => { fauxUIM.UIquitModMenu(); });
                back_events.triggers.Add(back_click);


                //SETUP MOD MENU

                //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
                EventTrigger events = modButton.gameObject.GetComponent<EventTrigger>();

                events.triggers = new List<EventTrigger.Entry>();

                EventTrigger.Entry submit = new EventTrigger.Entry();
                submit.eventID = EventTriggerType.Submit;
                submit.callback.AddListener((data) => { fauxUIM.UIloadModMenu(); });
                events.triggers.Add(submit);

                EventTrigger.Entry click = new EventTrigger.Entry();
                click.eventID = EventTriggerType.PointerClick;
                click.callback.AddListener((data) => { fauxUIM.UIloadModMenu(); });
                events.triggers.Add(click);

                //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
            }
        }

    }
}
