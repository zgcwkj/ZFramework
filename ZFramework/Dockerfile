#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# base �׶�
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# ��װ�����
RUN apt-get update && apt-get install -y libfontconfig1

# build �׶�
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# ��������
ENV PATH="${PATH}:/root/.dotnet/tools"
# ��ԭ��Ŀ
WORKDIR /src
COPY ["ZFramework/ZFramework.csproj", "ZFramework/"]
RUN dotnet restore "ZFramework/ZFramework.csproj"
# ���Ƶ�����Ŀ¼
COPY . .
# ������Ŀ
WORKDIR "/src/ZFramework"
RUN dotnet build "ZFramework.csproj" -c Release -o /app/build
# �������ݿ�
WORKDIR "/src/ZFramework.Data"
RUN dotnet tool install -g dotnet-ef
#RUN dotnet ef migrations add init
RUN dotnet ef database update
RUN cp -r zframework_db.db /app

# publish �׶�
FROM build AS publish
WORKDIR "/src/ZFramework"
RUN dotnet publish "ZFramework.csproj" -c Release -o /app/publish /p:UseAppHost=false

# final �׶�
FROM base AS final
# �л�����Ŀ¼
WORKDIR /app
# ���� publish �׶ε��ļ�
COPY --from=publish /app/publish .
# ���� build �׶ε��ļ�
COPY --from=build /app/zframework_db.db .
# Ĭ���������
ENTRYPOINT ["dotnet", "ZFramework.dll"]