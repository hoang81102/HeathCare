USE [ElderCare]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 3/2/2025 7:55:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[accountId] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[password] [varchar](255) NOT NULL,
	[phone] [varchar](15) NULL,
	[email] [varchar](100) NULL,
	[fullname] [varchar](100) NOT NULL,
	[address] [text] NULL,
	[birthdate] [date] NULL,
	[hobby] [text] NULL,
	[accountStatus] [varchar](20) NOT NULL,
	[roleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[accountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[bookingId] [int] IDENTITY(1,1) NOT NULL,
	[accountId] [int] NOT NULL,
	[serviceId] [int] NOT NULL,
	[caregiverId] [int] NOT NULL,
	[elderId] [int] NULL,
	[bookingDateTime] [datetime] NOT NULL,
	[status] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[bookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Caregiver]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Caregiver](
	[caregiverId] [int] IDENTITY(1,1) NOT NULL,
	[accountId] [int] NOT NULL,
	[experienceYears] [int] NOT NULL,
	[specialty] [varchar](100) NULL,
	[certification] [varchar](255) NULL,
	[availability] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[caregiverId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Elder]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Elder](
	[elderId] [int] IDENTITY(1,1) NOT NULL,
	[accountId] [int] NOT NULL,
	[fullname] [varchar](100) NOT NULL,
	[phone] [varchar](15) NULL,
	[address] [text] NULL,
	[birthdate] [date] NULL,
	[hobby] [text] NULL,
	[medicalNote] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[elderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[feedbackId] [int] IDENTITY(1,1) NOT NULL,
	[bookingId] [int] NOT NULL,
	[note] [text] NULL,
	[rating] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[feedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Record]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Record](
	[recordId] [int] IDENTITY(1,1) NOT NULL,
	[elderId] [int] NOT NULL,
	[description] [text] NOT NULL,
	[bookingId] [int] NOT NULL,
	[status] [varchar](50) NOT NULL,
	[last_updated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[recordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[roleId] [int] IDENTITY(1,1) NOT NULL,
	[roleName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[roleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[serviceId] [int] IDENTITY(1,1) NOT NULL,
	[serviceName] [varchar](100) NOT NULL,
	[description] [text] NULL,
	[price] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[serviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tracking]    Script Date: 3/2/2025 7:55:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tracking](
	[trackingId] [int] IDENTITY(1,1) NOT NULL,
	[elderId] [int] NOT NULL,
	[date] [date] NOT NULL,
	[weight] [decimal](5, 2) NULL,
	[bloodPressure] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[trackingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([accountId], [username], [password], [phone], [email], [fullname], [address], [birthdate], [hobby], [accountStatus], [roleId]) VALUES (2, N'letankim2811', N'25f9e794323b453885f5181f1b624d0b', N'0865141745', N'a@gmail.com', N'Lê T?n Kim', N'Th?nh dông th?nh phu?c gi?ng ri?ng kiên giang', CAST(N'2025-03-06' AS Date), N'123', N'inactive', 1)
INSERT [dbo].[Account] ([accountId], [username], [password], [phone], [email], [fullname], [address], [birthdate], [hobby], [accountStatus], [roleId]) VALUES (3, N'tesssss', N'197375cc47bcd625746f32b3442b9ee2', NULL, N'd@fpt.edu.vn', N'Le Tan Kim', NULL, NULL, NULL, N'Active', 2)
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([roleId], [roleName]) VALUES (1, N'Customer')
INSERT [dbo].[Role] ([roleId], [roleName]) VALUES (2, N'Admin')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Service] ON 

INSERT [dbo].[Service] ([serviceId], [serviceName], [description], [price]) VALUES (1, N'Demo', N'Xin chao', CAST(123.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[Service] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Account__AB6E6164AA87405F]    Script Date: 3/2/2025 7:55:57 PM ******/
ALTER TABLE [dbo].[Account] ADD UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Account__F3DBC5724A6C1096]    Script Date: 3/2/2025 7:55:57 PM ******/
ALTER TABLE [dbo].[Account] ADD UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Record] ADD  DEFAULT (getdate()) FOR [last_updated]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD FOREIGN KEY([roleId])
REFERENCES [dbo].[Role] ([roleId])
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([accountId])
REFERENCES [dbo].[Account] ([accountId])
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([caregiverId])
REFERENCES [dbo].[Caregiver] ([caregiverId])
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([elderId])
REFERENCES [dbo].[Elder] ([elderId])
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([serviceId])
REFERENCES [dbo].[Service] ([serviceId])
GO
ALTER TABLE [dbo].[Caregiver]  WITH CHECK ADD FOREIGN KEY([accountId])
REFERENCES [dbo].[Account] ([accountId])
GO
ALTER TABLE [dbo].[Elder]  WITH CHECK ADD FOREIGN KEY([accountId])
REFERENCES [dbo].[Account] ([accountId])
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD FOREIGN KEY([bookingId])
REFERENCES [dbo].[Booking] ([bookingId])
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD FOREIGN KEY([bookingId])
REFERENCES [dbo].[Booking] ([bookingId])
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD FOREIGN KEY([elderId])
REFERENCES [dbo].[Elder] ([elderId])
GO
ALTER TABLE [dbo].[Tracking]  WITH CHECK ADD FOREIGN KEY([elderId])
REFERENCES [dbo].[Elder] ([elderId])
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD CHECK  (([accountStatus]='inactive' OR [accountStatus]='active'))
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD CHECK  (([status]='canceled' OR [status]='completed' OR [status]='pending'))
GO
ALTER TABLE [dbo].[Caregiver]  WITH CHECK ADD CHECK  (([availability]='on-call' OR [availability]='part-time' OR [availability]='full-time'))
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD CHECK  (([rating]>=(1) AND [rating]<=(5)))
GO
