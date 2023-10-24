using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTimeSheetManagement.Concrete;
using WebTimeSheetManagement.Filters;
using WebTimeSheetManagement.Interface;
using WebTimeSheetManagement.Models;

namespace WebTimeSheetManagement.Controllers
{
    [ValidateUserSession]
    public class OverTimeController : Controller
    {
        IProject _IProject;
        IOverTime _IOverTime;
        IUsers _IUsers;
        public OverTimeController()
        {
            _IProject = new ProjectConcrete();
            _IOverTime = new OverTimeConcrete();
            _IUsers = new UsersConcrete();
        }

        // GET: OverTime
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(OverTimeModel overtimemodel)
        {
            try
            {
                if (overtimemodel == null)
                {
                    ModelState.AddModelError("", "Values Posted Are Not Accurate");
                    return View();
                }

                OverTimeMaster objOverTimemaster = new OverTimeMaster();
                objOverTimemaster.OverTimeMasterID = 0;
                objOverTimemaster.UserID = Convert.ToInt32(Session["UserID"]);
                objOverTimemaster.CreatedOn = DateTime.Now;
                objOverTimemaster.FromDate = overtimemodel.hdtext1;
                objOverTimemaster.ToDate = overtimemodel.hdtext7;
                objOverTimemaster.TotalHours = CalculateTotalHours(overtimemodel);
                objOverTimemaster.OverTimeStatus = 1;
                int OverTimeMasterID = _IOverTime.AddOverTimeMaster(objOverTimemaster);

                var count = ProjectSelectCount(overtimemodel);

                if (OverTimeMasterID > 0)
                {
                    Save(overtimemodel, OverTimeMasterID);
                    SaveDescription(overtimemodel, OverTimeMasterID);
                    _IOverTime.InsertOverTimeAuditLog(InsertOverTimeAudit(OverTimeMasterID, 1));
                }

                TempData["TimeCardMessage"] = "Data Saved Successfully";

                return RedirectToAction("Add", "OverTime");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult ListofProjects()
        {
            try
            {
                var listofProjects = _IProject.GetListofProjects();
                return Json(listofProjects, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        private int? CalculateTotalHours(OverTimeModel OverTimeModel)
        {
            try
            {
                int? Total = 0;
                var val1 = OverTimeModel.texttotal_p1 == null ? 0 : OverTimeModel.texttotal_p1;
                var val2 = OverTimeModel.texttotal_p2 == null ? 0 : OverTimeModel.texttotal_p2;
                var val3 = OverTimeModel.texttotal_p3 == null ? 0 : OverTimeModel.texttotal_p3;
                var val4 = OverTimeModel.texttotal_p4 == null ? 0 : OverTimeModel.texttotal_p4;
                var val5 = OverTimeModel.texttotal_p5 == null ? 0 : OverTimeModel.texttotal_p5;
                var val6 = OverTimeModel.texttotal_p6 == null ? 0 : OverTimeModel.texttotal_p6;
                Total = val1 + val2 + val3 + val4 + val5 + val6;
                return Total;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        private void SaveOverTimeDetail(string DaysofWeek, int? Hours, DateTime? Period, int? ProjectID, int OverTimeMasterID)
        {
            try
            {
                OverTimeDetails objOverTimedetails = new OverTimeDetails();
                objOverTimedetails.OverTimeID = 0;
                objOverTimedetails.DaysofWeek = DaysofWeek;
                objOverTimedetails.Hours = Hours == null ? 0 : Hours;
                objOverTimedetails.Period = Period;
                objOverTimedetails.ProjectID = ProjectID;
                objOverTimedetails.UserID = Convert.ToInt32(Session["UserID"]);
                objOverTimedetails.CreatedOn = DateTime.Now;
                objOverTimedetails.OverTimeMasterID = OverTimeMasterID;
                int OverTimeID = _IOverTime.AddOverTimeDetail(objOverTimedetails);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult CheckIsDateAlreadyUsed(DateTime FromDate)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(FromDate)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IOverTime.CheckIsDateAlreadyUsed(FromDate, Convert.ToInt32(Session["UserID"]));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult Delete(int OverTimeMasterID)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(OverTimeMasterID)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IOverTime.DeleteOverTimeByOverTimeMasterID(OverTimeMasterID, Convert.ToInt32(Session["UserID"]));

                if (data > 0)
                {
                    return Json(data: true, behavior: JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(data: false, behavior: JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int ProjectSelectCount(OverTimeModel OverTimemodel)
        {
            try
            {
                int count = 0;
                if (OverTimemodel.ProjectID1 != null && (OverTimemodel.texttotal_p1 != null && OverTimemodel.texttotal_p1 != 0))
                {
                    count = count + 1;
                }

                if (OverTimemodel.ProjectID2 != null && (OverTimemodel.texttotal_p2 != null && OverTimemodel.texttotal_p2 != 0))
                {
                    count = count + 1;
                }

                if (OverTimemodel.ProjectID3 != null && (OverTimemodel.texttotal_p3 != null && OverTimemodel.texttotal_p3 != 0))
                {
                    count = count + 1;
                }

                if (OverTimemodel.ProjectID3 != null && (OverTimemodel.texttotal_p3 != null && OverTimemodel.texttotal_p3 != 0))
                {
                    count = count + 1;
                }

                if (OverTimemodel.ProjectID4 != null && (OverTimemodel.texttotal_p4 != null && OverTimemodel.texttotal_p4 != 0))
                {
                    count = count + 1;
                }

                if (OverTimemodel.ProjectID5 != null && (OverTimemodel.texttotal_p5 != null && OverTimemodel.texttotal_p5 != 0))
                {
                    count = count + 1;
                }

                if (OverTimemodel.ProjectID6 != null && (OverTimemodel.texttotal_p6 != null && OverTimemodel.texttotal_p6 != 0))
                {
                    count = count + 1;
                }

                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Save(OverTimeModel OverTimemodel, int OverTimeMasterID)
        {
            try
            {
                if (OverTimemodel.ProjectID1 != null && (OverTimemodel.texttotal_p1 != null && OverTimemodel.texttotal_p1 != 0))
                {
                    #region Project 1
                    var date1 = OverTimemodel.hdtext1;
                    var DaysofWeek1 = OverTimemodel.DaysofWeek1;
                    var value1 = OverTimemodel.text1_p1;
                    SaveOverTimeDetail(DaysofWeek1, value1, date1, OverTimemodel.ProjectID1, OverTimeMasterID);

                    var date2 = OverTimemodel.hdtext2;
                    var DaysofWeek2 = OverTimemodel.DaysofWeek2;
                    var value2 = OverTimemodel.text2_p1;
                    SaveOverTimeDetail(DaysofWeek2, value2, date2, OverTimemodel.ProjectID1, OverTimeMasterID);

                    var date3 = OverTimemodel.hdtext3;
                    var DaysofWeek3 = OverTimemodel.DaysofWeek3;
                    var value3 = OverTimemodel.text3_p1;
                    SaveOverTimeDetail(DaysofWeek3, value3, date3, OverTimemodel.ProjectID1, OverTimeMasterID);

                    var date4 = OverTimemodel.hdtext4;
                    var DaysofWeek4 = OverTimemodel.DaysofWeek4;
                    var value4 = OverTimemodel.text4_p1;
                    SaveOverTimeDetail(DaysofWeek4, value4, date4, OverTimemodel.ProjectID1, OverTimeMasterID);

                    var date5 = OverTimemodel.hdtext5;
                    var DaysofWeek5 = OverTimemodel.DaysofWeek5;
                    var value5 = OverTimemodel.text5_p1;
                    SaveOverTimeDetail(DaysofWeek5, value5, date5, OverTimemodel.ProjectID1, OverTimeMasterID);

                    var date6 = OverTimemodel.hdtext6;
                    var DaysofWeek6 = OverTimemodel.DaysofWeek6;
                    var value6 = OverTimemodel.text6_p1;
                    SaveOverTimeDetail(DaysofWeek6, value6, date6, OverTimemodel.ProjectID1, OverTimeMasterID);

                    var date7 = OverTimemodel.hdtext7;
                    var DaysofWeek7 = OverTimemodel.DaysofWeek7;
                    var value7 = OverTimemodel.text7_p1;
                    SaveOverTimeDetail(DaysofWeek7, value7, date7, OverTimemodel.ProjectID1, OverTimeMasterID);


                    #endregion
                }

                if (OverTimemodel.ProjectID2 != null && (OverTimemodel.texttotal_p2 != null && OverTimemodel.texttotal_p2 != 0))
                {
                    #region Project 2
                    var date1 = OverTimemodel.hdtext1;
                    var DaysofWeek1 = OverTimemodel.DaysofWeek1;
                    var value1 = OverTimemodel.text1_p2;
                    SaveOverTimeDetail(DaysofWeek1, value1, date1, OverTimemodel.ProjectID2, OverTimeMasterID);

                    var date2 = OverTimemodel.hdtext2;
                    var DaysofWeek2 = OverTimemodel.DaysofWeek2;
                    var value2 = OverTimemodel.text2_p2;
                    SaveOverTimeDetail(DaysofWeek2, value2, date2, OverTimemodel.ProjectID2, OverTimeMasterID);

                    var date3 = OverTimemodel.hdtext3;
                    var DaysofWeek3 = OverTimemodel.DaysofWeek3;
                    var value3 = OverTimemodel.text3_p2;
                    SaveOverTimeDetail(DaysofWeek3, value3, date3, OverTimemodel.ProjectID2, OverTimeMasterID);

                    var date4 = OverTimemodel.hdtext4;
                    var DaysofWeek4 = OverTimemodel.DaysofWeek4;
                    var value4 = OverTimemodel.text4_p2;
                    SaveOverTimeDetail(DaysofWeek4, value4, date4, OverTimemodel.ProjectID2, OverTimeMasterID);

                    var date5 = OverTimemodel.hdtext5;
                    var DaysofWeek5 = OverTimemodel.DaysofWeek5;
                    var value5 = OverTimemodel.text5_p2;
                    SaveOverTimeDetail(DaysofWeek5, value5, date5, OverTimemodel.ProjectID2, OverTimeMasterID);

                    var date6 = OverTimemodel.hdtext6;
                    var DaysofWeek6 = OverTimemodel.DaysofWeek6;
                    var value6 = OverTimemodel.text6_p2;
                    SaveOverTimeDetail(DaysofWeek6, value6, date6, OverTimemodel.ProjectID2, OverTimeMasterID);

                    var date7 = OverTimemodel.hdtext7;
                    var DaysofWeek7 = OverTimemodel.DaysofWeek7;
                    var value7 = OverTimemodel.text7_p2;
                    SaveOverTimeDetail(DaysofWeek7, value7, date7, OverTimemodel.ProjectID2, OverTimeMasterID);


                    #endregion
                }

                if (OverTimemodel.ProjectID3 != null && (OverTimemodel.texttotal_p3 != null && OverTimemodel.texttotal_p3 != 0))
                {
                    #region Project 3
                    var date1 = OverTimemodel.hdtext1;
                    var DaysofWeek1 = OverTimemodel.DaysofWeek1;
                    var value1 = OverTimemodel.text1_p3;
                    SaveOverTimeDetail(DaysofWeek1, value1, date1, OverTimemodel.ProjectID3, OverTimeMasterID);

                    var date2 = OverTimemodel.hdtext2;
                    var DaysofWeek2 = OverTimemodel.DaysofWeek2;
                    var value2 = OverTimemodel.text2_p3;
                    SaveOverTimeDetail(DaysofWeek2, value2, date2, OverTimemodel.ProjectID3, OverTimeMasterID);

                    var date3 = OverTimemodel.hdtext3;
                    var DaysofWeek3 = OverTimemodel.DaysofWeek3;
                    var value3 = OverTimemodel.text3_p3;
                    SaveOverTimeDetail(DaysofWeek3, value3, date3, OverTimemodel.ProjectID3, OverTimeMasterID);

                    var date4 = OverTimemodel.hdtext4;
                    var DaysofWeek4 = OverTimemodel.DaysofWeek4;
                    var value4 = OverTimemodel.text4_p3;
                    SaveOverTimeDetail(DaysofWeek4, value4, date4, OverTimemodel.ProjectID3, OverTimeMasterID);

                    var date5 = OverTimemodel.hdtext5;
                    var DaysofWeek5 = OverTimemodel.DaysofWeek5;
                    var value5 = OverTimemodel.text5_p3;
                    SaveOverTimeDetail(DaysofWeek5, value5, date5, OverTimemodel.ProjectID3, OverTimeMasterID);

                    var date6 = OverTimemodel.hdtext6;
                    var DaysofWeek6 = OverTimemodel.DaysofWeek6;
                    var value6 = OverTimemodel.text6_p3;
                    SaveOverTimeDetail(DaysofWeek6, value6, date6, OverTimemodel.ProjectID3, OverTimeMasterID);

                    var date7 = OverTimemodel.hdtext7;
                    var DaysofWeek7 = OverTimemodel.DaysofWeek7;
                    var value7 = OverTimemodel.text7_p3;
                    SaveOverTimeDetail(DaysofWeek7, value7, date7, OverTimemodel.ProjectID3, OverTimeMasterID);


                    #endregion
                }

                if (OverTimemodel.ProjectID4 != null && (OverTimemodel.texttotal_p4 != null && OverTimemodel.texttotal_p4 != 0))
                {
                    #region Project 4
                    var date1 = OverTimemodel.hdtext1;
                    var DaysofWeek1 = OverTimemodel.DaysofWeek1;
                    var value1 = OverTimemodel.text1_p4;
                    SaveOverTimeDetail(DaysofWeek1, value1, date1, OverTimemodel.ProjectID4, OverTimeMasterID);

                    var date2 = OverTimemodel.hdtext2;
                    var DaysofWeek2 = OverTimemodel.DaysofWeek2;
                    var value2 = OverTimemodel.text2_p4;
                    SaveOverTimeDetail(DaysofWeek2, value2, date2, OverTimemodel.ProjectID4, OverTimeMasterID);

                    var date3 = OverTimemodel.hdtext3;
                    var DaysofWeek3 = OverTimemodel.DaysofWeek3;
                    var value3 = OverTimemodel.text3_p4;
                    SaveOverTimeDetail(DaysofWeek3, value3, date3, OverTimemodel.ProjectID4, OverTimeMasterID);

                    var date4 = OverTimemodel.hdtext4;
                    var DaysofWeek4 = OverTimemodel.DaysofWeek4;
                    var value4 = OverTimemodel.text4_p4;
                    SaveOverTimeDetail(DaysofWeek4, value4, date4, OverTimemodel.ProjectID4, OverTimeMasterID);

                    var date5 = OverTimemodel.hdtext5;
                    var DaysofWeek5 = OverTimemodel.DaysofWeek5;
                    var value5 = OverTimemodel.text5_p4;
                    SaveOverTimeDetail(DaysofWeek5, value5, date5, OverTimemodel.ProjectID4, OverTimeMasterID);

                    var date6 = OverTimemodel.hdtext6;
                    var DaysofWeek6 = OverTimemodel.DaysofWeek6;
                    var value6 = OverTimemodel.text6_p4;
                    SaveOverTimeDetail(DaysofWeek6, value6, date6, OverTimemodel.ProjectID4, OverTimeMasterID);

                    var date7 = OverTimemodel.hdtext7;
                    var DaysofWeek7 = OverTimemodel.DaysofWeek7;
                    var value7 = OverTimemodel.text7_p4;
                    SaveOverTimeDetail(DaysofWeek7, value7, date7, OverTimemodel.ProjectID4, OverTimeMasterID);


                    #endregion
                }

                if (OverTimemodel.ProjectID5 != null && (OverTimemodel.texttotal_p5 != null && OverTimemodel.texttotal_p5 != 0))
                {
                    #region Project 5
                    var date1 = OverTimemodel.hdtext1;
                    var DaysofWeek1 = OverTimemodel.DaysofWeek1;
                    var value1 = OverTimemodel.text1_p5;
                    SaveOverTimeDetail(DaysofWeek1, value1, date1, OverTimemodel.ProjectID5, OverTimeMasterID);

                    var date2 = OverTimemodel.hdtext2;
                    var DaysofWeek2 = OverTimemodel.DaysofWeek2;
                    var value2 = OverTimemodel.text2_p5;
                    SaveOverTimeDetail(DaysofWeek2, value2, date2, OverTimemodel.ProjectID5, OverTimeMasterID);

                    var date3 = OverTimemodel.hdtext3;
                    var DaysofWeek3 = OverTimemodel.DaysofWeek3;
                    var value3 = OverTimemodel.text3_p5;
                    SaveOverTimeDetail(DaysofWeek3, value3, date3, OverTimemodel.ProjectID5, OverTimeMasterID);

                    var date4 = OverTimemodel.hdtext4;
                    var DaysofWeek4 = OverTimemodel.DaysofWeek4;
                    var value4 = OverTimemodel.text4_p5;
                    SaveOverTimeDetail(DaysofWeek4, value4, date4, OverTimemodel.ProjectID5, OverTimeMasterID);

                    var date5 = OverTimemodel.hdtext5;
                    var DaysofWeek5 = OverTimemodel.DaysofWeek5;
                    var value5 = OverTimemodel.text5_p5;
                    SaveOverTimeDetail(DaysofWeek5, value5, date5, OverTimemodel.ProjectID5, OverTimeMasterID);

                    var date6 = OverTimemodel.hdtext6;
                    var DaysofWeek6 = OverTimemodel.DaysofWeek6;
                    var value6 = OverTimemodel.text6_p5;
                    SaveOverTimeDetail(DaysofWeek6, value6, date6, OverTimemodel.ProjectID5, OverTimeMasterID);

                    var date7 = OverTimemodel.hdtext7;
                    var DaysofWeek7 = OverTimemodel.DaysofWeek7;
                    var value7 = OverTimemodel.text7_p5;
                    SaveOverTimeDetail(DaysofWeek7, value7, date7, OverTimemodel.ProjectID5, OverTimeMasterID);


                    #endregion
                }

                if (OverTimemodel.ProjectID6 != null && (OverTimemodel.texttotal_p6 != null && OverTimemodel.texttotal_p6 != 0))
                {
                    #region Project 6
                    var date1 = OverTimemodel.hdtext1;
                    var DaysofWeek1 = OverTimemodel.DaysofWeek1;
                    var value1 = OverTimemodel.text1_p6;
                    SaveOverTimeDetail(DaysofWeek1, value1, date1, OverTimemodel.ProjectID6, OverTimeMasterID);

                    var date2 = OverTimemodel.hdtext2;
                    var DaysofWeek2 = OverTimemodel.DaysofWeek2;
                    var value2 = OverTimemodel.text2_p6;
                    SaveOverTimeDetail(DaysofWeek2, value2, date2, OverTimemodel.ProjectID6, OverTimeMasterID);

                    var date3 = OverTimemodel.hdtext3;
                    var DaysofWeek3 = OverTimemodel.DaysofWeek3;
                    var value3 = OverTimemodel.text3_p6;
                    SaveOverTimeDetail(DaysofWeek3, value3, date3, OverTimemodel.ProjectID6, OverTimeMasterID);

                    var date4 = OverTimemodel.hdtext4;
                    var DaysofWeek4 = OverTimemodel.DaysofWeek4;
                    var value4 = OverTimemodel.text4_p6;
                    SaveOverTimeDetail(DaysofWeek4, value4, date4, OverTimemodel.ProjectID6, OverTimeMasterID);

                    var date5 = OverTimemodel.hdtext5;
                    var DaysofWeek5 = OverTimemodel.DaysofWeek5;
                    var value5 = OverTimemodel.text5_p6;
                    SaveOverTimeDetail(DaysofWeek5, value5, date5, OverTimemodel.ProjectID6, OverTimeMasterID);

                    var date6 = OverTimemodel.hdtext6;
                    var DaysofWeek6 = OverTimemodel.DaysofWeek6;
                    var value6 = OverTimemodel.text6_p6;
                    SaveOverTimeDetail(DaysofWeek6, value6, date6, OverTimemodel.ProjectID6, OverTimeMasterID);

                    var date7 = OverTimemodel.hdtext7;
                    var DaysofWeek7 = OverTimemodel.DaysofWeek7;
                    var value7 = OverTimemodel.text7_p6;
                    SaveOverTimeDetail(DaysofWeek7, value7, date7, OverTimemodel.ProjectID6, OverTimeMasterID);


                    #endregion
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void SaveDescription(OverTimeModel OverTimemodel, int OverTimeMasterID)
        {
            try
            {

                if (OverTimemodel.ProjectID1 != null)
                {
                    InsertDescriptionDetail(OverTimemodel.ProjectID1, OverTimeMasterID, OverTimemodel.Description_p1);
                }

                if (OverTimemodel.ProjectID2 != null)
                {
                    InsertDescriptionDetail(OverTimemodel.ProjectID2, OverTimeMasterID, OverTimemodel.Description_p2);
                }

                if (OverTimemodel.ProjectID3 != null)
                {
                    InsertDescriptionDetail(OverTimemodel.ProjectID3, OverTimeMasterID, OverTimemodel.Description_p3);
                }

                if (OverTimemodel.ProjectID4 != null)
                {
                    InsertDescriptionDetail(OverTimemodel.ProjectID4, OverTimeMasterID, OverTimemodel.Description_p4);
                }

                if (OverTimemodel.ProjectID5 != null)
                {
                    InsertDescriptionDetail(OverTimemodel.ProjectID5, OverTimeMasterID, OverTimemodel.Description_p5);
                }

                if (OverTimemodel.ProjectID6 != null)
                {
                    InsertDescriptionDetail(OverTimemodel.ProjectID6, OverTimeMasterID, OverTimemodel.Description_p6);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private OverTimeAuditTB InsertOverTimeAudit(int OverTimeMasterID, int Status)
        {
            try
            {
                OverTimeAuditTB objAuditTB = new OverTimeAuditTB();
                objAuditTB.ApprovalOverTimeLogID = 0;
                objAuditTB.OverTimeID = OverTimeMasterID;
                objAuditTB.Status = Status;
                objAuditTB.CreatedOn = DateTime.Now;
                objAuditTB.Comment = string.Empty;
                objAuditTB.ApprovalUser = _IUsers.GetAdminIDbyUserID(Convert.ToInt32(Session["UserID"]));
                objAuditTB.ProcessedDate = DateTime.Now;
                objAuditTB.UserID = Convert.ToInt32(Session["UserID"]);
                return objAuditTB;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        private void InsertDescriptionDetail(int? ProjectID, int OverTimeMasterID, string Description)
        {
            try
            {
                DescriptionTB objOverTimedetails = new DescriptionTB();
                objOverTimedetails.DescriptionID = 0;
                objOverTimedetails.ProjectID = ProjectID;
                objOverTimedetails.UserID = Convert.ToInt32(Session["UserID"]);
                objOverTimedetails.CreatedOn = DateTime.Now;
                objOverTimedetails.OverTimeMasterID = OverTimeMasterID;
                objOverTimedetails.Description = Description;
                int? OverTimeID = _IOverTime.InsertDescription(objOverTimedetails);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}