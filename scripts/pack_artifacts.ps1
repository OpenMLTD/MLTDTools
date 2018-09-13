param([System.String] $zipName = "miritore.zip")

$configuration = $env:CONFIGURATION
$basePath = $env:APPVEYOR_BUILD_FOLDER

$subPaths = @(
    [System.String]::Format("src/AcbPack/bin/{0}", $configuration),
    [System.String]::Format("src/HcaDec/bin/{0}", $configuration),
    [System.String]::Format("src/ManifestExport/bin/{0}", $configuration),
    [System.String]::Format("src/MillionDance/bin/{0}", $configuration),
    [System.String]::Format("src/MillionDanceView/bin/{0}", $configuration),
    [System.String]::Format("src/MiriTore.Common/bin/{0}", $configuration),
    [System.String]::Format("src/MiriTore.Logging/bin/{0}", $configuration),
    [System.String]::Format("src/MltdInfoViewer/bin/{0}", $configuration)#,
    #[System.String]::Format("src/ScenarioEdit/bin/{0}", $configuration),
);

foreach ($subPath in $subPaths) {
    $fullDirPath = [System.IO.Path]::Combine($basePath, $subPath, "*");
    
    $scriptBlock = { 7z a $zipName -r $fullDirPath }
    Invoke-Command -ScriptBlock $scriptBlock
}
