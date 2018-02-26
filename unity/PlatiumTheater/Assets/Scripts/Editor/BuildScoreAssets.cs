using System.Collections.Generic;
using System.IO;
using Imas.Live;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public static class BuildScoreAssets {

    [MenuItem("Tools/MLTD Tools/Build Score Object Assets")]
    public static void Build() {
        const string scoreResourcesBaseDir = "Assets/Imas/Resources/Scrobj";

        var subDirs = Directory.GetDirectories(scoreResourcesBaseDir);

        foreach (var subDir in subDirs) {
            var resourceName = Path.GetFileName(subDir);
            Debug.Assert(resourceName != null, "resourceName != null");
            CreateScorePack(scoreResourcesBaseDir, resourceName);
        }

        Debug.Log("Built score assets");
    }

    private static void CreateScorePack([NotNull] string allScoresBaseDir, [NotNull] string songResourceName) {
        var thisBaseDir = Path.Combine(allScoresBaseDir, songResourceName);

        var noteScrObj = ScriptableObject.CreateInstance<NoteScrObj>();
        var noteScrObjPath = Path.Combine(thisBaseDir, songResourceName + "_fumen_sobj.asset");
        ScrObjLoader.LoadBeatmap(noteScrObj, Path.Combine(thisBaseDir, songResourceName + "_fumen_sobj.txt"));

        var scenarioObj = ScriptableObject.CreateInstance<ScenarioScrObj>();
        var scenarioObjPath = Path.Combine(thisBaseDir, songResourceName + "_scenario_sobj.asset");
        ScrObjLoader.LoadScenario(scenarioObj, Path.Combine(thisBaseDir, songResourceName + "_scenario_sobj.txt"));

        var scenarioVertObj = ScriptableObject.CreateInstance<ScenarioScrObj>();
        var scenarioVertObjPath = Path.Combine(thisBaseDir, songResourceName + "_scenario_tate_sobj.asset");
        ScrObjLoader.LoadScenario(scenarioVertObj, Path.Combine(thisBaseDir, songResourceName + "_scenario_tate_sobj.txt"));

        var scenarioHorizObj = ScriptableObject.CreateInstance<ScenarioScrObj>();
        var scenarioHorizObjPath = Path.Combine(thisBaseDir, songResourceName + "_scenario_yoko_sobj.asset");
        ScrObjLoader.LoadScenario(scenarioHorizObj, Path.Combine(thisBaseDir, songResourceName + "_scenario_yoko_sobj.txt"));

        var pairs = new Dictionary<string, ScriptableObject> {
            {noteScrObjPath, noteScrObj},
            {scenarioObjPath, scenarioObj},
            {scenarioVertObjPath, scenarioVertObj},
            {scenarioHorizObjPath, scenarioHorizObj}
        };

        var assetBundleName = string.Format("scrobj_{0}.unity3d", songResourceName);

        foreach (var kv in pairs) {
            AssetDatabase.CreateAsset(kv.Value, kv.Key);

            var assetImporter = AssetImporter.GetAtPath(kv.Key);
            assetImporter.assetBundleName = assetBundleName;
        }
    }

}
