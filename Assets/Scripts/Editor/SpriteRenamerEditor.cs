using UnityEngine;
using UnityEditor;

public class SpriteRenamerWindow : EditorWindow
{
    private Texture2D spritesheet;
    private string baseName = "idle_";
    private int startIndex = 0;

    [MenuItem("Window/Sprite Renamer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteRenamerWindow>("Sprite Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Renamer", EditorStyles.boldLabel);

        spritesheet = EditorGUILayout.ObjectField("Spritesheet", spritesheet, typeof(Texture2D), false) as Texture2D;
        baseName = EditorGUILayout.TextField("Base Name", baseName);
        startIndex = EditorGUILayout.IntField("Start Index", startIndex);

        if (GUILayout.Button("Rename Sprites"))
        {
            RenameSprites();
        }
    }

    private void RenameSprites()
    {
        if (spritesheet == null)
        {
            Debug.LogWarning("Spritesheet not assigned.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(spritesheet);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        Debug.Log(sprites.Length);
        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i] as Sprite;
            if (sprite != null && sprite.texture == spritesheet)
            {
                Debug.Log(sprite.name);
                string newName = baseName + (startIndex + i);
                sprite.name = newName;
                EditorUtility.SetDirty(sprite);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
