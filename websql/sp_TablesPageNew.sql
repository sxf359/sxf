CREATE PROCEDURE [dbo].[sp_TablesPageNew]
( 
    @tblName nvarchar(1000),            ----Ҫ��ʾ�ı�����������
    @fields nvarchar(1000)='*',        ----Ҫ��ʾ���ֶ��б�
    @sortfields    nvarchar(100)='',    ----�����������,һ��Ҫ��asc����desc,��@singleSortType���򷽷���Ч����֮,���и���@singleSortType������
    @singleSortType    int = 1,                ----���򷽷���0Ϊ����1Ϊ����
    @pageSize int = 10,                ----ÿҳ��ʾ�ļ�¼����
    @pageIndex    int = 1,                ----Ҫ��ʾ��һҳ�ļ�¼
    @strCondition    nvarchar(1000)='',    ----��ѯ����,����where    
    @Counts    int =1 output           ----��ѯ���ļ�¼��
) 

AS
set  nocount  on
/*
	���ߣ�cy,sl
	ʱ�䣺2010��12��27
	���ܣ�ʵ�ֶ���ҳ��ѯ��
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
    if charindex(',',@sortfields,0)>0--��������,��@singleSortType���򷽷���Ч
        begin
            set @frontOrder = @sortfields--��ȡ��ҳǰ�벿�����ݵ��������
            set @behindOrder = replace(@frontOrder,'desc','desctmp')
            set @behindOrder = replace(@behindOrder,'asc','desc')
            set @behindOrder = replace(@behindOrder,'desctmp','asc')--��ȡ��ҳ��벿�����ݵ��������
        end
    else--����
        begin
            set @sortfields=replace(replace(@sortfields,'desc',''),'asc','')--�滻����β�Ĺ���,��@singleSortTypeֵ����������
            if @singleSortType=1--����
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

    --��ȡ��¼��
    
      set @sqlGetCount = 'select @Counts=count(*) from ' + @tblName + @strCondition


    ----ȡ�ò�ѯ���������-----
    exec sp_executesql @sqlGetCount,N'@Counts int out ',@Counts out
    declare @tmpCounts int
    if @Counts = 0
        set @tmpCounts = 1
    else
        set @tmpCounts = @Counts

    --ȡ�÷�ҳ����
    set @pageCount=(@tmpCounts+@pageSize-1)/@pageSize

    /**��ǰҳ������ҳ�� ȡ���һҳ**/
    if @pageIndex>@pageCount
        set @pageIndex=@pageCount
    /*-----���ݷ�ҳ2�ִ���-------*/
    declare @pageIndexNew int --����/ҳ��С
    declare @lastcount int --����%ҳ��С ���һҳ��������

    set @pageIndexNew = @tmpCounts/@pageSize
    set @lastcount = @tmpCounts%@pageSize
    if @lastcount > 0
        set @pageIndexNew = @pageIndexNew + 1
    else
        set @lastcount = @pagesize

    --ȡ�����ݵ��߼�����
		  if @pageIndexNew<2 or @pageIndex<=@pageIndexNew / 2 + @pageIndexNew % 2   --ǰ�벿�����ݴ���
			begin 
				--���㿪ʼ�������к�
				set @start = @pageSize*(@pageIndex-1)+1
				set @end =     @start+@pageSize-1 
				set @sqlTmp='SELECT * FROM (select '+@fields+',ROW_NUMBER() OVER ( Order by '+@frontOrder+' ) AS RowNumber From '+@tblName+@strCondition+') T WHERE T.RowNumber BETWEEN '+@start+' AND '+@end+' order by '+@sortfields
			end
		else
			begin
			set @pageIndex = @pageIndexNew-@pageIndex+1 --��벿�����ݴ���
			if @lastcount=@pageSize --�������������ҳ
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