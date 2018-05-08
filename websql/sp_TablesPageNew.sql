CREATE PROCEDURE [dbo].[sp_TablesPageNew]
( 
    @tblName nvarchar(1000),            ----要显示的表或多个表的连接
    @fields nvarchar(1000)='*',        ----要显示的字段列表
    @sortfields    nvarchar(100)='',    ----如果多列排序,一定要带asc或者desc,则@singleSortType排序方法无效，反之,单列根据@singleSortType来处理
    @singleSortType    int = 1,                ----排序方法，0为升序，1为降序
    @pageSize int = 10,                ----每页显示的记录个数
    @pageIndex    int = 1,                ----要显示那一页的记录
    @strCondition    nvarchar(1000)='',    ----查询条件,不需where    
    @Counts    int =1 output           ----查询到的记录数
) 

AS
set  nocount  on
/*
	作者：cy,sl
	时间：2010－12－27
	功能：实现多表分页查询；
*/
declare @sqlTmp nvarchar(2000)
declare @sqlGetCount nvarchar(2000)
declare @frontOrder nvarchar(200)
declare @behindOrder nvarchar(200)
declare @start nvarchar(20) 
declare @end nvarchar(20)
declare @pageCount INT

if @strCondition=''
	set @strCondition=' where 1=1'
else
	set @strCondition=' where '+@strCondition
begin
    if charindex(',',@sortfields,0)>0--多列排序,则@singleSortType排序方法无效
        begin
            set @frontOrder = @sortfields--获取分页前半部分数据的排序规则
            set @behindOrder = replace(@frontOrder,'desc','desctmp')
            set @behindOrder = replace(@behindOrder,'asc','desc')
            set @behindOrder = replace(@behindOrder,'desctmp','asc')--获取分页后半部分数据的排序规则
        end
    else--单列
        begin
            set @sortfields=replace(replace(@sortfields,'desc',''),'asc','')--替换掉结尾的规则,用@singleSortType值来处理排序
            if @singleSortType=1--倒序
                begin
                    set @frontOrder = @sortfields + ' desc'
                    set @behindOrder = @sortfields + ' asc'
					set @sortfields=@sortfields+' desc'
                end
                
            else
                begin
                    set @frontOrder = @sortfields + ' asc'
                    set @behindOrder = @sortfields + ' desc'
					set @sortfields=@sortfields+' asc'
                end
    end

    --获取记录数
    
      set @sqlGetCount = 'select @Counts=count(*) from ' + @tblName + @strCondition


    ----取得查询结果总数量-----
    exec sp_executesql @sqlGetCount,N'@Counts int out ',@Counts out
    declare @tmpCounts int
    if @Counts = 0
        set @tmpCounts = 1
    else
        set @tmpCounts = @Counts

    --取得分页总数
    set @pageCount=(@tmpCounts+@pageSize-1)/@pageSize

    /**当前页大于总页数 取最后一页**/
    if @pageIndex>@pageCount
        set @pageIndex=@pageCount
    /*-----数据分页2分处理-------*/
    declare @pageIndexNew int --总数/页大小
    declare @lastcount int --总数%页大小 最后一页的数据量

    set @pageIndexNew = @tmpCounts/@pageSize
    set @lastcount = @tmpCounts%@pageSize
    if @lastcount > 0
        set @pageIndexNew = @pageIndexNew + 1
    else
        set @lastcount = @pagesize

    --取得数据的逻辑分析
		  if @pageIndexNew<2 or @pageIndex<=@pageIndexNew / 2 + @pageIndexNew % 2   --前半部分数据处理
			begin 
				--计算开始结束的行号
				set @start = @pageSize*(@pageIndex-1)+1
				set @end =     @start+@pageSize-1 
				set @sqlTmp='SELECT * FROM (select '+@fields+',ROW_NUMBER() OVER ( Order by '+@frontOrder+' ) AS RowNumber From '+@tblName+@strCondition+') T WHERE T.RowNumber BETWEEN '+@start+' AND '+@end+' order by '+@sortfields
			end
		else
			begin
			set @pageIndex = @pageIndexNew-@pageIndex+1 --后半部分数据处理
			if @lastcount=@pageSize --如果正好是整数页
				begin
					set @start = @pageSize*(@pageIndex-1)+1
					set @end =     @start+@pageSize-1
				end
			else
				begin
					set @start = @lastcount+@pageSize*(@pageIndex-2)+1
					set @end =     @start+@pageSize-1
				end
			set @sqlTmp='select top '+@end+' * FROM (select '+@fields+',ROW_NUMBER() OVER ( Order by '+@behindOrder+' ) AS RowNumber From '+@tblName+@strCondition+') T WHERE T.RowNumber BETWEEN '+@start+' AND '+@end+'  order by '+@sortfields
			end
        end
--print @sqlTmp
exec sp_executesql @sqlTmp

set nocount off
GO