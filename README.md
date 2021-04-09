# RingWebAPI
这是一个WebAPI的模板项目，内置很多基础功能，在创建项目时作出选择，生成对应的项目模板，这不是银弹，只是减少重复工作。



生成Nuget
.\nuget.exe pack .\RingWebAPI.nuspec -OutputDirectory .\


进入nupkg目录，安装这个Template包
dotnet new -i RingWebAPI.0.0.1.nupkg

卸载这个Template包
dotnet new -u RingWebAPI
