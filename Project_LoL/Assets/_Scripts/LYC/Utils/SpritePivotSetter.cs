#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

public class SpritePivotSetter : EditorWindow
{
	private SpriteAlignment alignment = SpriteAlignment.Center;
	private Vector2 customPivot = new Vector2(0.5f, 0.5f);

	[MenuItem("Tools/Sprite Pivot Setter")]
	public static void ShowWindow()
	{
		GetWindow<SpritePivotSetter>("Sprite Pivot Setter");
	}

	void OnGUI()
	{
		GUILayout.Label("선택한 텍스처의 모든 스프라이트 피봇 일괄 설정", EditorStyles.boldLabel);

		alignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot", alignment);

		if (alignment == SpriteAlignment.Custom)
		{
			customPivot = EditorGUILayout.Vector2Field("Custom Pivot", customPivot);
		}

		if (GUILayout.Button("Apply to Selected Textures"))
		{
			ApplyPivotToSelected();
		}
	}

	void ApplyPivotToSelected()
	{
		int count = 0;

		foreach (Object obj in Selection.objects)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

			if (importer == null || importer.spriteImportMode != SpriteImportMode.Multiple)
				continue;

			var factory = new SpriteDataProviderFactories();
			factory.Init();
			var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
			dataProvider.InitSpriteEditorDataProvider();

			var spriteRects = dataProvider.GetSpriteRects();

			foreach (var rect in spriteRects)
			{
				rect.alignment = alignment;
				rect.pivot = alignment == SpriteAlignment.Custom
					? customPivot
					: GetPivotValue(alignment);
			}

			dataProvider.SetSpriteRects(spriteRects);
			dataProvider.Apply();

			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			count++;
		}

		Debug.Log($"{count}개 텍스처의 스프라이트 피봇 설정 완료!");
		EditorUtility.DisplayDialog("완료", $"{count}개 텍스처 처리 완료", "OK");
	}

	Vector2 GetPivotValue(SpriteAlignment align)
	{
		return align switch
		{
			SpriteAlignment.TopLeft => new Vector2(0f, 1f),
			SpriteAlignment.TopCenter => new Vector2(0.5f, 1f),
			SpriteAlignment.TopRight => new Vector2(1f, 1f),
			SpriteAlignment.LeftCenter => new Vector2(0f, 0.5f),
			SpriteAlignment.Center => new Vector2(0.5f, 0.5f),
			SpriteAlignment.RightCenter => new Vector2(1f, 0.5f),
			SpriteAlignment.BottomLeft => new Vector2(0f, 0f),
			SpriteAlignment.BottomCenter => new Vector2(0.5f, 0f),
			SpriteAlignment.BottomRight => new Vector2(1f, 0f),
			_ => new Vector2(0.5f, 0.5f)
		};
	}
}
#endif