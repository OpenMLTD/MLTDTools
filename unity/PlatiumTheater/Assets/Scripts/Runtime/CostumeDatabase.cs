using System.IO;
using System.Text;
using JetBrains.Annotations;
using SFB;
using UnityEngine;

public class CostumeDatabase : MonoBehaviour {

    private void Awake() {
        _errorLog = gameObject.GetComponent<ErrorLog>();
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(10, 10, 200, 40), "Export Costume DB")) {
            DoExportCostumeDatabase();
        }
    }

    private void DoExportCostumeDatabase() {
        var openExtensions = new[] {
            new ExtensionFilter("Unity Asset Bundle", "unity3d")
        };

        var openPaths = StandaloneFileBrowser.OpenFilePanel("Open File", string.Empty, openExtensions, false);

        if (openPaths.Length == 0) {
            return;
        }

        var assetBundle = AssetBundle.LoadFromFile(openPaths[0]);

        const string costumeDbAssetName = "assets/imas/resources/json/chara_costume_release.json";
        if (!assetBundle.Contains(costumeDbAssetName)) {
            var fileName = Path.GetFileName(openPaths[0]);

            Log(fileName + " :");
            Log("This is not the costume database.");

            return;
        }

        var asset = assetBundle.LoadAsset<TextAsset>(costumeDbAssetName);
        var costumeDbDefaultFileName = Path.GetFileName(costumeDbAssetName);

        var saveExtension = new[] {
            new ExtensionFilter("JSON", "json")
        };

        var savePath = StandaloneFileBrowser.SaveFilePanel("Save File", string.Empty, costumeDbDefaultFileName, saveExtension);

        if (string.IsNullOrEmpty(savePath)) {
            return;
        }

        File.WriteAllText(savePath, asset.text, Utf8WithoutBom);

        Log("Done: " + savePath);
    }

    private void Log([NotNull] string text) {
        if (_errorLog != null) {
            _errorLog.Add(text);
        }
    }

    [CanBeNull]
    private ErrorLog _errorLog;

    private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

}
