@echo off
echo Lanzando MotorDsl.SampleApp en dispositivo Android...
set PATH=%PATH%;C:\Program Files (x86)\Android\android-sdk\platform-tools
adb devices
cd samples\MotorDsl.SampleApp
dotnet build -t:Run -f net10.0-android
cd ..\..
pause
