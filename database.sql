USE [globaltecnotification]
GO
/****** Object:  Table [dbo].[AndroidNotification]    Script Date: 27/11/2019 14:27:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AndroidNotification](
	[TokenId] [nvarchar](255) NOT NULL,
	[NotificationId] [uniqueidentifier] NOT NULL,
	[Success] [tinyint] NOT NULL,
	[Multicast_Id] [bigint] NULL,
	[Message_Id] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_NotificationAndroid] PRIMARY KEY CLUSTERED 
(
	[TokenId] ASC,
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AndroidToken]    Script Date: 27/11/2019 14:27:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AndroidToken](
	[Id] [nvarchar](255) NOT NULL,
	[CompanyId] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_NotificationToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[CompanyId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IOSNotification]    Script Date: 27/11/2019 14:27:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IOSNotification](
	[TokenId] [nvarchar](255) NOT NULL,
	[NotificationId] [uniqueidentifier] NOT NULL,
	[Success] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_NotificationIOS] PRIMARY KEY CLUSTERED 
(
	[TokenId] ASC,
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IOSToken]    Script Date: 27/11/2019 14:27:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IOSToken](
	[Id] [nvarchar](255) NOT NULL,
	[CompanyId] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_NotificationIOSToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[CompanyId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 27/11/2019 14:27:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](120) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[DeletedAt] [datetime] NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Notification] ADD  CONSTRAINT [DF_Notification_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AndroidNotification]  WITH CHECK ADD  CONSTRAINT [FK_NotificationAndroid_Notification] FOREIGN KEY([NotificationId])
REFERENCES [dbo].[Notification] ([Id])
GO
ALTER TABLE [dbo].[AndroidNotification] CHECK CONSTRAINT [FK_NotificationAndroid_Notification]
GO
ALTER TABLE [dbo].[IOSNotification]  WITH CHECK ADD  CONSTRAINT [FK_NotificationIOS_Notification] FOREIGN KEY([NotificationId])
REFERENCES [dbo].[Notification] ([Id])
GO
ALTER TABLE [dbo].[IOSNotification] CHECK CONSTRAINT [FK_NotificationIOS_Notification]
GO
