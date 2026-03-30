@echo off
set PATH=%PATH%;C:\Program Files (x86)\Android\android-sdk\platform-tools
cd samples\MotorDsl.MultaApp
dotnet build -t:Run -f net10.0-android
cd ..\..