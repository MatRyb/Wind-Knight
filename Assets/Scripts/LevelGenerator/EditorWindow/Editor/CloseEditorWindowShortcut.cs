using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class CloseEditorWindowShortcut
{
    [Shortcut("Window/Close", KeyCode.W, ShortcutModifiers.Control)]
    private static void CloseTab(ShortcutArguments args)
    {
        if (EditorWindow.focusedWindow != null)
        {
            EditorWindow.focusedWindow.Close();
        }
    }
}