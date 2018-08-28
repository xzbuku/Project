﻿using OjVolunteer.IBLL;
using OjVolunteer.UIPortal.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OjVolunteer.UIPortal.Controllers
{
    public class ActivityDetailController : Controller
    {
        public IActivityDetailService ActivityDetailService { get; set; }
        public short delNormal = (short)Model.Enum.DelFlagEnum.Normal;
        #region 用户活动详情
        [ActionAuthentication(AbleOrganize = true, AbleUser = true)]
        public JsonResult GetActivityDetailByUserId()
        {
            int pageSize = int.Parse(Request["limit"] ?? "5");
            int offset = int.Parse(Request["offset"] ?? "0");
            int pageIndex = (offset / pageSize) + 1;
            if (String.IsNullOrEmpty(Request["userId"]))
            {
                return Json(new { total = 0, rows = "" }, JsonRequestBehavior.AllowGet);
            }
            int userId = Convert.ToInt32(Request["userId"]);
            var pageData = ActivityDetailService.GetPageEntities(pageSize, pageIndex, out int total, u => u.UserInfoId == userId&&u.Status == delNormal, u => u.CreateTime, false).Select(n => new { n.ActivityID, n.ActivityDetailTime, n.Activity.ActivityName,n.Activity.ActivityStart,n.Activity.ActivityEnd}).ToList();
            var data = new { total = total, rows = pageData };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthentication(AbleOrganize = true, AbleUser = false)]
        public JsonResult GetActivityDetailByOrgId()
        {
            int pageSize = int.Parse(Request["limit"] ?? "5");
            int offset = int.Parse(Request["offset"] ?? "0");
            int pageIndex = (offset / pageSize) + 1;
            if (String.IsNullOrEmpty(Request["userId"]))
            {
                return Json(new { total = 0, rows = "" }, JsonRequestBehavior.AllowGet);
            }
            int orgId = Convert.ToInt32(Request["orgId"]);
            var pageData = ActivityDetailService.GetPageEntities(pageSize, pageIndex, out int total, u => u.ActivityID == orgId && u.Status == delNormal, u => u.CreateTime, false).Select(n => new { n.ActivityID, n.ActivityDetailTime, n.Activity.ActivityName, n.Activity.ActivityStart, n.Activity.ActivityEnd }).ToList();
            var data = new { total = total, rows = pageData };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        #endregion

    }
}