Controller:
1 per problem
Creates Problem Distributions

Problem Distribution


--- API ---

Build a Task queue 
Task: Id, DateTime Requested, DateTime Started, DateTime Completed, string RequestBody
Everytime a server finishes a request, start the next



----- (Needs updating) Redis Cache -----

Routing calculations are done using OSRM: https://github.com/DennisOSRM/Project-OSRM

Routing info is stored in Redis in 2 parts

1. locations with latitude and longitude (l stands for location)
an index "l:sectionId:locationId" -> "l:15:123"
a Hash -> lat 3888972 long -7700888
latitude as an integer (multiplied by 5 to maintain 1m precision)
longitude as an integer (multiplied by 5 to maintain 1m precision)
2. processed routes (r stands for route)
an index "r:sectionId:locationId:locationTwoId" -> "r:15:123:124"
a Hash -> distance 2500 time 600
distance of the route in meters
time for a car to drive the route in seconds
