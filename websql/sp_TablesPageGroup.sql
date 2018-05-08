alter PROCEDURE dbo.sp_TablesPageGroup
(
@TableNames VARCHAR(200),			 --表名，可以是多个表，但不能用别名  
@PrimaryKey VARCHAR(100),			 --主键，可以为空，但@Order为空时该值不能为空  
@Fields     VARCHAR(4000),           --要取出的字段，可以是多个表的字段，可以为空，为空表示select *  
@PageSize INT,						 --每页记录数  
@CurrentPage INT,					 --当前页，0表示第1页  
@Filter VARCHAR(4000) = '',			 --条件，可以为空，不用填 where  
@Group VARCHAR(200) = '',			 --分组依据，可以为空，不用填 group by  
@Order VARCHAR(200) = '',			 --排序，可以为空，为空默认按主键升序排列，不用填 order by ,多个字段排序中间用|隔开，不要使用,号。 
@RecordCount int OUTPUT              --总记录数,自己增加（总记录数）  
)
AS  
BEGIN  
     DECLARE @SortColumn VARCHAR(200)    --排序字段
     DECLARE @Operator CHAR(2)			
     DECLARE @SortTable VARCHAR(200)	
     DECLARE @SortName VARCHAR(200) 
	 declare @sOrder VARCHAR(200)    -- 把|分隔符用,替代
     IF @Fields = ''  
         SET @Fields = '*'  
     IF @Filter = ''  
         SET @Filter = 'Where 1=1'  
     ELSE  
         SET @Filter = 'Where ' +   @Filter  
     IF @Group <>''  
         SET @Group = 'GROUP BY ' + @Group  
  
     IF @Order <> ''  
     BEGIN  
         DECLARE @pos1 INT, @pos2 INT  
         SET @Order = REPLACE(REPLACE(@Order, ' asc', ' ASC'), ' desc', ' DESC')  
         IF CHARINDEX(' DESC', @Order) > 0  
             IF CHARINDEX(' ASC', @Order) > 0  
             BEGIN  
                 IF CHARINDEX(' DESC', @Order) < CHARINDEX(' ASC', @Order)  
                     SET @Operator = '<='  
                 ELSE  
                     SET @Operator = '>='  
             END  
             ELSE  
                 SET @Operator = '<='  
         ELSE  
             SET @Operator = '>='  
         SET @SortColumn = REPLACE(REPLACE(REPLACE(@Order, ' ASC', ''), ' DESC', ''), ' ', '')  
         SET @pos1 = CHARINDEX('|', @SortColumn)  
         IF @pos1 > 0  
             SET @SortColumn = SUBSTRING(@SortColumn, 1, @pos1-1)  
         SET @pos2 = CHARINDEX('.', @SortColumn)  
         IF @pos2 > 0  
         BEGIN  
             SET @SortTable = SUBSTRING(@SortColumn, 1, @pos2-1)  
             IF @pos1 > 0   
                 SET @SortName = SUBSTRING(@SortColumn, @pos2+1, @pos1-@pos2-1)  
             ELSE  
                 SET @SortName = SUBSTRING(@SortColumn, @pos2+1, LEN(@SortColumn)-@pos2)  
         END  
         ELSE  
         BEGIN  
             SET @SortTable = @TableNames  
             SET @SortName = @SortColumn  
         END  
     END  
     ELSE  
     BEGIN  
         SET @SortColumn = @PrimaryKey  
         SET @SortTable = @TableNames  
         SET @SortName = @SortColumn  
         SET @Order = @SortColumn  
         SET @Operator = '>='  
     END  
  
     DECLARE @type varchar(50)  
     DECLARE @prec int  
     Select @type=t.name, @prec=c.prec  
     FROM sysobjects o   
     JOIN syscolumns c on o.id=c.id  
     JOIN systypes t on c.xusertype=t.xusertype  
     Where o.name = @SortTable AND c.name = @SortName  
	 print @sorttable
	 print @sortName
     IF CHARINDEX('char', @type) > 0  
     SET @type = @type + '(' + CAST(@prec AS varchar) + ')'  
	 else
		set @type=' varchar(200) '
  
     DECLARE @TopRows INT  
	 declare @strTopRows varchar(50)   --字符串类型行数  
	 declare @strPageSize varchar(50)   --字符串类型页面大小
     SET @TopRows = @PageSize * @CurrentPage + 1 
	 set @strTopRows=cast(@topRows as varchar)
	 set @strPageSize=cast(@PageSize as varchar)
     --print @TopRows  
     --print @Operator  
	DECLARE @sql nvarchar(4000)  
	  --用单引号,替换|
	  set @sOrder=replace(@order,'|',',')
	  set @sql=N'DECLARE @SortColumnBegin ' + @type + '   SET ROWCOUNT ' + @strTopRows + '   Select @SortColumnBegin=' + @SortColumn + ' FROM   ' + @TableNames + ' ' + @Filter + ' ' + @Group + ' orDER BY ' + @sOrder + '  SET ROWCOUNT ' + @strPageSize + '   Select ' + @Fields + ' FROM   ' + @TableNames + ' ' + @Filter   + ' AND ' + @SortColumn + '' + @Operator + '@SortColumnBegin ' + @Group + ' ORDER BY ' + @sOrder + '  '

	  print @type
	  print @strTopRows
	  print @SortColumn
	  print @TableNames
	  exec(@sql)
  
	 print @sql
	 --EXEC(@sql)
	 
	--print @Filter 
	
	if @Group=''			--如果不分组
	begin
		SET @sql=N'SELECT @RecordCount=COUNT(*)'  
			+N' FROM '+@TableNames  
			+N' '+@Filter 
	end
	else
	begin
		SET @sql=N'select @RecordCount=count(*) from (SELECT COUNT(*) as b '  
			+N' FROM '+@TableNames  
			+N' '+@Filter +' ' + @Group + ' ) aa'
	end 

	--print @sql
	EXEC sp_executesql @sql,N'@RecordCount int OUTPUT',@RecordCount OUTPUT  
	

END  
  
GO 