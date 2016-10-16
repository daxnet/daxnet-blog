USE [DaxnetBlogDB]
GO

UPDATE [dbo].[Accounts] SET [IsLocked] = 0, [IsAdmin] = 1 WHERE [Id] = 1;
GO

