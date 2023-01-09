#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# base 阶段
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# 安装字体库
RUN apt-get update && apt-get install -y libfontconfig1

# build 阶段
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# 环境变量
ENV PATH="${PATH}:/root/.dotnet/tools"
# 还原项目
WORKDIR /src
COPY ["ZFramework/ZFramework.csproj", "ZFramework/"]
RUN dotnet restore "ZFramework/ZFramework.csproj"
# 复制到容器目录
COPY . .
# 构建项目
WORKDIR "/src/ZFramework"
RUN dotnet build "ZFramework.csproj" -c Release -o /app/build
# 构建数据库
WORKDIR "/src/ZFramework.Data"
RUN dotnet tool install -g dotnet-ef
#RUN dotnet ef migrations add init
RUN dotnet ef database update
RUN cp -r zframework_db.db /app

# publish 阶段
FROM build AS publish
WORKDIR "/src/ZFramework"
RUN dotnet publish "ZFramework.csproj" -c Release -o /app/publish /p:UseAppHost=false

# final 阶段
FROM base AS final
# 切换工作目录
WORKDIR /app
# 复制 publish 阶段的文件
COPY --from=publish /app/publish .
# 复制 build 阶段的文件
COPY --from=build /app/zframework_db.db .
# 默认入口命令
ENTRYPOINT ["dotnet", "ZFramework.dll"]
