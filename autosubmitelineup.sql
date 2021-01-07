--INSERT INTO LineupPlayer (PlayerId, IntervalId, TeamId, Active, Points, Position)
SELECT PlayerId, 35, TeamId, Active, Points, Position
FROM LineupPlayer
WHERE IntervalId = 34 AND TeamId = 14;


--SELECT * FROM GAME_INFO g
--WHERE g.START_TIME
SELECT i.Id, MIN(g.START_TIME) 
FROM GAME_INFO g INNER JOIN Interval i ON CONVERT(date,g.START_TIME) BETWEEN i.StartDate and i.EndDate
WHERE CONVERT(date,GETDATE()) BETWEEN i.StartDate and i.EndDate
GROUP BY i.Id;

SELECT p.IntervalId, p.TeamId, COUNT(DISTINCT p.PlayerId)
FROM LineupPlayer p INNER JOIN Interval i ON p.IntervalId = i.Id
CROSS JOIN Team t
WHERE i.Id = 36
--WHERE CONVERT(date,GETDATE()) BETWEEN i.StartDate and i.EndDate
GROUP BY p.IntervalId, p.TeamId;

SELECT i.Id, t.Id, COUNT(DISTINCT p.PlayerId)
FROM Team t CROSS JOIN Interval i INNER JOIN LineupPlayer p ON p.IntervalId = i.Id
WHERE i.Id = 36
--WHERE CONVERT(date,GETDATE()) BETWEEN i.StartDate and i.EndDate
GROUP BY i.Id, t.Id;

SELECT i.Id, t.Id, COUNT(DISTINCT p.PlayerId)
FROM Team t CROSS JOIN Interval i LEFT JOIN LineupPlayer p ON p.IntervalId = i.Id AND p.TeamId = t.Id
--WHERE i.Id = 36
WHERE CONVERT(date,GETDATE()) BETWEEN i.StartDate and i.EndDate
GROUP BY i.Id, t.Id;