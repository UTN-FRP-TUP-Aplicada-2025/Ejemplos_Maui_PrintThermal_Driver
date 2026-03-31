@echo off
echo Lanzando MotorDsl.MultaApp en dispositivo Android...
set PATH=%PATH%;C:\Program Files (x86)\Android\android-sdk\platform-tools
adb devices
cd samples\MotorDsl.MultaApp
dotnet build -t:Run -f net10.0-android
cd ..\..
pause
