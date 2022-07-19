using UnityEditor;
namespace KingdomOfNight
{
    public partial class EditorSceneSwitcher
    {
#if UNITY_EDITOR
        [MenuItem("Scenes/Backrooms Levels/Escape")]
        public static void LoadEscape() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/Escape.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL Fun")]
        public static void LoadLVLFun() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL Fun.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL Habitable Zone")]
        public static void LoadLVLHabitableZone() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL Habitable Zone.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL Lobby")]
        public static void LoadLVLLobby() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL Lobby.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL Pipe Dreams")]
        public static void LoadLVLPipeDreams() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL Pipe Dreams.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL Poolrooms")]
        public static void LoadLVLPoolrooms() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL Poolrooms.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL RFYL")]
        public static void LoadLVLRFYL() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL RFYL.unity"); }
        [MenuItem("Scenes/Testing/Jumpscares")]
        public static void LoadJumpscares() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Jumpscares.unity"); }
        [MenuItem("Scenes/Testing/Mobile Test")]
        public static void LoadMobileTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Mobile Test.unity"); }
        [MenuItem("Scenes/Testing/Shader Test")]
        public static void LoadShaderTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Shader Test.unity"); }
        [MenuItem("Scenes/Testing/Test")]
        public static void LoadTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Test.unity"); }
        [MenuItem("Scenes/Testing/Video Experiments")]
        public static void LoadVideoExperiments() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Video Experiments.unity"); }
        [MenuItem("Scenes/Thumbnail/Thumbnail")]
        public static void LoadThumbnail() { EditorSceneSwitcher.OpenScene("Assets/scenes/Thumbnail/Thumbnail.unity"); }
#endif
    }
}