SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
/****** ����:  StoredProcedure [dbo].[ziduan]    �ű�����: 11/03/2010 15:22:41 ******/

alter PROCEDURE [dbo].[ziduan]
/*
author:sxf
function:��ѯĳ������ֶ������ֶ����ͣ����ȼ��ֶ�˵������asc����;��ѯ������Ϣ����ѯ��������Ϣ
create date:2010-1-23 
*/	 
	(
	@biaoming nvarchar(50)
	 
	)
	 
AS
	 SET NOCOUNT ON 
	 
	 
   --��ȡ�ֶ��������ȣ�ȱʡֵ���ֶ�˵��sql2000����
   --SELECT  [name] =   a.name, [type_name] = b.name,  length = a.prec,[default]=m.text,[description] = isnull(g.[value], ' ')  FROM   syscolumns   a left   join  systypes   b   on   a.xusertype=b.xusertype left   join    sysproperties   g   on   a.id=g.id   and   a.colid=g.smallid left join syscomments m on a.cdefault=m.id   where   a.id=object_id( @biaoming)   order by [name] asc

--sql2005����
 SELECT  [name] =   a.name, [type_name] = b.name,  length = a.prec,[default]=m.text,[description] = isnull(g.[value], ' ')  FROM   syscolumns   a left   join  systypes   b   on   a.xusertype=b.xusertype left   join   sys.extended_properties   g   on   a.id=g.major_id   and   a.colid=g.minor_id left join syscomments m on a.cdefault=m.id   where   a.id=object_id( @biaoming)   order by [name] asc
	
	--��ȡ�������ֵ
	exec sp_pkeys @biaoming

	--��ȡ�ñ������е������Ϣ
	Select so.name Table_name, --������
	sc.name Identity_Column_name, --�����ֶ�����
	ident_current(so.name) curr_value, --�����ֶε�ǰֵ
	ident_incr(so.name) incr_value, --�����ֶ�����ֵ
	ident_seed(so.name) seed_value --�����ֶ�����ֵ
	from sysobjects so 
	Inner Join syscolumns sc
	on so.id = sc.id
	and columnproperty(sc.id, sc.name, 'IsIdentity') = 1
	Where upper(so.name) = upper(@biaoming)

	set nocount off
GO
