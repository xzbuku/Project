﻿using OjVolunteer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OjVolunteer.BLL
{
    public partial class ActivityService
    {
        short delInvalid = (short)Model.Enum.DelFlagEnum.Invalid;
        short delNormal = (short)Model.Enum.DelFlagEnum.Normal;

        //政治面貌编号
        short polParty = (short)Model.Enum.PoliticalEnum.Party;
        short polPreparatory = (short)Model.Enum.PoliticalEnum.Preparatory;

        public Boolean AddTime(int actId)
        {
            bool flag = false;
            try
            {
                short delNormal = (short)Model.Enum.DelFlagEnum.Normal;
                short delDoneAuditing = (short)Model.Enum.DelFlagEnum.DoneAuditing;
                Activity activity = DbSession.ActivityDal.GetEntities(u => u.ActivityID == actId&&u.Status == delDoneAuditing).FirstOrDefault();
                var EnrollList = DbSession.UserEnrollDal.GetEntities(u=>u.ActivityID ==actId&&u.Status==delNormal).AsQueryable();
                foreach (var Enroll in EnrollList)
                {
                    //添加活动详情
                    ActivityDetail activityDetail = new ActivityDetail();
                    activityDetail.Status = (short)Model.Enum.DelFlagEnum.Normal;
                    activityDetail.UserInfoId = Enroll.UserInfoID;
                    activityDetail.ActivityID = Enroll.ActivityID;
                    activityDetail.CreateTime = DateTime.Now;
                    activityDetail.ModfiedOn = activityDetail.CreateTime;
                    activityDetail.ActivityDetailTime = (decimal)Enroll.ActivityTime;
                    DbSession.ActivityDetailDal.Add(activityDetail);

                    //用户总时长累积
                    UserDuration userDuration = DbSession.UserDurationDal.GetEntities(u => u.UserDurationID == Enroll.UserInfoID).FirstOrDefault();

                    //政治面貌
                    int pId = DbSession.UserInfoDal.GetEntities(u => u.UserInfoID == userDuration.UserDurationID).FirstOrDefault().PoliticalID;

                    //总时长
                    userDuration.UserDurationTotal = userDuration.UserDurationTotal + (decimal)Enroll.ActivityTime;

                    //用户为党员
                    if (pId == polParty)
                    {
                        userDuration.UserDurationPartyTotal = userDuration.UserDurationPartyTotal + (decimal)Enroll.ActivityTime;
                    }
                    //用户为预备党员
                    else if (pId == polPreparatory)
                    {
                        userDuration.UserDurationPropartyTotal = userDuration.UserDurationPropartyTotal + (decimal)Enroll.ActivityTime;
                        //50小时志愿者徽章
                        if (userDuration.UserDurationPropartyTotal >= 50&& DbSession.UserBadgeDal.GetEntities(u => u.BadgeID == 1 && u.UserInfoID == userDuration.UserDurationID).FirstOrDefault() == null)
                        {
                            UserBadge userBadge = new UserBadge
                            {
                                //徽章ID
                                BadgeID = 1,
                                UserInfoID = userDuration.UserDurationID,
                                CreateTime = DateTime.Now
                            };
                            userDuration.Status = delNormal;
                            DbSession.UserBadgeDal.Add(userBadge);
                        }
                    }
                    else
                        userDuration.UserDurationNormalTotal = userDuration.UserDurationNormalTotal + (decimal)Enroll.ActivityTime;
                    DbSession.UserDurationDal.Update(userDuration);
                }
                activity.Status = delNormal;
                activity.ModfiedOn = DateTime.Now;
                DbSession.ActivityDal.Update(activity);
                DbSession.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }

        public Boolean AddActivity(Activity activity)
        {
            CurrentDal.Add(activity);
            if (!(DbSession.SaveChanges() > 0))
                return false;
            UserEnroll userEnroll = new UserEnroll()
            {
                ActivityID = activity.ActivityID,
                UserInfoID = activity.ActivityManagerID,
                UserEnrollStart = activity.ActivityEnrollStart,
                ModfiedOn = DateTime.Now,
                CreateTime = DateTime.Now,
                Status = delInvalid,
            };
            DbSession.UserEnrollDal.Add(userEnroll);
            var org = DbSession.OrganizeInfoDal.GetEntities(u => u.OrganizeInfoID == activity.ActivityApplyOrganizeID).FirstOrDefault();
            org.ActivityCount++;
            DbSession.OrganizeInfoDal.Update(org);
            if (DbSession.SaveChanges() > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 添加以往已完成的活动  仅测试用
        /// </summary>
        /// <param name="activity">活动</param>
        /// <param name="ids">参与人Id</param>
        /// <returns></returns>
        public Boolean AddBeforeActivity(Activity activity, List<int> ids)
        {
            CurrentDal.Add(activity);
            if (!(DbSession.SaveChanges() > 0))
                return false;
            TimeSpan timeSpan = (TimeSpan)(activity.ActivityEnd - activity.ActivityStart);
            double Time = timeSpan.Hours + (double)timeSpan.Minutes / 60;
            //集体报名 集体及时
            foreach (var id in ids)
            {
                UserEnroll userEnroll = new UserEnroll()
                {
                    ActivityID = activity.ActivityID,
                    UserInfoID = id,
                    UserEnrollStart = activity.ActivityEnrollStart,
                    UserEnrollActivityStart = activity.ActivityStart,
                    UserEnrollActivityEnd = activity.ActivityEnd,
                    ActivityTime = (decimal)Time,
                    CreateTime = DateTime.Now,
                    Status = delNormal,
                };
                DbSession.UserEnrollDal.Add(userEnroll);
            } 

            var org = DbSession.OrganizeInfoDal.GetEntities(u => u.OrganizeInfoID == activity.ActivityApplyOrganizeID).FirstOrDefault();
            org.ActivityCount++;
            DbSession.OrganizeInfoDal.Update(org);
            if (!(DbSession.SaveChanges() > 0))
                return false;
            return AddTime(activity.ActivityID);
        }
    }
}
