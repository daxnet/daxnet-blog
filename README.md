# daxnet-blog
本人个人博客系统【[http://daxnet.me](http://daxnet.me)】源代码。

## 技术概览
### 部署环境 ###
- Linux Ubuntu 14.04.4 LTS
- Docker
- Microsoft Windows Azure


### 前端 ###

- ASP.NET Core MVC
	- ASP.NET Identity
	- Middlewares
	- Tag Helpers
- jQuery
- Bootstrap
- CKEditor
- Syntaxhighlighter
- libgdiplus (在服务端提供对CAPTCHA图片生成的支持）
- 对Metaweblog API的支持

### 后端 ###

- ASP.NET Core Web API
	- Middlewares
	- Autofac Web API Integration
- Autofac
- ADO.NET
	- 自定义的基于ADO.NET的RDBMS存储解决方案
	- 自定义的实体对象存储操作（Entity Stores）

### 所使用的Windows Azure服务 ###

- Azure SQL Database
- Azure BLOB Storage
- Azure SendGrid Account (for Email support)
- Azure VM (Docker Host)

