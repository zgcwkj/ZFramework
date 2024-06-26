﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using zgcwkj.Util;

namespace ZFramework.Comm
{
    /// <summary>
    /// 配置 Swagger
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Swagger 配置
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="title">标题</param>
        /// <param name="version">版本</param>
        /// <param name="description">描述</param>
        public static void AddSwagger(this IServiceCollection services, string title = "接口文档", string version = "v1", string? description = default)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //Api Explorer
            services.AddEndpointsApiExplorer();
            //注册 Swagger
            services.AddSwaggerGen(options =>
            {
                //文档
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = description,
                });
                //排序接口
                //options.OrderActionsBy(o => o.RelativePath);
                //显示注释

                var basePath = GlobalConstant.GetRunPath;
                var xmlFilesPath = Directory.GetFiles(basePath, "*.xml");
                foreach (var xmlPath in xmlFilesPath)
                {
                    options.IncludeXmlComments(xmlPath, true);
                }
            });
        }

        /// <summary>
        /// Swagger 和 Jwt 配置
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="title">标题</param>
        /// <param name="version">版本</param>
        /// <param name="description">描述</param>
        public static void AddSwaggerJwt(this IServiceCollection services, string title = "接口文档", string version = "v1", string? description = default)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //Api Explorer
            services.AddEndpointsApiExplorer();
            //注册 Swagger
            services.AddSwaggerGen(options =>
            {
                //文档
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = description,
                });
                //排序接口
                //options.OrderActionsBy(o => o.RelativePath);
                //显示注释
                var basePath = GlobalConstant.GetRunPath;
                var xmlFilesPath = Directory.GetFiles(basePath, "*.xml");
                foreach (var xmlPath in xmlFilesPath)
                {
                    options.IncludeXmlComments(xmlPath, true);
                }
                //放置接口 Auth 授权按钮
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Value Bearer {token}",
                });
                //添加 Jwt 验证请求头信息
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }

        /// <summary>
        /// Swagger 路由
        /// </summary>
        /// <param name="app">应用</param>
        /// <param name="url">地址</param>
        /// <param name="api">接口</param>
        /// <param name="name">名称</param>
        public static void AddSwagger(this IApplicationBuilder app, string url = "Swagger", string api = "/swagger/v1/swagger.json", string name = "Web API")
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(api, name);
                //访问地址
                options.RoutePrefix = url;
                //修改界面打开时自动展开
                options.DocExpansion(DocExpansion.List);
                //隐藏底部的 Schemas
                options.DefaultModelsExpandDepth(-1);
                //展开请求详情的 Schema
                options.DefaultModelExpandDepth(int.MaxValue);
            });
        }
    }
}
