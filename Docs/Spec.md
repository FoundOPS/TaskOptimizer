# Requirements #

Multiple starting points on per worker basis (for routing in the middle of the day)

Valid route order
	-Tasks can have prerequisites

Generic cost system
     -time windows (per Task)
     -same side of the street

Capabilities (independent or as part of generic cost system?)

Avoid tolls, avoid non-commercial roads

Geographically distribute routing calculations
Auto scaling routing algorithm Minimum 2 servers, up it when >60% servers are busy

[Code Structure](https://github.com/FoundOPS/TaskOptimizer/blob/master/Docs/CodeStructure.md)

----------

# Redis Data Format #

Redis data will be Base64 encoded byte arrays with no separators.

## Redis Cached Distance/Time Entries ##

24 Bytes per Entry

Key:

`0x00	CoordinateA.X`

`0x04	CoordinateA.Y`

`0x08	CoordinateB.X`

`0x0C	CoordinateB.Y`

Value:

`0x10	CachedDistance`

`0x14	CachedTime (?)`