SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
/****** 对象:  StoredProcedure [dbo].[ziduan]    脚本日期: 11/03/2010 15:22:41 ******/

alter PROCEDURE [dbo].[ziduan]
/*
author:sxf
function:查询某个表的字段名，字段类型，长度及字段说明，并asc排序;查询主键信息；查询自增列信息
create date:2010-1-23 
*/	 
	(
	@biaoming nvarchar(50)
	 
	)
	 
AS
	 SET NOCOUNT ON 
	 
	 
   --获取字段名，长度，缺省值，字段说明sql2000可用
   --SELECT  [name] =   a.name, [type_name] = b.name,  length = a.prec,[default]=m.text,[description] = isnull(g.[value], ' ')  FROM   syscolumns   a left   join  systypes   b   on   a.xusertype=b.xusertype left   join    sysproperties   g   on   a.id=g.id   and   a.colid=g.smallid left join syscomments m on a.cdefault=m.id   where   a.id=object_id( @biaoming)   order by [name] asc

--sql2005可用
 SELECT  [name] =   a.name, [type_name] = b.name,  length = a.prec,[default]=m.text,[description] = isnull(g.[value], ' ')  FROM   syscolumns   a left   join  systypes   b   on   a.xusertype=b.xusertype left   join   sys.extended_properties   g   on   a.id=g.major_id   and   a.colid=g.minor_id left join syscomments m on a.cdefault=m.id   where   a.id=object_id( @biaoming)   order by [name] asc
	
	--获取表的主键值
	exec sp_pkeys @biaoming

	--获取该表自增列的相关信息
	Select so.name Table_name, --表名字
	sc.name Identity_Column_name, --自增字段名字
	ident_current(so.name) curr_value, --自增字段当前值
	ident_incr(so.name) incr_value, --自增字段增长值
	ident_seed(so.name) seed_value --自增字段种子值
	from sysobjects so 
	Inner Join syscolumns sc
	on so.id = sc.id
	and columnproperty(sc.id, sc.name, 'IsIdentity') = 1
	Where upper(so.name) = upper(@biaoming)

	set nocount off
GO
