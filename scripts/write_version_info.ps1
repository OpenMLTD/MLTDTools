param(
    [String]$FilePath = "version.txt"
)

[String]$versionText = "${env:APPVEYOR_BUILD_VERSION}${env:RELEASE_SUFFIX}";

[System.IO.File]::WriteAllText($FilePath, $versionText, [System.Text.Encoding]::UTF8);
