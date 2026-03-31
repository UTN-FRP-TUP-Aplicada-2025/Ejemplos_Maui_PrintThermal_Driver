@echo off
echo Publicando MultaApp como APK...
dotnet publish samples\MotorDsl.MultaApp\MotorDsl.MultaApp.csproj ^
  -f net10.0-android ^
  -c Release ^
  -p:AndroidPackageFormat=apk ^
  -p:AndroidKeyStore=false ^
  -o output\multaapp
echo.
echo APK generado en: output\multaapp\
echo Instalá con: adb install output\multaapp\com.motordsl.multaapp-Signed.apk
pause
