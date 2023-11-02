CREATE TABLE [dbo].[OverTimeMaster](
	[OverTimeMasterID] [int] IDENTITY(1,1) NOT NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
	[TotalHours] [int] NULL,
	[UserID] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[Comment] [varchar](100) NULL,
	[OverTimeStatus] [int] NULL,
 CONSTRAINT [PK_OverTimeMaster] PRIMARY KEY CLUSTERED 
(
	[OverTimeMasterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[OverTimeDetails](
	[OverTimeID] [int] IDENTITY(1,1) NOT NULL,
	[DaysofWeek] [varchar](50) NULL,
	[Hours] [int] NULL,
	[Period] [date] NULL,
	[ProjectID] [int] NULL,
	[UserID] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[OverTimeMasterID] [int] NULL,
	[TotalHours] [int] NULL,
 CONSTRAINT [PK_OverTimeDetails] PRIMARY KEY CLUSTERED 
(
	[OverTimeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[OverTimeAuditTB](
	[ApprovalOverTimeLogID] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalUser] [int] NULL,
	[ProcessedDate] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[Comment] [varchar](100) NULL,
	[Status] [int] NULL,
	[OverTimeID] [int] NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_OverTimeAuditTB] PRIMARY KEY CLUSTERED 
(
	[ApprovalOverTimeLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DescriptionTB]
ADD [overTimeMasterID] [int] NULL;

/****** Object:  StoredProcedure [dbo].[GetDescriptionbyOverTimeMasterID]    Script Date: 05/19/2018 09:45:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetDescriptionbyOverTimeMasterID]      
@OverTimeMasterID int   ,  
@ProjectID int   
as      
begin      

select Description from DescriptionTB where OverTimeMasterID =@OverTimeMasterID and ProjectID =@ProjectID  
end
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Usp_GetHoursbyOverTimeMasterID]    
@OverTimeMasterID int   ,
@ProjectID int 
as    
begin    
    
SELECT     
      Hours 
  FROM [OverTimeDetails]     
  where OverTimeMasterID =@OverTimeMasterID and ProjectID =@ProjectID
  
  union all
  
  SELECT     
      SUM(Hours) 
  FROM [OverTimeDetails]     
  where OverTimeMasterID =@OverTimeMasterID and ProjectID =@ProjectID 
end
GO

/****** Object:  StoredProcedure [dbo].[Usp_GetTimeSheetsCountByUserID]    Script Date: 05/19/2018 09:45:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Usp_GetOverTimeCountByUserID]   
@UserID int   
as  
begin  
 select UserID,  
    count(case when Status = 1 then 1 else NULL end) SubmittedCount,  
    count(case when Status = 2 then 1 else NULL end) ApprovedCount,  
    count(case when Status = 3 then 1 else NULL end) RejectedCount  
from OverTimeAuditTB 
where UserID = @UserID   
group by UserID  
end
GO

/****** Object:  StoredProcedure [dbo].[Usp_GetProjectNamesbyTimeSheetMasterID]    Script Date: 05/19/2018 09:45:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[Usp_GetProjectNamesbyOverTimeMasterID]
@OverTimeMasterID int
as
begin

  SELECT 
      TM.ProjectID,
      PM.ProjectName
  FROM [OverTimeDetails] TM
  inner join ProjectMaster PM on TM.ProjectID =PM.ProjectID 
  where  TM.OverTimeMasterID =@OverTimeMasterID 
  group by TM.[ProjectID],PM.ProjectName
end
GO

/****** Object:  StoredProcedure [dbo].[Usp_GetPeriodsbyOverTimeMasterID]    Script Date: 05/19/2018 09:45:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Usp_GetPeriodsbyOverTimeMasterID]      
@OverTimeMasterID int      
as      
begin      
      
SELECT       
      CONVERT(varchar(10),T.Period) as Period    
  FROM [TimesheetDB].[dbo].[OverTimeDetails] T       
  inner join OverTimeAuditTB TA on T.OverTimeMasterID = TA.OverTimeID  
  where OverTimeMasterID =@OverTimeMasterID       
  group by Period      
end
GO
