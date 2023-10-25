#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//---Written by Matej Vanco 20.10.2018 dd/mm/yyyy
//---Updated by Matej Vanco 24.08.2021 dd/mm/yyyy
//---Language Localization - Editor

[CustomEditor(typeof(Localization_SOURCE))]
[CanEditMultipleObjects]
public class Localization_SOURCE_Editor : Editor
{
    private SerializedProperty LanguageFiles, SelectedLanguage;
    private SerializedProperty LoadLanguageOnStart;

    private SerializedProperty LocalizationSelector;

    private SerializedProperty AT_GameObjectChildsRoot;

    private SerializedProperty quickActions;

    private Localization_SOURCE l;
    private bool addKey = false;
    private bool categorySelected = false;
    private int category = 0;

    private string[] categories;

    private void OnEnable()
    {
        l = (Localization_SOURCE)target;

        LanguageFiles = serializedObject.FindProperty("LanguageFiles");
        SelectedLanguage = serializedObject.FindProperty("SelectedLanguage");
        LoadLanguageOnStart = serializedObject.FindProperty("LoadLanguageOnStart");
        LocalizationSelector = serializedObject.FindProperty("localizationSelector");
        AT_GameObjectChildsRoot = serializedObject.FindProperty("AT_GameObjectChildsRoot");
        quickActions = serializedObject.FindProperty("quickActions");

        categories = new string[l.Categories.Count + 1];
        categories[0] = "All";
        for (int i = 0; i < l.Categories.Count; i++)
            categories[i + 1] = l.Categories[i];
    }

    public override void OnInspectorGUI()
    {
        if (target == null) return;
        serializedObject.Update();

        ps();

        pv();
        pP(LanguageFiles, "Language Files", "", true);
        pP(SelectedLanguage, "Selected Language", "Currently selected language for the localization");
        ps(5);
        pP(LoadLanguageOnStart, "Load Language On Start", "Load localization after program startup");
        pve();

        ps(15);

        pv();
        if (pb("Add Key"))
        {
            Localization_SOURCE_Window.Init();
            addKey = true;
            l.Internal_RefreshInternalLocalization();
        }

        if(addKey)
        {
            pv();
            if(pb("X",GUILayout.Width(40)))
            {
                addKey = false;
                categorySelected = false;
                category = 0;
                return;
            }
            ple("From Category:");
            pv();
            for(int i = 0; i<l.Categories.Count;i++)
            {
                if(pb(l.Categories[i]))
                {
                    categorySelected = true;
                    category = i;
                    return;
                }
            }
            pve();
            if(categorySelected)
            {
                ple("Key:");
                if (pb("Add All",GUILayout.Width(120)))
                {
                    for (int i = 0; i < Localization_SOURCE_Window.localizationElements.Count; i++)
                    {
                        if (Localization_SOURCE_Window.localizationElements[i].Category != category)
                            continue;
                        l.Internal_AddKey(Localization_SOURCE_Window.localizationElements[i].Key);
                    }
                    addKey = false;
                    categorySelected = false;
                    category = 0;
                    return;
                }
                EditorGUILayout.BeginVertical("Box");
                for (int i = 0; i < Localization_SOURCE_Window.localizationElements.Count; i++)
                {
                    if (Localization_SOURCE_Window.localizationElements[i].Category != category)
                        continue;
                    bool passed = true;
                    foreach(Localization_SOURCE.LocalizationSelector sel in l.localizationSelector)
                    {
                        if(sel.Key == Localization_SOURCE_Window.localizationElements[i].Key)
                        {
                            passed = false;
                            break;
                        } 
                    }
                    if (!passed)
                        continue;
                    if (pb(Localization_SOURCE_Window.localizationElements[i].Key))
                    {
                        l.Internal_AddKey(Localization_SOURCE_Window.localizationElements[i].Key);
                        addKey = false;
                        categorySelected = false;
                        category = 0;
                        return;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            pve();
        }
        pve();

        ps(15);

        pv();
        l.selectedCategory = EditorGUILayout.Popup(new GUIContent("Filter Category: "), l.selectedCategory, categories);
        pv();
        pP(quickActions, "Quick Actions", "Edit keys with quick actions", true);
        if(serializedObject.FindProperty("quickActions").isExpanded)
        {
            if(pb($"Apply To All Keys in '{categories[l.selectedCategory]}' Category"))
            {
                foreach(Localization_SOURCE.LocalizationSelector sel in l.localizationSelector)
                {
                    if (l.selectedCategory != 0 && sel.Category != l.selectedCategory - 1) continue;

                    sel.AssignationType = l.quickActions.assignationType;
                    sel.AT_UseGeneralChildsRootObject = l.quickActions.useGeneralChildsRoot;
                    sel.AT_UITextComponentAllowed = l.quickActions.UITextAllowed;
                    sel.AT_TextMeshComponentAllowed = l.quickActions.TextMeshAllowed;
                    sel.AT_TextMeshProComponentAllowed = l.quickActions.TextMeshProAllowed;

                    if (l.quickActions.ClearAllPreviousTargets)
                    {
                        sel.AT_TextMeshObject = null;
                        sel.AT_UITextObject = null;
                        sel.AT_TextMeshProObject = null;
                    }

                    if (l.quickActions.SpecificUITexts.Length > 0)
                        sel.AT_UITextObject = l.quickActions.SpecificUITexts;
                    if (l.quickActions.SpecificTextMeshes.Length > 0)
                        sel.AT_TextMeshObject = l.quickActions.SpecificTextMeshes;
                    if (l.quickActions.SpecificTextProMeshes.Length > 0)
                        sel.AT_TextMeshProObject = l.quickActions.SpecificTextProMeshes;
                }
            }
        }
        pve();
        ps();

        pv();
        pP(AT_GameObjectChildsRoot, "GameObject Childs Root", "Starting root for keys containing 'GameObjectChild' assignation type");
        if (l.localizationSelector.Count > 0) DrawList();
        pve();
        pve();
    }

    private void DrawList()
    {
        for (int i = 0; i < l.localizationSelector.Count; i++)
        {
            if (l.selectedCategory != 0 && l.localizationSelector[i].Category != l.selectedCategory - 1) continue;

            pv();
            SerializedProperty item = LocalizationSelector.GetArrayElementAtIndex(i);
            GUILayout.BeginHorizontal();

            pP(item, l.localizationSelector[i].Key);
            if (pb("X", GUILayout.Width(40)))
            {
                l.localizationSelector.RemoveAt(i);
                return;
            }
            GUILayout.EndHorizontal();
            if (!item.isExpanded)
            {
                pve();
                continue;
            }

            Localization_SOURCE.LocalizationSelector sec = l.localizationSelector[i];
            
            ps(5);

            EditorGUI.indentLevel += 1;
            GUILayout.BeginHorizontal("Box");
            ple("Key: "+ sec.Key, true);
            pl("Category: " + l.Categories[sec.Category]);
            GUILayout.EndHorizontal();

            ps();

            pP(item.FindPropertyRelative("AssignationType"), "Assignation Type");

            switch(sec.AssignationType)
            {
                case Localization_SOURCE.LocalizationSelector._AssignationType.GameObjectChild:
                    pv();
                    pP(item.FindPropertyRelative("AT_FindChildByKeyName"), "Find Child By Key Name", "If enabled, the system will find the child of the selected component type [below] by the key name");
                    if (!sec.AT_FindChildByKeyName)
                        pP(item.FindPropertyRelative("AT_ChildName"), "Child Name");
                    else
                        EditorGUILayout.HelpBox("Object with name '"+sec.Key+"' should exist", MessageType.None);
                    pve();
                    ps(3);

                    pv();
                    pP(item.FindPropertyRelative("AT_UseGeneralChildsRootObject"), "Use General Childs Root Object");
                    if(!sec.AT_UseGeneralChildsRootObject)
                        pP(item.FindPropertyRelative("AT_CustomChildsRootObject"), "Custom Childs Root Object");
                    pve();

                    ps(3);

                    pv();
                    pP(item.FindPropertyRelative("AT_UITextComponentAllowed"), "UIText Component Allowed", "If disabled, objects with UI Text component will be ignored");
                    pP(item.FindPropertyRelative("AT_TextMeshComponentAllowed"), "TextMesh Component Allowed", "If disabled, objects with Text Mesh component will be ignored");
                    pP(item.FindPropertyRelative("AT_TextMeshProComponentAllowed"), "TextMeshPro [UGUI] Component Allowed", "If disabled, objects with Text Mesh Pro UGUI component will be ignored");

                    pve();
                    break;

                case Localization_SOURCE.LocalizationSelector._AssignationType.LocalizationComponent:
                    pv();
                    EditorGUILayout.HelpBox("Object with Localization_KEY component should have an ID '" + sec.Key + "'", MessageType.None);
                    pve();
                    ps(3);

                    pv();
                    pP(item.FindPropertyRelative("AT_UITextComponentAllowed"), "UIText Component Allowed", "If disabled, objects with UI Text component will be ignored");
                    pP(item.FindPropertyRelative("AT_TextMeshComponentAllowed"), "TextMesh Component Allowed", "If disabled, objects with Text Mesh component will be ignored");
                    pP(item.FindPropertyRelative("AT_TextMeshProComponentAllowed"), "TextMeshPro [UGUI] Component Allowed", "If disabled, objects with Text Mesh Pro component will be ignored");
                    pve();
                    break;

                case Localization_SOURCE.LocalizationSelector._AssignationType.SpecificUIText:
                    pP(item.FindPropertyRelative("AT_UITextObject"), "Specific UI Text", "Assign specific UI Text objects", true);
                    break;

                case Localization_SOURCE.LocalizationSelector._AssignationType.SpecificTextMesh:
                    pP(item.FindPropertyRelative("AT_TextMeshObject"), "Specific Text Mesh", "Assign specific Text Mesh objects", true);
                    break;

                case Localization_SOURCE.LocalizationSelector._AssignationType.SpecificTextMeshPro:
                    pP(item.FindPropertyRelative("AT_TextMeshProObject"), "Specific Text Mesh Pro [UGUI]", "Assign specific Text Mesh Pro UGUI objects", true);
                    break;
            }
           

            EditorGUI.indentLevel -= 1;
            pve();
        }
    }

    #region LayoutShortcuts

    private void pl(string text)
    {
        GUILayout.Label(text);
    }
    private void ple(string text, bool bold = false)
    {
        if (bold)
        {
            string add = "<b>";
            add += text + "</b>";
            text = add;
        }
        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.normal.textColor = Color.white;
        EditorGUILayout.LabelField(text, style);
    }
    private void ps(float space = 10)
    {
        GUILayout.Space(space);
    }
    private void pP(SerializedProperty p, string Text, string ToolTip = "", bool includeChilds = false)
    {
        EditorGUILayout.PropertyField(p, new GUIContent(Text, ToolTip), includeChilds);
        serializedObject.ApplyModifiedProperties();
    }
    private void pv()
    {
        GUILayout.BeginVertical("Box");
    }
    private void pve()
    {
        GUILayout.EndVertical();
    }
    private bool pb(string mess, GUILayoutOption opt = null)
    {
        if(opt == null)     return GUILayout.Button(mess);
        else                return GUILayout.Button(mess, opt);
    }

    #endregion
}
#endif