SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE SCHEMA [Shared]
GO

CREATE TABLE [Shared].[Link](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Url] [varchar](500) NOT NULL,
	[Description] [ntext] NULL,
	[Path] [varchar](100) NULL,
	[Added] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[Loaded] [datetime] NULL,
	[Created] [datetime] NULL,
	[Updated] [datetime] NULL,
	[Opened] [datetime] NULL,
	[Time] [int] NOT NULL,
 CONSTRAINT [PK_Link] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [Shared].[Function](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Path] [varchar](200) NULL,
	[Assembly] [varchar](50) NOT NULL,
	[Class] [varchar](50) NOT NULL,
	[Description] [ntext] NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NULL,
	[Updated] [datetime] NULL,
 CONSTRAINT [PK_Function] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

