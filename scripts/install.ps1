Start-FileDownload "https://damassets.autodesk.net/content/dam/autodesk/www/adn/fbx/2020-0-1/fbx202001_fbxsdk_vs2017_win.exe" "fbxsdk.exe"
Start-Process -FilePath "fbxsdk.exe" /S -Wait
