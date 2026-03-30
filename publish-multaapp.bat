@echo off
echo Publicando MultaApp...
dotnet publish samples\MotorDsl.MultaApp\MotorDsl.MultaApp.csproj ^
  -f net10.0-android ^
  -c Release ^
  -p:AndroidPackageFormat=apk ^
  -p:AndroidKeyStore=false
echo Listo. APK en: samples\MotorDsl.MultaApp\bin\Release\net10.0-android\publish\
pause