using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ZFramework.Controllers
{
    /// <summary>
    /// 日志管理
    /// </summary>
    [Models.Authorization]
    public class LogController : Controller
    {
        /// <summary>
        /// 日志管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询日志数据
        /// </summary>
        /// <param name="Page">页码</param>
        /// <param name="PageSize">每页数量</param>
        /// <param name="QueryLikeStr">模糊搜索内容</param>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <returns></returns>
        public ActionResult InquireLog(string Page, string PageSize, string QueryLikeStr, string BeginDate, string EndDate)
        {
            if (Page == null || PageSize == null || QueryLikeStr == null) { return null; }
            if (Page == "" || PageSize == "") { return null; }

            try
            {
                #region SQL查询方法

                if (BeginDate == "") { BeginDate = null; }
                if (EndDate == "") { EndDate = null; }
                SqlParameter[] sql =
                {
                    new SqlParameter("@Type",SqlDbType.NChar){ Value = "InquireLogData" },
                    new SqlParameter("@Page",SqlDbType.NChar){ Value = Page },
                    new SqlParameter("@PageSize",SqlDbType.NChar){ Value = PageSize },
                    new SqlParameter("@QueryLikeStr",SqlDbType.NChar){ Value = QueryLikeStr },
                    new SqlParameter("@BeginDate",SqlDbType.NChar){ Value = BeginDate },
                    new SqlParameter("@EndDate",SqlDbType.NChar){ Value = EndDate },
                };
                List<Dictionary<string, object>> ListReturn = Models.StaticData.myDal.QueryList("LogManage", sql);
                var total = "0";//获取总数
                if (ListReturn.Count != 0) total = ListReturn[0]["tbCount"].ToString();

                #endregion SQL查询方法

                return Json(new { status = 0, message = "成功", total, rows = ListReturn }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new { status = -1, message = "失败" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 新增日志数据
        /// </summary>
        /// <param name="FilePath">请求地址</param>
        /// <param name="ControllerName">控制器名称</param>
        /// <returns></returns>
        public ActionResult InsertLogData(string FilePath, string ControllerName)
        {
            if (FilePath == null || ControllerName == null) { return null; }
            if (FilePath == "" || ControllerName == "") { return null; }

            try
            {
                #region SQL操作方法

                var userID = Session["UserID"];//用户ID
                var toUserIP = Request.UserHostAddress;//用户IP

                SqlParameter[] sqlInsertLogData =
                {
                    new SqlParameter("@Type",SqlDbType.NChar){ Value = "InsertLogData" },
                    new SqlParameter("@UserID",SqlDbType.Int){ Value = userID },
                    new SqlParameter("@UserIP",SqlDbType.NChar){ Value = toUserIP },
                    new SqlParameter("@RequestPath",SqlDbType.NChar){ Value = FilePath },
                    new SqlParameter("@Controller",SqlDbType.NChar){ Value = ControllerName },
                };
                Models.StaticData.myDal.UpdateData("LogManage", sqlInsertLogData);

                #endregion SQL操作方法

                return Json(new { status = 0, message = "成功" }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new { status = -1, message = "失败" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 删除所有日志
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteLogDate()
        {
            Models.ReturnJson returnJson = new Models.ReturnJson();
            returnJson.Message = "信息错误";
            returnJson.Status = false;

            #region SQL操作方法 删除所有日志

            SqlParameter[] sqlDeleteRoleDetailData =
            {
                new SqlParameter("Type",SqlDbType.NChar){ Value = "DeleteLogDate" }
            };
            int DelectCount = Models.StaticData.myDal.UpdateData("LogManage", sqlDeleteRoleDetailData);

            #endregion SQL操作方法 删除所有日志

            if (DelectCount > 0)
            {
                returnJson.Message = "删除成功";
                returnJson.Status = true;
            }
            else
            {
                returnJson.Message = "删除失败";
                returnJson.Status = false;
            }

            return Json(returnJson, JsonRequestBehavior.DenyGet);
        }
    }
}