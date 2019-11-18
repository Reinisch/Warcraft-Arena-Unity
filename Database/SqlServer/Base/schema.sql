/*
 Navicat Premium Data Transfer

 Source Server         : SQL Server Locale
 Source Server Type    : SQL Server
 Source Server Version : 14001000
 Source Host           : localhost\SQLEXPRESS:1433
 Source Catalog        : ellerealtime
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 14001000
 File Encoding         : 65001

 Date: 14/11/2019 16:03:09
*/


-- ----------------------------
-- Table structure for accounts
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[accounts]') AND type IN ('U'))
	DROP TABLE [dbo].[accounts]
GO

CREATE TABLE [dbo].[accounts] (
  [ID] int IDENTITY(1,1) NOT NULL,
  [Username] varchar(255) COLLATE Latin1_General_CI_AS  NOT NULL,
  [Password] varchar(500) COLLATE Latin1_General_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[accounts] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Primary Key structure for table accounts
-- ----------------------------
ALTER TABLE [dbo].[accounts] ADD CONSTRAINT [PK__accounts__3214EC274390594D] PRIMARY KEY CLUSTERED ([ID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO



CREATE TABLE [dbo].[players_info] (
  [AccountID] int  NOT NULL,
  [PosX] float(53)  NOT NULL,
  [PosY] float(53)  NOT NULL,
  [PosZ] float(53)  NOT NULL,
  [RotX] float(53)  NOT NULL,
  [RotY] float(53)  NOT NULL,
  [RotZ] float(53)  NOT NULL
)
GO

ALTER TABLE [dbo].[players_info] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Primary Key structure for table players_info
-- ----------------------------
ALTER TABLE [dbo].[players_info] ADD CONSTRAINT [PK__players___349DA586E401A15B] PRIMARY KEY CLUSTERED ([AccountID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO