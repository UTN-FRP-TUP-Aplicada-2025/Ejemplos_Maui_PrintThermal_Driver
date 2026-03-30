@echo off
echo Publicando SampleApp...
dotnet publish samples\MotorDsl.SampleApp\MotorDsl.SampleApp.csproj ^
  -f net10.0-android ^
  -c Release ^
  -p:AndroidPackageFormat=apk ^
  -p:AndroidKeyStore=false
echo Listo. APK en: samples\MotorDsl.SampleApp\bin\Release\net10.0-android\publish\
pause