@echo off

cd  ..

set PROJECT_PATH=samples\MotorDsl.MultaApp\MotorDsl.MultaApp.csproj
set OUTPUT_PATH=output

echo Limpiando residuos de compilaciones anteriores...
:: Borra las carpetas bin y obj para asegurar un rebuild real
if exist samples\MotorDsl.MultaApp\bin rd /s /q samples\MotorDsl.MultaApp\bin
if exist samples\MotorDsl.MultaApp\obj rd /s /q samples\MotorDsl.MultaApp\obj

echo.
echo Publicando MultaApp (Rebuild total) como APK...
dotnet publish %PROJECT_PATH% ^
  -f net10.0-android ^
  -c Release ^
  -p:AndroidPackageFormat=apk ^
  -p:AndroidKeyStore=false ^
  -o %OUTPUT_PATH% ^
  /p:ForceGenerationOfBuildId=true

echo.
echo APK generado en: %OUTPUT_PATH%\
echo Instalá con: adb install %OUTPUT_PATH%\com.motordsl.multaapp-Signed.apk
pause

echo.