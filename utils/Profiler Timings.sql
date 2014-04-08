use MABMoney_Profiler
go

--delete from MiniProfilerClientTimings
--delete from MiniProfilerTimings
--delete from MiniProfilers

select 
	Name, 
	SUM(DurationMilliseconds) as TotalDuration 
from 
	MiniProfilerTimings
where 
	Name != 'http://localhost:64201/'
group by
	Name
order by
	TotalDuration desc
	
--select 
--	Name, 
--	DurationMilliseconds
--from 
--	MiniProfilerTimings
--where 
--	Name != 'http://localhost:64201/'
--order by
--	Name, Id