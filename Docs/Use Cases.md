###Terms###

1) **Resource Group**

A group of resources available to complete tasks.

Ex. a route with 3 employees and 3 vehicles

Properties

- StartPoint
	- 1305 Cumberland Ave, 47906
- Scheduled
	- 1/1/2012 9 am to 1/1/2012 5 pm
- Task Capabilities / Time Efficiency
	- leaf collection, 10/10
	- landscaping, 3/10
	- tree removal, 7/10
- Wage
	- $50 / hour

FUTURE

- ?? Capacities (if any filled must return to location before continuing skill)
	- volume, 3000 (gallons), 12414 English Garden Court, 47906 or .....
	- weight, 1000 (lbs)
- Overtime (if not defined: hard constraint)
	- 125% (first hour, employee specific, etc)

- Maximum distance before refueled

2) **Task** Ex. fix toilet

Properties

- Type
	- Leaf collection
- Price: The monetary reward for completing a task
	- $200
- Time
	- 30 minutes
- Window Constraints - Penalty (1-100%)
	- 3 - 5 pm, 1/1/2012 - 1/2/2012, 100%
	- 1 - 4 pm, 1/3/2012 - 1/5/2012, 25%
- Optional Target Date & Last Date
	Ensure jobs happen on a user defined recurrence. By penalizing exponentially weight of distance from target date


FUTURE
- Capacity
	- 100 lbs
- Linked task - must be done consecutively or serially & or in the same group
- Dynamic price in the future (script api for internal variables: distance previous location)

Questions: Dynamic value
		- worth more if performed by specific resources (higher rating resource group)

**Constraints**

Hard constraints cannot charge price for a task if not met.

Soft constraints charge a penalty cost (if resource group) or price (if task).

**Utility**

Value - Cost

## Use Cases ##

**Single Company Use**

Goal: Optimize the utility of tasks & resources for the week

Parameters

- 10,000 Tasks
- 50 Resource Groups


## Design Concepts ##

**When to start the algorithm**


**When to stop the algorithm**

Stop entire request

	- Single company: optimize for ideal amount (until efficiency gains <= computing cost)
	- Single company: optimize for 5 minutes

**Meta heuristics**

- Intersection of resources' (by group) and tasks'
	- availability
	- skills
- Location of tasks and resources

**Scalability**

Partition by creating optimization requests for tasks and resources that do not have overlapping skills

Use distance oracle when cache > 1/2 of memory (8 gb worth of cache ~ 5,000 locations); if everything is in memory. Could store some and prioritize.


**(FUTURE: LIVE DISPATCHING/SEPERATE ALGORITHM)**

Adding tasks on demand

Goal: Optimize the industry utility of tasks & resources

- 25,000 Resource Groups
	- 3 available time windows each
	- belonging to 10,000 different resource group ids
- 65,000 Tasks

Continually add tasks to the system

Remove a resource (24) hours before it needs to be used

Never stop it. Remove tasks a threshold before they need to be performed (and remove assigned availability on resources).

	- Single company: optimize until 12 hours before resources are used
	- Industry: lock tasks 24 hours before their service