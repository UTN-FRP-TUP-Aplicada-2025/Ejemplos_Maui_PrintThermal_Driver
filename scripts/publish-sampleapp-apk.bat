@echo off
echo Publicando SampleApp como APK...
dotnet publish samples\MotorDsl.SampleApp\MotorDsl.SampleApp.csproj ^
  -f net10.0-android ^
  -c Release ^
  -p:AndroidPackageFormat=apk ^
  -p:AndroidKeyStore=false ^
  -o output\sampleapp
echo.
echo APK generado en: output\sampleapp\
echo Instalá con: adb install output\sampleapp\com.motordsl.sampleapp-Signed.apk
pause
