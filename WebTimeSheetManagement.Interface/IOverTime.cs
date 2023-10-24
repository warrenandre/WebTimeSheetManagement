using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTimeSheetManagement.Models;

namespace WebTimeSheetManagement.Interface
{
    public interface IOverTime
    {
        int AddOverTimeMaster(OverTimeMaster OverTimeMaster);
        int AddOverTimeDetail(OverTimeDetails OverTimeDetails);
        bool CheckIsDateAlreadyUsed(DateTime FromDate, int UserID);
        IQueryable<OverTimeMasterView> ShowOverTime(string sortColumn, string sortColumnDir, string Search, int UserID);
        List<OverTimeDetailsView> OverTimeDetailsbyOverTimeMasterID(int UserID, int OverTimeMasterID);
        int DeleteOverTimeByOverTimeMasterID(int OverTimeMasterID, int UserID);
        IQueryable<OverTimeMasterView> ShowAllOverTime(string sortColumn, string sortColumnDir, string Search, int UserID);
        List<OverTimeDetailsView> OverTimeDetailsbyOverTimeMasterID(int OverTimeMasterID);
        List<GetPeriods> GetPeriodsbyOverTimeMasterID(int OverTimeMasterID);
        List<GetProjectNames> GetProjectNamesbyOverTimeMasterID(int OverTimeMasterID);
        bool UpdateOverTimeStatus(OverTimeApproval OverTimeapprovalmodel, int Status);
        void InsertOverTimeAuditLog(OverTimeAuditTB OverTimeaudittb);
        int DeleteOverTimeByOnlyOverTimeMasterID(int OverTimeMasterID);
        int? InsertDescription(DescriptionTB DescriptionTB);
        DisplayViewModel GetOverTimesCountByAdminID(string AdminID);
        IQueryable<OverTimeMasterView> ShowAllApprovedOverTime(string sortColumn, string sortColumnDir, string Search, int UserID);
        IQueryable<OverTimeMasterView> ShowAllRejectOverTime(string sortColumn, string sortColumnDir, string Search, int UserID);
        IQueryable<OverTimeMasterView> ShowAllSubmittedOverTime(string sortColumn, string sortColumnDir, string Search, int UserID);
        DisplayViewModel GetOverTimesCountByUserID(string UserID);
        IQueryable<OverTimeMasterView> ShowOverTimeStatus(string sortColumn, string sortColumnDir, string Search, int UserID, int OverTimeStatus);
        bool UpdateOverTimeAuditStatus(int OverTimeID, string Comment, int Status);
        bool IsOverTimeALreadyProcessed(int OverTimeID);
    }
}
