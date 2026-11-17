using SteppeBastion.Core;
using SteppeBastion.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SteppeBastion.EditorTools
{
    /// <summary>
    /// Builds Assets/Scenes/MainMenu.unity from code and puts it first in Build Settings.
    /// Running it again overwrites the scene, so hand-made changes to the scene would be lost.
    /// </summary>
    public static class MainMenuSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/" + SceneLoader.MainMenuScene + ".unity";
        private const string ArtPath = "Assets/Art/";

        private static readonly Vector2 Center = new(0.5f, 0.5f);
        private static readonly Vector2 ButtonSize = new(480f, 150f); // same 3.2 : 1 ratio as ui_button.png

        [MenuItem("Steppe Bastion/Build Main Menu Scene")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            Sprite background = LoadSprite("Backgrounds/level_01_steppe.png");
            Sprite panel = LoadSprite("Sprites/UI/ui_panel.png");
            Sprite button = LoadSprite("Sprites/UI/ui_button.png");
            Sprite akOrda = LoadSprite("Sprites/Buildings/ak_orda.png");
            Sprite berkut = LoadSprite("Sprites/Heroes/berkut.png");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            CreateEventSystem();
            Transform canvas = CreateCanvas();

            // Backdrop: the level map, darkened so the menu stands out.
            Image backdrop = UIFactory.Stretch(canvas, "Background").gameObject.AddComponent<Image>();
            backdrop.sprite = background;
            backdrop.raycastTarget = false;
            var fitter = backdrop.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = background.rect.width / background.rect.height;
            Image dim = UIFactory.Stretch(canvas, "Dim").gameObject.AddComponent<Image>();
            dim.color = new Color(0.08f, 0.05f, 0.03f, 0.35f);
            dim.raycastTarget = false;

            // Decorations: Ak Orda guarded by the berkut.
            Decoration(canvas, "AkOrda", akOrda, new Vector2(0.18f, 0.22f), 380f);
            Decoration(canvas, "Berkut", berkut, new Vector2(0.82f, 0.3f), 300f);

            // Title banner.
            Image banner = UIFactory.Image(canvas, "TitleBanner", panel, Color.white, Center, new Vector2(0f, 290f), new Vector2(1080f, 250f));
            Sliced(banner);
            UIFactory.Text(banner.transform, "Title", "STEPPE BASTION", 104, UIFactory.Gold, TextAnchor.MiddleCenter,
                Center, new Vector2(0f, 18f), new Vector2(1000f, 130f));
            UIFactory.Text(banner.transform, "Subtitle", "Defend Ak Orda, the khan's white yurt", 36, UIFactory.Cream,
                TextAnchor.MiddleCenter, Center, new Vector2(0f, -62f), new Vector2(1000f, 50f));

            // Main buttons.
            Button play = UIFactory.Button(canvas, "PlayButton", button, "PLAY", 60, Center, new Vector2(0f, 20f), ButtonSize);
            Button settings = UIFactory.Button(canvas, "SettingsButton", button, "SETTINGS", 52, Center, new Vector2(0f, -150f), ButtonSize);
            Button quit = UIFactory.Button(canvas, "QuitButton", button, "QUIT", 52, Center, new Vector2(0f, -320f), ButtonSize);
            SetVerticalNavigation(play, settings, quit);

            // Settings window. The volume and fullscreen controls are added in the Settings task.
            RectTransform window = UIFactory.Modal(canvas, "SettingsWindow", panel, new Vector2(900f, 600f));
            Transform windowPanel = window.Find("Panel");
            Sliced(windowPanel.GetComponent<Image>());
            UIFactory.Text(windowPanel, "Title", "SETTINGS", 72, UIFactory.Gold, TextAnchor.MiddleCenter,
                Center, new Vector2(0f, 200f), new Vector2(800f, 100f));
            UIFactory.Text(windowPanel, "Placeholder", "Music, sound and fullscreen options\nwill appear here.", 36,
                UIFactory.Cream, TextAnchor.MiddleCenter, Center, new Vector2(0f, 30f), new Vector2(800f, 140f));
            Button back = UIFactory.Button(windowPanel, "BackButton", button, "BACK", 48, Center, new Vector2(0f, -190f),
                ButtonSize * 0.8f);

            MainMenu menu = canvas.gameObject.AddComponent<MainMenu>();
            var so = new SerializedObject(menu);
            so.FindProperty("playButton").objectReferenceValue = play;
            so.FindProperty("settingsButton").objectReferenceValue = settings;
            so.FindProperty("quitButton").objectReferenceValue = quit;
            so.FindProperty("settingsWindow").objectReferenceValue = window.gameObject;
            so.FindProperty("settingsBackButton").objectReferenceValue = back;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            PutFirstInBuildSettings(ScenePath);
            Debug.Log($"[Steppe Bastion] Built {ScenePath} and made it the first scene in Build Settings.");
        }

        private static Sprite LoadSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + path);
            if (sprite == null)
                throw new System.IO.FileNotFoundException($"Sprite not found: {ArtPath + path}. Is Git LFS installed?");
            return sprite;
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" };
            go.transform.position = new Vector3(0f, 0f, -10f);
            var camera = go.GetComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = UIFactory.Ink;
        }

        private static void CreateEventSystem() =>
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        private static Transform CreateCanvas()
        {
            var go = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return go.transform;
        }

        private static void Decoration(Transform parent, string name, Sprite sprite, Vector2 anchor, float size)
        {
            Image image = UIFactory.Image(parent, name, sprite, Color.white, anchor, Vector2.zero, Vector2.one * size);
            image.raycastTarget = false;
        }

        private static void Sliced(Image image)
        {
            image.type = Image.Type.Sliced;
            image.preserveAspect = false;
        }

        private static void SetVerticalNavigation(params Selectable[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = items[(i - 1 + items.Length) % items.Length],
                    selectOnDown = items[(i + 1) % items.Length]
                };
                items[i].navigation = nav;
            }
        }

        private static void PutFirstInBuildSettings(string path)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.RemoveAll(s => s.path == path);
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
