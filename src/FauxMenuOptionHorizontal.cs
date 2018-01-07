using System;
using Language;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class FauxMenuOptionHorizontal : MenuSelectable, IEventSystemHandler, IPointerClickHandler, IPointerEnterHandler, IMoveHandler
    {
        private new void Awake()
        {
            this.gm = GameManager.instance;
        }
        private new void OnEnable()
        {
            this.gm.RefreshLanguageText += this.UpdateText;
            this.UpdateText();
        }
        private new void OnDisable()
        {
            this.gm.RefreshLanguageText -= this.UpdateText;
        }
        public new void OnMove(AxisEventData move)
        {
            if (move.moveDir == MoveDirection.Left)
            {
                this.DecrementOption();
                this.uiAudioPlayer.PlaySlider();
            }
            else if (move.moveDir == MoveDirection.Right)
            {
                this.IncrementOption();
                this.uiAudioPlayer.PlaySlider();
            }
            else
            {
                base.OnMove(move);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.IncrementOption();
                this.uiAudioPlayer.PlaySlider();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.DecrementOption();
                this.uiAudioPlayer.PlaySlider();
            }
        }
        public void SetOptionList(string[] optionList)
        {
            this.optionList = optionList;
        }
        public string GetSelectedOptionText()
        {
            if (this.localizeText)
            {
                return Language.Language.Get(this.optionList[this.selectedOptionIndex].ToString(), this.sheetTitle);
            }
            return this.optionList[this.selectedOptionIndex].ToString();
        }
        public string GetSelectedOptionTextRaw()
        {
            return this.optionList[this.selectedOptionIndex].ToString();
        }
        public virtual void SetOptionTo(int optionNumber)
        {
            if (optionNumber >= 0 && optionNumber < this.optionList.Length)
            {
                this.selectedOptionIndex = optionNumber;
                this.UpdateText();
            }
            else
            {
                Debug.LogErrorFormat("{0} - Trying to select an option outside the list size (index: {1} listsize: {2})", new object[]
				{
					base.name,
					optionNumber,
					this.optionList.Length
				});
            }
        }
        protected virtual void UpdateText()
        {
            if (this.optionList != null && this.optionText != null)
            {
                try
                {
                    if (this.localizeText)
                    {
                        this.optionText.text = Language.Language.Get(this.optionList[this.selectedOptionIndex].ToString(), this.sheetTitle);
                    }
                    else
                    {
                        this.optionText.text = this.optionList[this.selectedOptionIndex].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(string.Concat(new object[]
					{
						this.optionText.text,
						" : ",
						this.optionList,
						" : ",
						this.selectedOptionIndex,
						" ",
						ex
					}));
                }
                this.optionText.GetComponent<FixVerticalAlign>().AlignText();
            }
        }
        protected void UpdateSetting()
        {
            Modding.Logger.Log(modName + " set option to " + optionText.text);
        }
        protected void DecrementOption()
        {
            if (this.selectedOptionIndex > 0)
            {
                this.selectedOptionIndex--;
                if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
                {
                    this.UpdateSetting();
                }
                this.UpdateText();
            }
            else if (this.selectedOptionIndex == 0)
            {
                this.selectedOptionIndex = this.optionList.Length - 1;
                if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
                {
                    this.UpdateSetting();
                }
                this.UpdateText();
            }
        }
        protected void IncrementOption()
        {
            if (this.selectedOptionIndex >= 0 && this.selectedOptionIndex < this.optionList.Length - 1)
            {
                this.selectedOptionIndex++;
                if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
                {
                    this.UpdateSetting();
                }
                this.UpdateText();
            }
            else if (this.selectedOptionIndex == this.optionList.Length - 1)
            {
                this.selectedOptionIndex = 0;
                if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
                {
                    this.UpdateSetting();
                }
                this.UpdateText();
            }
        }
        [Header("Option List Settings")]
        public Text optionText;
        public string[] optionList;
        public int selectedOptionIndex;
        [Header("Interaction")]
        public MenuOptionHorizontal.ApplyOnType applySettingOn;
        [Header("Localization")]
        public bool localizeText;
        public string sheetTitle;

        public string modName;

        protected GameManager gm;
        public enum ApplyOnType
        {
            Scroll,
            Submit
        }
    }
}
