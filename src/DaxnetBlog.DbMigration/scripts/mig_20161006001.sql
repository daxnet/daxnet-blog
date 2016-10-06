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


