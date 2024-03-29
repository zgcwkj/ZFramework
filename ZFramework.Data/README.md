                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

## 官方文档：

https://docs.microsoft.com/zh-cn/ef/core/cli/dotnet


### 卸载当前版本

```
dotnet tool uninstall -g dotnet-ef
```

### 安装最新版本

```
dotnet tool install -g dotnet-ef
```

### 使用说明

```
dotnet ef
```

### 添加新的对象时生成数据库模板

```
dotnet ef migrations add 名称（比如描述什么功能）
```

### 撤回上一次的模板添加

```
dotnet ef migrations remove
```

### 获取模板的完整脚本

```
dotnet ef migrations script
```

### 应用到数据库中去（应用后好像无法撤回）

```
dotnet ef database update
```