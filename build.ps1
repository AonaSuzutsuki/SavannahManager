Param( $outName )

if ( [string]::IsNullOrEmpty($outName) ) {
    $outName = "bin"
}

$CurrentDirectory = Split-Path $MyInvocation.MyCommand.Path -Parent
$OutBinDirectory = "$CurrentDirectory\$outName"
$Framework = "netcoreapp3.1"

$SavannahManagerReleaseDirectory = "$CurrentDirectory\SavannahManager\bin\Release\$Framework"
$XmlEditorReleaseDirectory = "$CurrentDirectory\SavannahXmlEditor\bin\Release\$Framework"
$ConfigEditorReleaseDirectory = "$CurrentDirectory\ConfigEditor\bin\Release\$Framework"
$UpdaterReleaseDirectory = "$CurrentDirectory\Updater\bin\Release\$Framework"

#SavannahManager
xcopy /Y $SavannahManagerReleaseDirectory\*.dll $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.exe $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.deps.json $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.runtimeconfig.json $OutBinDirectory\
Copy-Item -Path $SavannahManagerReleaseDirectory\Settings\ -Destination "$OutBinDirectory\Settings" -Recurse -Force
Copy-Item -Path $SavannahManagerReleaseDirectory\en-US\ -Destination "$OutBinDirectory\en-US" -Recurse -Force

# XmlEditor
xcopy /Y $XmlEditorReleaseDirectory\*.dll "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.exe "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.deps.json "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.runtimeconfig.json "$OutBinDirectory\XmlEditor\"

# ConfigEditor
xcopy /Y $ConfigEditorReleaseDirectory\*.dll "$OutBinDirectory\ConfigEditor\"
xcopy /Y $ConfigEditorReleaseDirectory\*.exe "$OutBinDirectory\ConfigEditor\"
xcopy /Y $ConfigEditorReleaseDirectory\*.deps.json "$OutBinDirectory\ConfigEditor\"
xcopy /Y $ConfigEditorReleaseDirectory\*.runtimeconfig.json "$OutBinDirectory\ConfigEditor\"
Copy-Item -Path $ConfigEditorReleaseDirectory\lang\ -Destination "$OutBinDirectory\ConfigEditor\" -Recurse -Force
Copy-Item -Path $ConfigEditorReleaseDirectory\en-US\ -Destination "$OutBinDirectory\ConfigEditor\" -Recurse -Force

# Updater
xcopy /Y $UpdaterReleaseDirectory\*.dll "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.exe "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.deps.json "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.runtimeconfig.json "$OutBinDirectory\Updater\"