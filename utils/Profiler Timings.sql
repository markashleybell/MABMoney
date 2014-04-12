use MABMoney_Profiler
go

--delete from MiniProfilerClientTimings
--delete from MiniProfilerTimings
--delete from MiniProfilers

declare @filter nvarchar(50)

set @filter = 'http://localhost%'
set @filter = 'X'

select 
	Name, 
	SUM(DurationMilliseconds) as TotalDuration 
from 
	MiniProfilerTimings
where 
	Name not like @filter
group by
	Name
order by
	TotalDuration desc
	
	
select 
	Name, 
	SUM(DurationMilliseconds) / Count(*) as AverageDuration
from 
	MiniProfilerTimings
where 
	Name not like @filter
group by
	Name
order by
	AverageDuration desc
	
	
--select 
--	Name, 
--	DurationMilliseconds
--from 
--	MiniProfilerTimings
--where 
--	Name != 'http://localhost%'
--order by
--	Name, Id