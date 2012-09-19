**An Open Source MTSP Implemenation**

This is based off [planetcolonizer](http:// "http://code.google.com/p/planetcolonizer/") and uses [OSRM](https://github.com/DennisOSRM/Project-OSRM) for the routing calculations.

The API runs through [ServiceStack](https://github.com/ServiceStack/ServiceStack) and the project is built to run on linux with [mono](https://github.com/mono/mono.git).

Distance caching is done with [redis](https://github.com/antirez/redis) and uses the [ServiceStack.Redis](https://github.com/ServiceStack/ServiceStack.Redis) client.

Licensed under the GNU AFFERO GENERAL PUBLIC LICENSE

### Running MTSP-OSRM (Linux) ###

####Download OSM [files](http://download.geofabrik.de/osm/)*####

*Merge multiple files them with [osmconvert](link to repo)

Download and compile osmconvert

> cc -x c osmconvert.c -lz -O3 -o osmconvert

Merge files

> ./osmconvert fileone.osm filetwo.osm filethree.osm -o=merged.osm

####[Run OSRM](https://github.com/DennisOSRM/Project-OSRM/wiki/Running-OSRM)####

####Setup Redis Server####

`sudo apt-get install redis-server`



