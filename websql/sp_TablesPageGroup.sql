alter PROCEDURE dbo.sp_TablesPageGroup
(
@TableNames VARCHAR(200),			 --�����������Ƕ�����������ñ���  
@PrimaryKey VARCHAR(100),			 --����������Ϊ�գ���@OrderΪ��ʱ��ֵ����Ϊ��  
@Fields     VARCHAR(4000),           --Ҫȡ�����ֶΣ������Ƕ������ֶΣ�����Ϊ�գ�Ϊ�ձ�ʾselect *  
@PageSize INT,						 --ÿҳ��¼��  
@CurrentPage INT,					 --��ǰҳ��0��ʾ��1ҳ  
@Filter VARCHAR(4000) = '',			 --����������Ϊ�գ������� where  
@Group VARCHAR(200) = '',			 --�������ݣ�����Ϊ�գ������� group by  
@Order VARCHAR(200) = '',			 --���򣬿���Ϊ�գ�Ϊ��Ĭ�ϰ������������У������� order by ,����ֶ������м���|��������Ҫʹ��,�š� 
@RecordCount int OUTPUT              --�ܼ�¼��,�Լ����ӣ��ܼ�¼����  
)
AS  
BEGIN  
     DECLARE @SortColumn VARCHAR(200)    --�����ֶ�
     DECLARE @Operator CHAR(2)			
     DECLARE @SortTable VARCHAR(200)	
     DECLARE @SortName VARCHAR(200) 
	 declare @sOrder VARCHAR(200)    -- ��|�ָ�����,���
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
	 declare @strTopRows varchar(50)   --�ַ�����������  
	 declare @strPageSize varchar(50)   --�ַ�������ҳ���С
     SET @TopRows = @PageSize * @CurrentPage + 1 
	 set @strTopRows=cast(@topRows as varchar)
	 set @strPageSize=cast(@PageSize as varchar)
     --print @TopRows  
     --print @Operator  
	DECLARE @sql nvarchar(4000)  
	  --�õ�����,�滻|
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
	
	if @Group=''			--���������
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