# Setup
install dotnet 2.2 sdk first (https://dotnet.microsoft.com/download/linux-package-manager/centos/sdk-2.2.401)
git clone
cd NotificationService.Api
dotnet publish -c Release
cd bin/Release/netcoreapp2.2/publish
dotnet NotificationService.Api.dll

# NotificationService
Notification system architecture and implementation for Email, SMS, Push Notifications and Web. Backed by Hangfire and MongoDb

# Third Party Packages
This software package uses other libraries/software below;

Hangfire: https://github.com/HangfireIO/Hangfire

Hangfire.Mongo: https://github.com/sergeyzwezdin/Hangfire.Mongo
