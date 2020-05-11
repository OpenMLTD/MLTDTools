param([String]$zipName = "miritore.zip")

[String]$configuration = $env:CONFIGURATION
[String]$basePath = $env:APPVEYOR_BUILD_FOLDER

$subPaths = @(
    [String]::Format("src/AcbPack/bin/{0}", $configuration),
    [String]::Format("src/HcaDec/bin/{0}", $configuration),
    [String]::Format("src/ManifestExport/bin/{0}", $configuration),
    [String]::Format("src/MillionDance/bin/{0}", $configuration),
    [String]::Format("src/MillionDanceView/bin/{0}", $configuration),
    [String]::Format("src/MiriTore.Common/bin/{0}", $configuration),
    [String]::Format("src/MiriTore.Logging/bin/{0}", $configuration),
    [String]::Format("src/MltdInfoViewer/bin/{0}", $configuration),
    #[String]::Format("src/ScenarioEdit/bin/{0}", $configuration),
	[String]::Format("src/TDFacial/bin/{0}", $configuration), # remember to include facial_expr.json
    [String]::Format("src/ManifestTools/bin/{0}", $configuration),
);

foreach ($subPath in $subPaths) {
    [String]$fullDirPath = [System.IO.Path]::Combine($basePath, $subPath, "*");
    
    [ScriptBlock]$scriptBlock = { 7z a $zipName -r $fullDirPath }
    Invoke-Command -ScriptBlock $scriptBlock
}
