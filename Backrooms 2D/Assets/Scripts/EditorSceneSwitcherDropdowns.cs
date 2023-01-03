using UnityEditor;
namespace KingdomOfNight
{
    public partial class EditorSceneSwitcher
    {
#if UNITY_EDITOR
        [MenuItem("Scenes/Backrooms Levels/Epilepsy Warning")]
        public static void LoadEpilepsyWarning() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/Epilepsy Warning.unity"); }
        [MenuItem("Scenes/Backrooms Levels/Escape")]
        public static void LoadEscape() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/Escape.unity"); }
        [MenuItem("Scenes/Backrooms Levels/LVL Electrical Station")]
        public static void LoadLVLElectricalStation() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL Electrical Station.unity"); }
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
        [MenuItem("Scenes/Backrooms Levels/LVL The End")]
        public static void LoadLVLTheEnd() { EditorSceneSwitcher.OpenScene("Assets/scenes/Backrooms Levels/LVL The End.unity"); }
        [MenuItem("Scenes/Testing/Electrical Station Gate Test")]
        public static void LoadElectricalStationGateTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Electrical Station Gate Test.unity"); }
        [MenuItem("Scenes/Testing/Input Field Test")]
        public static void LoadInputFieldTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Input Field Test.unity"); }
        [MenuItem("Scenes/Testing/Instantiating Performance Test")]
        public static void LoadInstantiatingPerformanceTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Instantiating Performance Test.unity"); }
        [MenuItem("Scenes/Testing/Mobile Test")]
        public static void LoadMobileTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Mobile Test.unity"); }
        [MenuItem("Scenes/Testing/OverlapBoxText")]
        public static void LoadOverlapBoxText() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/OverlapBoxText.unity"); }
        [MenuItem("Scenes/Testing/Shader Test")]
        public static void LoadShaderTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Shader Test.unity"); }
        [MenuItem("Scenes/Testing/Test")]
        public static void LoadTest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Test.unity"); }
        [MenuItem("Scenes/Testing/UI Test")]
        public static void LoadUITest() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/UI Test.unity"); }
        [MenuItem("Scenes/Testing/Video Experiments")]
        public static void LoadVideoExperiments() { EditorSceneSwitcher.OpenScene("Assets/scenes/Testing/Video Experiments.unity"); }
        [MenuItem("Scenes/Thumbnail/Thumbnail 1")]
        public static void LoadThumbnail1() { EditorSceneSwitcher.OpenScene("Assets/scenes/Thumbnail/Thumbnail 1.unity"); }
        [MenuItem("Scenes/Thumbnail/Thumbnail 2 Remake 2")]
        public static void LoadThumbnail2Remake2() { EditorSceneSwitcher.OpenScene("Assets/scenes/Thumbnail/Thumbnail 2 Remake 2.unity"); }
        [MenuItem("Scenes/Thumbnail/Thumbnail 2 Remake")]
        public static void LoadThumbnail2Remake() { EditorSceneSwitcher.OpenScene("Assets/scenes/Thumbnail/Thumbnail 2 Remake.unity"); }
        [MenuItem("Scenes/Thumbnail/Thumbnail 2")]
        public static void LoadThumbnail2() { EditorSceneSwitcher.OpenScene("Assets/scenes/Thumbnail/Thumbnail 2.unity"); }
        [MenuItem("Scenes/Videos/Part 3/Part 3 Intro")]
        public static void LoadPart3Intro() { EditorSceneSwitcher.OpenScene("Assets/scenes/Videos/Part 3/Part 3 Intro.unity"); }
#endif
    }
}