# RingWebAPI
这是一个WebAPI的模板项目，内置一些基础功能，在创建项目时作出选择，生成对应的项目模板，这不是银弹，只是减少重复工作。

### 功能
- 是否启用NLog
- 是否启用SWagger
- 是否启有Dapper
- 使用数据库类型：MSSql,MySql,Postgresql
- 使用权限认证方式：无权限认证，固定角色，自定义策略



生成Nuget
> .\nuget.exe pack .\RingWebAPI.nuspec -OutputDirectory .\

安装这个Template包
> dotnet new -i RingWebAPI.0.0.1.nupkg

卸载这个Template包
> dotnet new -u RingWebAPI
