﻿ 
namespace OjVolunteer.IDAL
{
    public partial interface IDbSession
    {   
	 
		IActivityDal ActivityDal { get;}
	 
		IActivityDetailDal ActivityDetailDal { get;}
	 
		IActivityTypeDal ActivityTypeDal { get;}
	 
		IBadgeDal BadgeDal { get;}
	 
		IDepartmentDal DepartmentDal { get;}
	 
		IFavorsDal FavorsDal { get;}
	 
		IIntegralsDal IntegralsDal { get;}
	 
		IMajorDal MajorDal { get;}
	 
		IOrganizeInfoDal OrganizeInfoDal { get;}
	 
		IPoliticalDal PoliticalDal { get;}
	 
		ITalksDal TalksDal { get;}
	 
		IUserBadgeDal UserBadgeDal { get;}
	 
		IUserDurationDal UserDurationDal { get;}
	 
		IUserEnrollDal UserEnrollDal { get;}
	 
		IUserInfoDal UserInfoDal { get;}
	 
		Iv_User_ActDetailDal v_User_ActDetailDal { get;}
	}
}