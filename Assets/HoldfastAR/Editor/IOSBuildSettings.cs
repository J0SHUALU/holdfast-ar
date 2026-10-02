#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace HoldfastAR.EditorTools
{
    public static class IOSBuildSettings
    {
        private const string SwiftLibraries = "$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)";
        private const string SwiftLibrariesLegacy = "$(TOOLCHAIN_DIR)/usr/lib/swift-5.0/$(PLATFORM_NAME)";

        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;

            string projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);

            foreach (string guid in new[] { project.GetUnityMainTargetGuid(), project.GetUnityFrameworkTargetGuid() })
            {
                project.SetBuildProperty(guid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
                project.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", SwiftLibraries);
                project.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", SwiftLibrariesLegacy);
                project.AddBuildProperty(guid, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");
            }

            project.WriteToFile(projectPath);
        }
    }
}
#endif
