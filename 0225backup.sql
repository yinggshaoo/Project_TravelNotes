USE [master]
GO
/****** Object:  Database [Travel]    Script Date: 2024/2/25 下午 03:15:23 ******/
CREATE DATABASE [Travel]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Travel', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Travel.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Travel_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Travel_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Travel] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Travel].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Travel] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Travel] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Travel] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Travel] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Travel] SET ARITHABORT OFF 
GO
ALTER DATABASE [Travel] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Travel] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Travel] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Travel] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Travel] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Travel] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Travel] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Travel] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Travel] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Travel] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Travel] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Travel] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Travel] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Travel] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Travel] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Travel] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Travel] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Travel] SET RECOVERY FULL 
GO
ALTER DATABASE [Travel] SET  MULTI_USER 
GO
ALTER DATABASE [Travel] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Travel] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Travel] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Travel] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Travel] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Travel] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Travel', N'ON'
GO
ALTER DATABASE [Travel] SET QUERY_STORE = ON
GO
ALTER DATABASE [Travel] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Travel]
GO
/****** Object:  Table [dbo].[album]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[album](
	[AlbumId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[AlbumName] [nvarchar](10) NOT NULL,
	[CreateTime] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[AlbumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[article]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[article](
	[ArticleId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Title] [nvarchar](20) NOT NULL,
	[Subtitle] [nvarchar](20) NULL,
	[PublishTime] [datetime] NULL,
	[TravelTime] [datetime] NULL,
	[Contents] [nvarchar](2500) NULL,
	[Location] [nvarchar](100) NULL,
	[Images] [varchar](256) NULL,
	[LikeCount] [int] NULL,
	[PageView] [int] NULL,
	[ArticleState] [char](6) NULL,
PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LabelKeyword]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LabelKeyword](
	[Result] [varchar](400) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LabelManage]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LabelManage](
	[ScenicSpotName] [varchar](100) NULL,
	[Class1] [varchar](20) NULL,
	[Class2] [varchar](20) NULL,
	[Class3] [varchar](20) NULL,
	[_level] [varchar](10) NULL,
	[Keyword] [varchar](100) NULL,
	[city] [char](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[messageBoard]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[messageBoard](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[ArticleId] [int] NOT NULL,
	[UserId] [int] NULL,
	[Contents] [nvarchar](2500) NULL,
	[MessageTime] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[photo]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[photo](
	[PhotoId] [int] IDENTITY(1,1) NOT NULL,
	[PhotoTitle] [varchar](50) NOT NULL,
	[PhotoDescription] [varchar](255) NULL,
	[PhotoPath] [varchar](255) NULL,
	[UploadDate] [date] NULL,
	[AlbumId] [int] NULL,
	[Haedshot] [varchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[PhotoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[recommandBackup]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recommandBackup](
	[userId] [varchar](10) NOT NULL,
	[gender] [varchar](4) NULL,
	[age] [varchar](3) NULL,
	[likeWeather] [char](4) NULL,
	[interest1] [varchar](10) NULL,
	[interest2] [varchar](10) NULL,
	[interest3] [varchar](10) NULL,
	[likeLocation] [varchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[recommend]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recommend](
	[LabelId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Gender] [varchar](6) NULL,
	[Weather] [varchar](4) NULL,
	[Interest] [varchar](6) NULL,
	[Interest2] [varchar](6) NULL,
	[Interest3] [varchar](6) NULL,
	[Location] [varchar](6) NULL,
PRIMARY KEY CLUSTERED 
(
	[LabelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Spots]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Spots](
	[ScenicSpotID] [char](20) NOT NULL,
	[ScenicSpotName] [varchar](100) NULL,
	[Phone] [varchar](50) NULL,
	[ZipCode] [varchar](10) NULL,
	[_Address] [varchar](500) NULL,
	[TravelInfo] [varchar](500) NULL,
	[OpenTime] [varchar](500) NULL,
	[PictureUrl1] [varchar](255) NULL,
	[PictureDescription1] [varchar](255) NULL,
	[PictureUrl2] [varchar](255) NULL,
	[PictureDescription2] [varchar](255) NULL,
	[PictureUrl3] [varchar](255) NULL,
	[PictureDescription3] [varchar](255) NULL,
	[PositionLon] [varchar](11) NULL,
	[PositionLat] [varchar](11) NULL,
	[GeoHash] [varchar](15) NULL,
	[Class1] [varchar](20) NULL,
	[Class2] [varchar](20) NULL,
	[Class3] [varchar](20) NULL,
	[_level] [varchar](10) NULL,
	[WebsiteUrl] [varchar](255) NULL,
	[ParkingInfo] [varchar](255) NULL,
	[ParkingLon] [varchar](11) NULL,
	[ParkingLat] [varchar](11) NULL,
	[ParkingGeoHash] [varchar](10) NULL,
	[TicketInfo] [varchar](500) NULL,
	[Remarks] [varchar](500) NULL,
	[Keyword] [varchar](100) NULL,
	[city] [char](10) NULL,
	[_Description] [varchar](5000) NULL,
	[DescriptionDetail] [varchar](5000) NULL,
PRIMARY KEY CLUSTERED 
(
	[ScenicSpotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tags]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tags](
	[LabelId] [int] IDENTITY(1,1) NOT NULL,
	[ArticleId] [int] NULL,
	[LabelName] [varchar](10) NOT NULL,
	[LabelDescription] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[LabelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2024/2/25 下午 03:15:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](25) NOT NULL,
	[Phone] [char](10) NULL,
	[Mail] [varchar](254) NULL,
	[Gender] [varchar](6) NULL,
	[Pwd] [varchar](256) NULL,
	[Nickname] [nvarchar](32) NULL,
	[Birthday] [datetime] NULL,
	[Address] [nvarchar](100) NULL,
	[Introduction] [nvarchar](150) NULL,
	[Interest] [nvarchar](10) NULL,
	[Haedshot] [varchar](256) NULL,
	[SuperUser] [char](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[album]  WITH CHECK ADD  CONSTRAINT [fk_album_AlbumId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([UserId])
GO
ALTER TABLE [dbo].[album] CHECK CONSTRAINT [fk_album_AlbumId]
GO
ALTER TABLE [dbo].[article]  WITH CHECK ADD  CONSTRAINT [fk_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([UserId])
GO
ALTER TABLE [dbo].[article] CHECK CONSTRAINT [fk_users_UserId]
GO
ALTER TABLE [dbo].[messageBoard]  WITH CHECK ADD  CONSTRAINT [fk_messageBoard_ArticleId] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[article] ([ArticleId])
GO
ALTER TABLE [dbo].[messageBoard] CHECK CONSTRAINT [fk_messageBoard_ArticleId]
GO
ALTER TABLE [dbo].[messageBoard]  WITH CHECK ADD  CONSTRAINT [fk_messageBoard_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([UserId])
GO
ALTER TABLE [dbo].[messageBoard] CHECK CONSTRAINT [fk_messageBoard_UserId]
GO
ALTER TABLE [dbo].[photo]  WITH CHECK ADD  CONSTRAINT [fk_photo_AlbumId] FOREIGN KEY([AlbumId])
REFERENCES [dbo].[album] ([AlbumId])
GO
ALTER TABLE [dbo].[photo] CHECK CONSTRAINT [fk_photo_AlbumId]
GO
ALTER TABLE [dbo].[tags]  WITH CHECK ADD  CONSTRAINT [fk_tags_ArticleId] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[article] ([ArticleId])
GO
ALTER TABLE [dbo].[tags] CHECK CONSTRAINT [fk_tags_ArticleId]
GO
USE [master]
GO
ALTER DATABASE [Travel] SET  READ_WRITE 
GO
