/*******************************************************************************************/
-- Daxnet Blog Database Initialization Script
/*******************************************************************************************/

IF OBJECT_ID('Replies', 'U') IS NOT NULL 
  DROP TABLE [Replies]; 

IF OBJECT_ID('BlogPosts', 'U') IS NOT NULL 
  DROP TABLE [BlogPosts]; 

IF OBJECT_ID('Accounts', 'U') IS NOT NULL 
  DROP TABLE [Accounts]; 

/****** Object:  Table [dbo][Accounts]    Script Date: 2016/10/6 10:55:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Accounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](16) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[NickName] [nvarchar](16) NOT NULL,
	[EmailAddress] [nvarchar](256) NOT NULL,
	[DateRegistered] [datetime] NOT NULL,
	[DateLastLogin] [datetime] NULL,
	[EmailVerifyCode] NVARCHAR(32) NULL, 
	[EmailVerifiedDate] DATETIME NULL, 
	[IsLocked] BIT NULL,
	[IsAdmin] BIT NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Accounts_EmailAddress] UNIQUE NONCLUSTERED 
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Accounts_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


/****** Object:  Table [dbo][BlogPosts]    Script Date: 2016/10/6 10:56:16 ******/
CREATE TABLE [BlogPosts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](64) NOT NULL,
	[Content] [ntext] NOT NULL,
	[DatePublished] [datetime] NOT NULL,
	[AccountId] [int] NOT NULL,
	[UpVote] [int] NULL,
	[DownVote] [int] NULL,
	[Visits] [int] NULL,
	[IsDeleted] BIT NULL,
 CONSTRAINT [PK_BlogPosts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [BlogPosts]  WITH CHECK ADD  CONSTRAINT [FK_BlogPosts_Accounts] FOREIGN KEY([AccountId])
REFERENCES [Accounts] ([Id])
GO

ALTER TABLE [BlogPosts] CHECK CONSTRAINT [FK_BlogPosts_Accounts]
GO

/****** Object:  Table [dbo].[Replies]    Script Date: 2016/10/12 12:02:25 ******/
CREATE TABLE [dbo].[Replies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BlogPostId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[DatePublished] [datetime] NOT NULL,
	[ParentId] [int] NULL,
	[Content] [ntext] NOT NULL,
	[Status] [int] NULL
 CONSTRAINT [PK_Replies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Replies]  WITH CHECK ADD  CONSTRAINT [FK_Replies_Accounts] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([Id])
GO

ALTER TABLE [dbo].[Replies] CHECK CONSTRAINT [FK_Replies_Accounts]
GO

ALTER TABLE [dbo].[Replies]  WITH CHECK ADD  CONSTRAINT [FK_Replies_BlogPosts] FOREIGN KEY([BlogPostId])
REFERENCES [dbo].[BlogPosts] ([Id])
GO

ALTER TABLE [dbo].[Replies] CHECK CONSTRAINT [FK_Replies_BlogPosts]
GO

ALTER TABLE [dbo].[Replies]  WITH CHECK ADD  CONSTRAINT [FK_Replies_Replies1] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Replies] ([Id])
GO

ALTER TABLE [dbo].[Replies] CHECK CONSTRAINT [FK_Replies_Replies1]
GO

/******************************* Initialize Data **********************************/
SET IDENTITY_INSERT [dbo].[Accounts] ON
GO

INSERT [dbo].[Accounts] ([Id], [UserName], [PasswordHash], [NickName], [EmailAddress], [DateRegistered], [DateLastLogin], [IsLocked], [IsAdmin]) VALUES (1, N'daxnet', N'sTFpu6lqfcpmWJKY0Enj4wz8H0E=', N'daxnet', N'daxnet@outlook.com', getutcdate(), NULL, 0, 1)

SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO
