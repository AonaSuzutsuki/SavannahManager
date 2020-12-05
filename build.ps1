Param( $outName )

if ( [string]::IsNullOrEmpty($outName) ) {
    $outName = "bin"
}

$CurrentDirectory = Split-Path $MyInvocation.MyCommand.Path -Parent
$OutBinDirectory = "$CurrentDirectory\$outName"

$SavannahManagerReleaseDirectory = "$CurrentDirectory\7dtd-svmanager-fix-mvvm\bin\Release"
$XmlEditorReleaseDirectory = "$CurrentDirectory\7dtd-XmlEditor\bin\Release"
$ConfigEditorReleaseDirectory = "$CurrentDirectory\ConfigEditor-mvvm\bin\Release"
$UpdaterReleaseDirectory = "$CurrentDirectory\Updater\bin\Release"

#SavannahManager
xcopy /Y $SavannahManagerReleaseDirectory\*.dll $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.xml $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.config $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.exe $OutBinDirectory\
Copy-Item -Path $SavannahManagerReleaseDirectory\Settings\ -Destination "$OutBinDirectory\Settings" -Recurse -Force
Copy-Item -Path $SavannahManagerReleaseDirectory\en-US\ -Destination "$OutBinDirectory\en-US" -Recurse -Force

# XmlEditor
xcopy /Y $XmlEditorReleaseDirectory\*.dll "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.xml "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.config "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.exe "$OutBinDirectory\XmlEditor\"

# ConfigEditor
xcopy /Y $ConfigEditorReleaseDirectory\*.dll $OutBinDirectory\
xcopy /Y $ConfigEditorReleaseDirectory\*.xml $OutBinDirectory\
xcopy /Y $ConfigEditorReleaseDirectory\*.config $OutBinDirectory\
xcopy /Y $ConfigEditorReleaseDirectory\*.exe $OutBinDirectory\
Copy-Item -Path $ConfigEditorReleaseDirectory\lang\ -Destination $OutBinDirectory\ -Recurse -Force
Copy-Item -Path $ConfigEditorReleaseDirectory\en-US\ -Destination $OutBinDirectory\ -Recurse -Force

# Updater
xcopy /Y $UpdaterReleaseDirectory\*.dll "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.xml "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.config "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.exe "$OutBinDirectory\Updater\"