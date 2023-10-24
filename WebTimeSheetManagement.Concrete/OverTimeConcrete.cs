using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTimeSheetManagement.Interface;
using WebTimeSheetManagement.Models;
using System.Linq.Dynamic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace WebTimeSheetManagement.Concrete
{
    public class OverTimeConcrete : IOverTime
    {
        public int AddOverTimeMaster(OverTimeMaster OverTimeMaster)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.OverTimeMaster.Add(OverTimeMaster);
                    _context.SaveChanges();
                    int id = OverTimeMaster.OverTimeMasterID; // Yes it's here
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int AddOverTimeDetail(OverTimeDetails OverTimeDetails)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.OverTimeDetails.Add(OverTimeDetails);
                    _context.SaveChanges();
                    int id = OverTimeDetails.OverTimeID; // Yes it's here
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckIsDateAlreadyUsed(DateTime FromDate, int UserID)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var result = (from OverTimedetails in _context.OverTimeDetails
                                  where OverTimedetails.Period == FromDate && OverTimedetails.UserID == UserID
                                  select OverTimedetails).Count();

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<OverTimeMasterView> ShowOverTime(string sortColumn, string sortColumnDir, string Search, int UserID)
        {
            var _context = new DatabaseContext();

            var IQueryableOverTime = (from OverTimemaster in _context.OverTimeMaster

                                       where OverTimemaster.UserID == UserID
                                       select new OverTimeMasterView
                                       {
                                           OverTimeStatus = OverTimemaster.OverTimeStatus == 1 ? "Submitted" : OverTimemaster.OverTimeStatus == 2 ? "Approved" : "Rejected",
                                           Comment = OverTimemaster.Comment,
                                           OverTimeMasterID = OverTimemaster.OverTimeMasterID,
                                           FromDate = SqlFunctions.DateName("day", OverTimemaster.FromDate).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.FromDate.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.FromDate),
                                           ToDate = SqlFunctions.DateName("day", OverTimemaster.ToDate).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.ToDate.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.ToDate),

                                           CreatedOn = SqlFunctions.DateName("day", OverTimemaster.CreatedOn).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.CreatedOn.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.CreatedOn),
                                           TotalHours = OverTimemaster.TotalHours
                                       });

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IQueryableOverTime = IQueryableOverTime.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(Search))
            {
                IQueryableOverTime = IQueryableOverTime.Where(m => m.FromDate == Search);
            }

            return IQueryableOverTime;

        }

        public List<OverTimeDetailsView> OverTimeDetailsbyOverTimeMasterID(int UserID, int OverTimeMasterID)
        {
            using (var _context = new DatabaseContext())
            {
                var data = (from OverTime in _context.OverTimeDetails
                            join project in _context.ProjectMaster on OverTime.ProjectID equals project.ProjectID
                            where OverTime.UserID == UserID && OverTime.OverTimeMasterID == OverTimeMasterID
                            select new OverTimeDetailsView
                            {
                                OverTimeID = OverTime.OverTimeID,
                                CreatedOn = SqlFunctions.DateName("day", OverTime.CreatedOn).Trim() + "/" +
                    SqlFunctions.StringConvert((double)OverTime.CreatedOn.Value.Month).TrimStart() + "/" +
                    SqlFunctions.DateName("year", OverTime.CreatedOn),
                                Period = SqlFunctions.DateName("day", OverTime.Period).Trim() + "/" +
                    SqlFunctions.StringConvert((double)OverTime.Period.Value.Month).TrimStart() + "/" +
                    SqlFunctions.DateName("year", OverTime.Period),
                                DaysofWeek = OverTime.DaysofWeek,
                                Hours = OverTime.Hours,
                                ProjectName = project.ProjectName,
                                OverTimeMasterID = OverTime.OverTimeMasterID

                            }).ToList();

                return data;
            }
        }

        public List<OverTimeDetailsView> OverTimeDetailsbyOverTimeMasterID(int OverTimeMasterID)
        {
            using (var _context = new DatabaseContext())
            {
                var data = (from OverTime in _context.OverTimeDetails
                            join project in _context.ProjectMaster on OverTime.ProjectID equals project.ProjectID
                            where OverTime.OverTimeMasterID == OverTimeMasterID
                            select new OverTimeDetailsView
                            {
                                OverTimeID = OverTime.OverTimeID,
                                CreatedOn = SqlFunctions.DateName("day", OverTime.CreatedOn).Trim() + "/" +
                    SqlFunctions.StringConvert((double)OverTime.CreatedOn.Value.Month).TrimStart() + "/" +
                    SqlFunctions.DateName("year", OverTime.CreatedOn),
                                Period = SqlFunctions.DateName("day", OverTime.Period).Trim() + "/" +
                    SqlFunctions.StringConvert((double)OverTime.Period.Value.Month).TrimStart() + "/" +
                    SqlFunctions.DateName("year", OverTime.Period),
                                DaysofWeek = OverTime.DaysofWeek,
                                Hours = OverTime.Hours,
                                ProjectName = project.ProjectName,
                                OverTimeMasterID = OverTime.OverTimeMasterID

                            }).ToList();

                return data;
            }
        }

        public int DeleteOverTimeByOverTimeMasterID(int OverTimeMasterID, int UserID)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
                {
                    var param = new DynamicParameters();
                    param.Add("@OverTimeID", OverTimeMasterID);
                    param.Add("@UserID", UserID);
                    return con.Execute("Usp_DeleteOverTime", param, null, 0, System.Data.CommandType.StoredProcedure);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IQueryable<OverTimeMasterView> ShowAllOverTime(string sortColumn, string sortColumnDir, string Search, int UserID)
        {
            var _context = new DatabaseContext();

            var IQueryableOverTime = (from OverTimemaster in _context.OverTimeMaster
                                       join registration in _context.Registration on OverTimemaster.UserID equals registration.RegistrationID
                                       join AssignedRolesAdmin in _context.AssignedRoles on registration.RegistrationID equals AssignedRolesAdmin.RegistrationID
                                       where AssignedRolesAdmin.AssignToAdmin == UserID
                                       select new OverTimeMasterView
                                       {
                                           OverTimeStatus = OverTimemaster.OverTimeStatus == 1 ? "Submitted" : OverTimemaster.OverTimeStatus == 2 ? "Approved" : "Rejected",
                                           Comment = OverTimemaster.Comment,
                                           OverTimeMasterID = OverTimemaster.OverTimeMasterID,
                                           FromDate =
                (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.FromDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.FromDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.FromDate))), 2)
                       ).Replace(" ", "0"),


                                           ToDate =
                     (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.ToDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.ToDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.ToDate))), 2)
                       ).Replace(" ", "0"),

                                           CreatedOn = SqlFunctions.DateName("day", OverTimemaster.CreatedOn).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.CreatedOn.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.CreatedOn),
                                           TotalHours = OverTimemaster.TotalHours,
                                           Username = registration.Username,
                                           SubmittedMonth = SqlFunctions.DateName("MONTH", OverTimemaster.ToDate).ToString()




                                       });

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IQueryableOverTime = IQueryableOverTime.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(Search))
            {
                IQueryableOverTime = IQueryableOverTime.Where(m => m.FromDate == Search || m.Username == Search);
            }

            return IQueryableOverTime;

        }

        public List<GetPeriods> GetPeriodsbyOverTimeMasterID(int OverTimeMasterID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
            {
                con.Open();
                try
                {
                    var param = new DynamicParameters();
                    param.Add("@OverTimeMasterID", OverTimeMasterID);
                    var result = con.Query<GetPeriods>("Usp_GetPeriodsbyOverTimeMasterID", param, null, true, 0, System.Data.CommandType.StoredProcedure).ToList();
                    if (result.Count > 0)
                    {
                        return result;
                    }
                    else
                    {
                        return new List<GetPeriods>();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<GetProjectNames> GetProjectNamesbyOverTimeMasterID(int OverTimeMasterID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
            {
                con.Open();
                try
                {
                    var param = new DynamicParameters();
                    param.Add("@OverTimeMasterID", OverTimeMasterID);
                    var result = con.Query<GetProjectNames>("Usp_GetProjectNamesbyOverTimeMasterID", param, null, true, 0, System.Data.CommandType.StoredProcedure).ToList();
                    if (result.Count > 0)
                    {
                        return result;
                    }
                    else
                    {
                        return new List<GetProjectNames>();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool UpdateOverTimeStatus(OverTimeApproval OverTimeapprovalmodel, int Status)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
            {
                con.Open();
                SqlTransaction sql = con.BeginTransaction();

                try
                {
                    var param = new DynamicParameters();
                    param.Add("@OverTimeMasterID", OverTimeapprovalmodel.OverTimeMasterID);
                    param.Add("@Comment", OverTimeapprovalmodel.Comment);
                    param.Add("@OverTimeStatus", Status);
                    var result = con.Execute("Usp_UpdateOverTimeStatus", param, sql, 0, System.Data.CommandType.StoredProcedure);
                    if (result > 0)
                    {
                        sql.Commit();
                        return true;
                    }
                    else
                    {
                        sql.Rollback();
                        return false;
                    }
                }
                catch (Exception)
                {
                    sql.Rollback();
                    throw;
                }
            }
        }

        public void InsertOverTimeAuditLog(OverTimeAuditTB OverTimeaudittb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.OverTimeAuditTB.Add(OverTimeaudittb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int DeleteOverTimeByOnlyOverTimeMasterID(int OverTimeMasterID)
        {
            int resultOverTimeMaster = 0;
            int resultOverTimeDetails = 0;
            try
            {
                using (var _context = new DatabaseContext())
                {

                    var OverTimecount = (from ex in _context.OverTimeMaster
                                          where ex.OverTimeMasterID == OverTimeMasterID
                                          select ex).Count();

                    if (OverTimecount > 0)
                    {
                        OverTimeMaster OverTime = (from ex in _context.OverTimeMaster
                                                     where ex.OverTimeMasterID == OverTimeMasterID
                                                     select ex).SingleOrDefault();

                        _context.OverTimeMaster.Remove(OverTime);
                        resultOverTimeMaster = _context.SaveChanges();
                    }

                    var OverTimedetailscount = (from ex in _context.OverTimeDetails
                                                 where ex.OverTimeMasterID == OverTimeMasterID
                                                 select ex).Count();

                    if (OverTimedetailscount > 0)
                    {

                        var OverTimedetails = (from ex in _context.OverTimeDetails
                                                where ex.OverTimeMasterID == OverTimeMasterID
                                                select ex).ToList();

                        _context.OverTimeDetails.RemoveRange(OverTimedetails);
                        resultOverTimeDetails = _context.SaveChanges();

                    }

                    if (resultOverTimeMaster > 0 || resultOverTimeDetails > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int? InsertDescription(DescriptionTB DescriptionTB)
        {
            using (var _context = new DatabaseContext())
            {
                _context.DescriptionTB.Add(DescriptionTB);
                _context.SaveChanges();
                int? id = DescriptionTB.DescriptionID; // Yes it's here
                return id;
            }
        }

        public DisplayViewModel GetOverTimesCountByAdminID(string AdminID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
            {
                var param = new DynamicParameters();
                param.Add("@AdminID", AdminID);
                return con.Query<DisplayViewModel>("Usp_GetOverTimesCountByAdminID", param, null, true, 0, System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IQueryable<OverTimeMasterView> ShowAllApprovedOverTime(string sortColumn, string sortColumnDir, string Search, int UserID)
        {
            var _context = new DatabaseContext();

            var IQueryableOverTime = (from OverTimemaster in _context.OverTimeMaster
                                       join OverTimeAuditTB in _context.OverTimeAuditTB on OverTimemaster.OverTimeMasterID equals OverTimeAuditTB.OverTimeID
                                       join registration in _context.Registration on OverTimemaster.UserID equals registration.RegistrationID
                                       join AssignedRolesAdmin in _context.AssignedRoles on registration.RegistrationID equals AssignedRolesAdmin.RegistrationID
                                       where AssignedRolesAdmin.AssignToAdmin == UserID && OverTimeAuditTB.Status == 2
                                       select new OverTimeMasterView
                                       {
                                           OverTimeStatus = OverTimemaster.OverTimeStatus == 1 ? "Submitted" : OverTimemaster.OverTimeStatus == 2 ? "Approved" : "Rejected",
                                           Comment = OverTimemaster.Comment,
                                           OverTimeMasterID = OverTimemaster.OverTimeMasterID,
                                           FromDate =
                (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.FromDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.FromDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.FromDate))), 2)
                       ).Replace(" ", "0"),


                                           ToDate =
                     (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.ToDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.ToDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.ToDate))), 2)
                       ).Replace(" ", "0"),

                                           CreatedOn = SqlFunctions.DateName("day", OverTimemaster.CreatedOn).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.CreatedOn.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.CreatedOn),
                                           TotalHours = OverTimemaster.TotalHours,
                                           Username = registration.Username,
                                           SubmittedMonth = SqlFunctions.DateName("MONTH", OverTimemaster.ToDate).ToString()




                                       });

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IQueryableOverTime = IQueryableOverTime.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(Search))
            {
                IQueryableOverTime = IQueryableOverTime.Where(m => m.FromDate == Search || m.Username == Search);
            }

            return IQueryableOverTime;

        }

        public IQueryable<OverTimeMasterView> ShowAllRejectOverTime(string sortColumn, string sortColumnDir, string Search, int UserID)
        {
            var _context = new DatabaseContext();

            var IQueryableOverTime = (from OverTimemaster in _context.OverTimeMaster
                                       join OverTimeAuditTB in _context.OverTimeAuditTB on OverTimemaster.OverTimeMasterID equals OverTimeAuditTB.OverTimeID
                                       join registration in _context.Registration on OverTimemaster.UserID equals registration.RegistrationID
                                       join AssignedRolesAdmin in _context.AssignedRoles on registration.RegistrationID equals AssignedRolesAdmin.RegistrationID
                                       where AssignedRolesAdmin.AssignToAdmin == UserID && OverTimeAuditTB.Status == 3
                                       select new OverTimeMasterView
                                       {
                                           OverTimeStatus = OverTimemaster.OverTimeStatus == 1 ? "Submitted" : OverTimemaster.OverTimeStatus == 2 ? "Approved" : "Rejected",
                                           Comment = OverTimemaster.Comment,
                                           OverTimeMasterID = OverTimemaster.OverTimeMasterID,
                                           FromDate =
                (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.FromDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.FromDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.FromDate))), 2)
                       ).Replace(" ", "0"),


                                           ToDate =
                     (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.ToDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.ToDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.ToDate))), 2)
                       ).Replace(" ", "0"),

                                           CreatedOn = SqlFunctions.DateName("day", OverTimemaster.CreatedOn).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.CreatedOn.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.CreatedOn),
                                           TotalHours = OverTimemaster.TotalHours,
                                           Username = registration.Username,
                                           SubmittedMonth = SqlFunctions.DateName("MONTH", OverTimemaster.ToDate).ToString()




                                       });

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IQueryableOverTime = IQueryableOverTime.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(Search))
            {
                IQueryableOverTime = IQueryableOverTime.Where(m => m.FromDate == Search || m.Username == Search);
            }

            return IQueryableOverTime;

        }

        public IQueryable<OverTimeMasterView> ShowAllSubmittedOverTime(string sortColumn, string sortColumnDir, string Search, int UserID)
        {
            var _context = new DatabaseContext();

            var IQueryableOverTime = (from OverTimemaster in _context.OverTimeMaster
                                       join OverTimeAuditTB in _context.OverTimeAuditTB on OverTimemaster.OverTimeMasterID equals OverTimeAuditTB.OverTimeID
                                       join registration in _context.Registration on OverTimemaster.UserID equals registration.RegistrationID
                                       join AssignedRolesAdmin in _context.AssignedRoles on registration.RegistrationID equals AssignedRolesAdmin.RegistrationID
                                       where AssignedRolesAdmin.AssignToAdmin == UserID && OverTimeAuditTB.Status == 1
                                       select new OverTimeMasterView
                                       {
                                           OverTimeStatus = OverTimemaster.OverTimeStatus == 1 ? "Submitted" : OverTimemaster.OverTimeStatus == 2 ? "Approved" : "Rejected",
                                           Comment = OverTimemaster.Comment,
                                           OverTimeMasterID = OverTimemaster.OverTimeMasterID,
                                           FromDate =
                (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.FromDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.FromDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.FromDate))), 2)
                       ).Replace(" ", "0"),


                                           ToDate =
                     (
                     EntityFunctions.Right(SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("yyyy", OverTimemaster.ToDate)), 4)

                                            + "-"
                    + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("mm", OverTimemaster.ToDate))), 2)
                        + "-"
                        + EntityFunctions.Right(String.Concat(" ", SqlFunctions.StringConvert((double?)SqlFunctions.DatePart("dd", OverTimemaster.ToDate))), 2)
                       ).Replace(" ", "0"),

                                           CreatedOn = SqlFunctions.DateName("day", OverTimemaster.CreatedOn).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.CreatedOn.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.CreatedOn),
                                           TotalHours = OverTimemaster.TotalHours,
                                           Username = registration.Username,
                                           SubmittedMonth = SqlFunctions.DateName("MONTH", OverTimemaster.ToDate).ToString()




                                       });

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IQueryableOverTime = IQueryableOverTime.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(Search))
            {
                IQueryableOverTime = IQueryableOverTime.Where(m => m.FromDate == Search || m.Username == Search);
            }

            return IQueryableOverTime;

        }

        public DisplayViewModel GetOverTimesCountByUserID(string UserID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
            {
                var param = new DynamicParameters();
                param.Add("@UserID", UserID);
                return con.Query<DisplayViewModel>("Usp_GetOverTimeCountByUserID", param, null, true, 0, System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IQueryable<OverTimeMasterView> ShowOverTimeStatus(string sortColumn, string sortColumnDir, string Search, int UserID, int OverTimeStatus)
        {
            var _context = new DatabaseContext();

            var IQueryableOverTime = (from OverTimemaster in _context.OverTimeMaster
                                       where OverTimemaster.UserID == UserID && OverTimemaster.OverTimeStatus == OverTimeStatus
                                       select new OverTimeMasterView
                                       {
                                           OverTimeStatus = OverTimemaster.OverTimeStatus == 1 ? "Submitted" : OverTimemaster.OverTimeStatus == 2 ? "Approved" : "Rejected",
                                           Comment = OverTimemaster.Comment,
                                           OverTimeMasterID = OverTimemaster.OverTimeMasterID,
                                           FromDate = SqlFunctions.DateName("day", OverTimemaster.FromDate).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.FromDate.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.FromDate),
                                           ToDate = SqlFunctions.DateName("day", OverTimemaster.ToDate).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.ToDate.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.ToDate),

                                           CreatedOn = SqlFunctions.DateName("day", OverTimemaster.CreatedOn).Trim() + "/" +
                   SqlFunctions.StringConvert((double)OverTimemaster.CreatedOn.Value.Month).TrimStart() + "/" +
                   SqlFunctions.DateName("year", OverTimemaster.CreatedOn),
                                           TotalHours = OverTimemaster.TotalHours
                                       });

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                IQueryableOverTime = IQueryableOverTime.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(Search))
            {
                IQueryableOverTime = IQueryableOverTime.Where(m => m.FromDate == Search);
            }

            return IQueryableOverTime;

        }

        public bool UpdateOverTimeAuditStatus(int OverTimeID, string Comment, int Status)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TimeSheetDBEntities"].ToString()))
            {
                con.Open();
                SqlTransaction sql = con.BeginTransaction();

                try
                {
                    var param = new DynamicParameters();
                    param.Add("@OverTimeID", OverTimeID);
                    param.Add("@Comment", Comment);
                    param.Add("@Status", Status);
                    var result = con.Execute("Usp_ChangeOverTimeStatus", param, sql, 0, System.Data.CommandType.StoredProcedure);
                    if (result > 0)
                    {
                        sql.Commit();
                        return true;
                    }
                    else
                    {
                        sql.Rollback();
                        return false;
                    }
                }
                catch (Exception)
                {
                    sql.Rollback();
                    throw;
                }
            }
        }

        public bool IsOverTimeALreadyProcessed(int OverTimeID)
        {
            using (var _context = new DatabaseContext())
            {
                var data = (from OverTime in _context.OverTimeAuditTB
                            where OverTime.OverTimeID == OverTimeID && OverTime.Status != 1
                            select OverTime).Count();

                if (data > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

       
    }
}
