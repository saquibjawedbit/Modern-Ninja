using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//---Written by Matej Vanco 20.10.2018 dd/mm/yyyy
//---Updated by Matej Vanco 24.08.2021 dd/mm/yyyy
//---Language Localization - Source
//Contact: https://matejvanco.com/contact

[AddComponentMenu("Matej Vanco/Language Localization/Language Localization")]
public class Localization_SOURCE : MonoBehaviour 
{
    //Values are editable-----------------
    public static string GENERAL_DelimiterSymbol_Category = ">"; //This delimiter corresponds to custom categories
    public static string GENERAL_DelimiterSymbol_Key = "="; //This delimiter splits key & it's content in text
    public static string GENERAL_NewLineSymbol = "/l"; //This delimiter splits new lines
    //------------------------------------------

    public TextAsset[] LanguageFiles;
    public int SelectedLanguage = 0;

    public bool LoadLanguageOnStart = true;

    public List<string> Categories = new List<string>();

	[System.Serializable]
    public class LocalizationSelector
    {
        public enum _AssignationType {GameObjectChild, LocalizationComponent, SpecificUIText, SpecificTextMesh, SpecificTextMeshPro};
        public _AssignationType AssignationType;

        //Essentials
        public string Key;
        public string Text;
        public int Category;

        //AT - GameObjectChild
        public bool AT_FindChildByKeyName = true;
        public string AT_ChildName;
        public bool AT_UseGeneralChildsRootObject = true;
        public Transform AT_CustomChildsRootObject;

        //AT - Allowed Types
        public bool AT_UITextComponentAllowed = true;
        public bool AT_TextMeshComponentAllowed = true;
        public bool AT_TextMeshProComponentAllowed = true;

        //AT - Available Objects
        public List<GameObject> AT_FoundObjects = new List<GameObject>();

        //AT - Specific_UIText
        public Text[] AT_UITextObject;
        //AT - Specific_TextMesh
        public TextMesh[] AT_TextMeshObject;
        //AT - Specific_TextMeshPro
        public TextMeshProUGUI[] AT_TextMeshProObject;
    }
    public List<LocalizationSelector> localizationSelector = new List<LocalizationSelector>();
    public Transform AT_GameObjectChildsRoot;

    public int selectedCategory = 0;

    [System.Serializable]
    public class QuickActions
    {
        public LocalizationSelector._AssignationType assignationType;
        [Tooltip("If assignation type is set to GameObjectChild & the bool is set to True, the target text will be searched in the global Childs root object")] 
        public bool useGeneralChildsRoot = true;
        [Space]
        [Tooltip("Allow UIText component?")] public bool UITextAllowed = true;
        [Tooltip("Allow TextMesh component?")] public bool TextMeshAllowed = true;
        [Tooltip("Allow TextMeshPro component?")] public bool TextMeshProAllowed = true;
        [Space]
        [Tooltip("New specific UI Texts")] public Text[] SpecificUITexts;
        [Tooltip("New specific Text Meshes")] public TextMesh[] SpecificTextMeshes;
        [Tooltip("New specific Text Pro Meshes")] public TextMeshProUGUI[] SpecificTextProMeshes;
        [Tooltip("If enabled, the object fields above will be cleared if the QuickActions are applied to key in specific category")] public bool ClearAllPreviousTargets = true;
    }
    public QuickActions quickActions;
    //Quick actions allow user to manipulate with exist keys much faster!

    private void Awake()
    {
        if (!Application.isPlaying) return;

        Lang_RefreshKeyAssignations();
        if (LoadLanguageOnStart)
            Lang_LoadLanguage(SelectedLanguage);
    }

    #region INTERNAL METHODS

#if UNITY_EDITOR

    internal void Internal_RefreshInternalLocalization()
    {
        Categories.Clear();
        Categories.AddRange(Localization_SOURCE_Window.locAvailableCategories);
    }

    internal void Internal_AddKey(string KeyName)
    {
        foreach(Localization_SOURCE_Window.LocalizationElemenets a in Localization_SOURCE_Window.localizationElements)
        {
            if(a.Key == KeyName)
            {
                localizationSelector.Add(new LocalizationSelector() { Key = a.Key, Text = a.Text, Category = a.Category });
                return;
            }
        }
    }

#endif

    private string Internal_ConvertAndReturnText(LocalizationSelector lSelector, string[] lines)
    {
        if (lines.Length > 1)
        {
            List<string> storedFilelines = new List<string>();
            for (int i = 1; i < lines.Length; i++)
                storedFilelines.Add(lines[i]);

            foreach (string categories in Categories)
            {
                if (Internal_GetLocalizationCategory(categories) == lSelector.Category)
                {
                    foreach (string s in storedFilelines)
                    {
                        if (string.IsNullOrEmpty(s))
                            continue;
                        if (s.StartsWith(GENERAL_DelimiterSymbol_Category))
                            continue;
                        int del = s.IndexOf(GENERAL_DelimiterSymbol_Key);
                        if (del == 0 || del > s.Length)     continue;

                        string Key = s.Substring(0,del);

                        if (string.IsNullOrEmpty(Key))      continue;
                        if (Key == lSelector.Key)
                        {
                            if (s.Length < Key.Length + 1)
                                continue;
                            lSelector.Text = s.Substring(Key.Length + 1, s.Length - Key.Length - 1);
                            return lSelector.Text;
                        }
                    }
                }
            }
        }
        return "";
    }

    internal int Internal_GetLocalizationCategory(string entry)
    {
        int c = 0;
        foreach (string categ in Categories)
        {
            if (categ == entry) return c;
            c++;
        }
        return 0;
    }

    #endregion

    /// <summary>
    /// Refresh all resource objects by selected options
    /// </summary>
    public void Lang_RefreshKeyAssignations()
    {
        foreach (LocalizationSelector sel in localizationSelector)
        {
            switch (sel.AssignationType)
            {
                case LocalizationSelector._AssignationType.GameObjectChild:
                    string childName = sel.AT_ChildName;
                    if (sel.AT_FindChildByKeyName)
                        childName = sel.Key;

                    sel.AT_FoundObjects.Clear();
                    Transform root = sel.AT_UseGeneralChildsRootObject ? AT_GameObjectChildsRoot : sel.AT_CustomChildsRootObject;
                    if(!root)
                    {
                        Debug.LogError("Localization: The key '" + sel.Key + "' should have been assigned to specific childs by it's key name, but the root object is empty");
                        return;
                    }

                    foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                    {
                        if (t.name != childName) continue;

                        if (sel.AT_UITextComponentAllowed && t.GetComponent<Text>())
                            sel.AT_FoundObjects.Add(t.gameObject);
                        if (sel.AT_TextMeshComponentAllowed && t.GetComponent<TextMesh>())
                            sel.AT_FoundObjects.Add(t.gameObject);
                        if (sel.AT_TextMeshProComponentAllowed && t.GetComponent<TextMeshProUGUI>())
                            sel.AT_FoundObjects.Add(t.gameObject);
                    }
                    break;

                case LocalizationSelector._AssignationType.LocalizationComponent:
                    sel.AT_FoundObjects.Clear();
                    foreach (Localization_KEY k in FindObjectsOfType<Localization_KEY>())
                    {
                        if(k.KeyID == sel.Key) sel.AT_FoundObjects.Add(k.gameObject);
                    }
                    break;
            }
            if (sel.AssignationType == LocalizationSelector._AssignationType.GameObjectChild || sel.AssignationType == LocalizationSelector._AssignationType.LocalizationComponent)
            {
                if (sel.AT_FoundObjects.Count == 0)
                    Debug.Log("Localization: The key '" + sel.Key + "' couldn't find any child objects");
            }
        }
    }

    /// <summary>
    ///  Load language database by the selected language index
    /// </summary>
    public void Lang_LoadLanguage(int languageIndex)
    {
        if (LanguageFiles.Length <= languageIndex)
        {
            Debug.LogError("Localization: The index for language selection is incorrect! Languages count: " + LanguageFiles.Length + ", Your index: " + languageIndex);
            return;
        }
        else if (LanguageFiles[languageIndex] == null)
        {
            Debug.LogError("Localization: The language that you've selected is empty!");
            return;
        }

        foreach (LocalizationSelector sel in localizationSelector)
        {
            sel.Text = Internal_ConvertAndReturnText(sel, LanguageFiles[languageIndex].text.Split('\n')).Replace(GENERAL_NewLineSymbol, System.Environment.NewLine);

            switch(sel.AssignationType)
            {
                case LocalizationSelector._AssignationType.LocalizationComponent:
                case LocalizationSelector._AssignationType.GameObjectChild:
                    foreach (GameObject gm in sel.AT_FoundObjects)
                    {
                        if (gm.GetComponent<Text>() && sel.AT_UITextComponentAllowed)
                            gm.GetComponent<Text>().text = sel.Text;
                        else if (gm.GetComponent<TextMesh>() && sel.AT_TextMeshComponentAllowed)
                            gm.GetComponent<TextMesh>().text = sel.Text;
                        else if (gm.GetComponent<TextMeshProUGUI>() && sel.AT_TextMeshProComponentAllowed)
                            gm.GetComponent<TextMeshProUGUI>().text = sel.Text;
                    }
                    break;

                case LocalizationSelector._AssignationType.SpecificTextMesh:
                    foreach (TextMesh t in sel.AT_TextMeshObject)
                    {
                        if (t == null) continue;
                        t.text = sel.Text;
                    }
                    break;

                case LocalizationSelector._AssignationType.SpecificUIText:
                    foreach (Text t in sel.AT_UITextObject)
                    {
                        if (t == null) continue;
                        t.text = sel.Text;
                    }
                    break;

                case LocalizationSelector._AssignationType.SpecificTextMeshPro:
                    foreach (TextMeshProUGUI t in sel.AT_TextMeshProObject)
                    {
                        if (t == null) continue;
                        t.text = sel.Text;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Return exists text by the specific key input
    /// </summary>
    public string Lang_ReturnText(string KeyInput)
    {
        foreach(LocalizationSelector l in localizationSelector)
            if (l.Key == KeyInput) return l.Text;
        Debug.Log("Localization: Key '" + KeyInput + "' couldn't be found");
        return "";
    }

#if UNITY_EDITOR

    /// <summary>
    /// Load language from the selected index in the editor
    /// </summary>
    [ContextMenu("Load Language")]
    public void Lang_LoadLanguage()
    {
        Lang_RefreshKeyAssignations();
        Lang_LoadLanguage(SelectedLanguage);
    }

#endif
}
