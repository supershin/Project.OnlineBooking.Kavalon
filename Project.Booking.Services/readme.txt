link = https://www.c-sharpcorner.com/article/create-windows-services-in-c-sharp/

======================================
Installing a Windows Service
 
Open the command prompt and fire the below command and press ENTER.
 
 เข้าไปที่ Path = cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 

Syntax
InstallUtil.exe + Your copied path + \your service name + .exe
 
Our path
InstallUtil.exe C:\Users\Faisal-Pathan\source\repos\MyFirstService\MyFirstService\bin\Debug\MyFirstService.exe

=============================================
Uninstalling a Windows Service
 เข้าไปที่ Path = cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 

If you want to uninstall your service, fire the below command.
1. Syntax InstallUtil.exe -u + Your copied path + \your service name + .exe
2. Our path InstallUtil.exe -u C:\Users\Faisal-Pathan\source\repos\MyFirstService\MyFirstService\bin\Debug\MyFirstService.exe


