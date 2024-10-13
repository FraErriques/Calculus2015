USE [PrimeData]
GO
/****** Object:  User [app_Riemann]    Script Date: 10/13/2024 8:47:04 PM ******/
CREATE USER [app_Riemann] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [appuser]    Script Date: 10/13/2024 8:47:04 PM ******/
CREATE USER [appuser] FOR LOGIN [appuser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [appuser]
GO
/****** Object:  Table [dbo].[Prime_sequence]    Script Date: 10/13/2024 8:47:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prime_sequence](
	[ordinal] [bigint] IDENTITY(1,1) NOT NULL,
	[prime] [bigint] NOT NULL,
 CONSTRAINT [PK_Prime_sequence] PRIMARY KEY CLUSTERED 
(
	[ordinal] ASC,
	[prime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prime_sequence_noIdentity]    Script Date: 10/13/2024 8:47:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prime_sequence_noIdentity](
	[ordinal] [bigint] NOT NULL,
	[prime] [bigint] NOT NULL,
 CONSTRAINT [PK_Prime_sequence_noIdentity] PRIMARY KEY CLUSTERED 
(
	[ordinal] ASC,
	[prime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[Prime_sequence_INSERT]    Script Date: 10/13/2024 8:47:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[Prime_sequence_INSERT]
	@prime bigint
as
insert into Prime_sequence(
--ordinal
prime
) values(
	@prime
)
GO
/****** Object:  StoredProcedure [dbo].[Prime_sequence_LOAD_atMaxOrdinal]    Script Date: 10/13/2024 8:47:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[Prime_sequence_LOAD_atMaxOrdinal]
as
	select prime, ordinal from Prime_sequence
	where ordinal=(select count(ordinal) from Prime_sequence )
GO
/****** Object:  StoredProcedure [dbo].[Prime_sequence_LOAD_MULTI]    Script Date: 10/13/2024 8:47:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[Prime_sequence_LOAD_MULTI]
	@start_ordinal bigint,
	@end_ordinal bigint
as
select prime, ordinal from Prime_sequence
	where 
		ordinal>=@start_ordinal
		and ordinal<=@end_ordinal
GO
/****** Object:  StoredProcedure [dbo].[Prime_sequence_LOAD_SINGLE]    Script Date: 10/13/2024 8:47:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[Prime_sequence_LOAD_SINGLE]
	@ordinal bigint
as
select prime, ordinal from Prime_sequence
	where ordinal=@ordinal
GO
