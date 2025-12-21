using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Build Android APK")]
    public static void BuildAndroid()
    {
        Debug.Log("[BuildScript] Starting Android build...");

        // Configure build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/_AR Lingo/Scenes/Main Scene.unity" },
            locationPathName = "D:/Codevs/AR-Lingo-Builds/AR-Lingo.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        // Build
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BuildScript] ✅ Build succeeded: {summary.totalSize} bytes");
            Debug.Log($"[BuildScript] APK location: {buildPlayerOptions.locationPathName}");
        }
        else
        {
            Debug.LogError($"[BuildScript] ❌ Build failed: {summary.result}");
            
            // Print all errors
            foreach (var step in report.steps)
            {
                foreach (var message in step.messages)
                {
                    if (message.type == LogType.Error || message.type == LogType.Exception)
                    {
                        Debug.LogError($"[BuildScript] Error: {message.content}");
                    }
                }
            }
        }
    }
}