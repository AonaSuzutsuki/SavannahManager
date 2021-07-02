Param( $outName )

if ( [string]::IsNullOrEmpty($outName) ) {
    $outName = "bin"
}

$CurrentDirectory = Split-Path $MyInvocation.MyCommand.Path -Parent
$OutBinDirectory = "$CurrentDirectory\$outName"
$Framework = "net48"

$SavannahManagerReleaseDirectory = "$CurrentDirectory\SavannahManager\bin\Release\$Framework"
$XmlEditorReleaseDirectory = "$CurrentDirectory\SavannahXmlEditor\bin\Release\$Framework"
$ConfigEditorReleaseDirectory = "$CurrentDirectory\ConfigEditor\bin\Release\$Framework"
$UpdaterReleaseDirectory = "$CurrentDirectory\Updater\bin\Release\$Framework"

#SavannahManager
xcopy /Y $SavannahManagerReleaseDirectory\*.dll $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.config $OutBinDirectory\
xcopy /Y $SavannahManagerReleaseDirectory\*.exe $OutBinDirectory\
Copy-Item -Path $SavannahManagerReleaseDirectory\Settings\ -Destination "$OutBinDirectory\Settings" -Recurse -Force
Copy-Item -Path $SavannahManagerReleaseDirectory\en-US\ -Destination "$OutBinDirectory\en-US" -Recurse -Force

# XmlEditor
xcopy /Y $XmlEditorReleaseDirectory\*.dll "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.config "$OutBinDirectory\XmlEditor\"
xcopy /Y $XmlEditorReleaseDirectory\*.exe "$OutBinDirectory\XmlEditor\"

# ConfigEditor
xcopy /Y $ConfigEditorReleaseDirectory\*.dll "$OutBinDirectory\ConfigEditor\"
xcopy /Y $ConfigEditorReleaseDirectory\*.config "$OutBinDirectory\ConfigEditor\"
xcopy /Y $ConfigEditorReleaseDirectory\*.exe "$OutBinDirectory\ConfigEditor\"
Copy-Item -Path $ConfigEditorReleaseDirectory\lang\ -Destination "$OutBinDirectory\ConfigEditor\" -Recurse -Force
Copy-Item -Path $ConfigEditorReleaseDirectory\en-US\ -Destination "$OutBinDirectory\ConfigEditor\" -Recurse -Force

# Updater
xcopy /Y $UpdaterReleaseDirectory\*.dll "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.config "$OutBinDirectory\Updater\"
xcopy /Y $UpdaterReleaseDirectory\*.exe "$OutBinDirectory\Updater\"