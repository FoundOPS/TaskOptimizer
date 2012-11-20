###Terms###

**Resource Group**: A group of resources available to complete tasks during a specified period of time

Ex. a route with 3 employees and 3 vehicles

Properties
- Scheduled time hard or soft constraint ?????????
	- 1/1/2012 3 pm to 1/1/2012 5 pm 
- Skills
	- leaf collection, 10/10 efficiency
	- landscaping, 3/10 efficiency
	- tree removal, 5/10 efficiency
- Capacities
	- volume, 3000 (gallons)
	- weight, 1000 (lbs)
- Cost ????????? overtime

**Task Type** Ex. leaf collection, Id = a

**Skill** The ability to complete a task type

Properties

- TaskType: The Id of the TaskType this skill can complete. 
	- a (leaf collection)
- Efficiency: A multiplier of how efficient this is (reducing cost)
	- 3/10
- ?? is efficiency related to cost, maybe the efficiency is merged w cost

**Capacity**

Properties

- Id: The type of capacity constraint
	- a (volume)
- Amount: The numeric capacity amount
	- 500 (cubic feet)

**Task** Ex. fix toilet

Properties

- Price: The monetary reward for completing a task
	- $200
- Skill Constraint
	- Leaf collection, hard
- Time Constraint
	- 3 - 5 pm, soft, 1.3 penalty ????

Questions: Dynamic value
		- worth more if performed by specific resources (higher rating resource group)

**Skill Constraint : IConstraint**

**Time Constraint : IConstraint**

**IConstraint**

Hard: cannot charge price for a task if not met.
Soft: price penalty if not met.

Properties

- TaskTypeId: The skill required type 
- Multiplier: 0
	- 0, it's a hard constraint
	- .3, multiplier to cost

**Utility**

??Value - cost

## Use Cases ##

**Single Company Use**

Goal: Optimize the utility of tasks & resources for the week

Parameters

- 10,000 Tasks

**Industry Use**

Goal: Optimize the industry utility of tasks & resource 

- 25,000 Resource Groups
	- 3 available time windows each
	- belonging to 10,000 different resource group ids
- 65,000 Tasks

Continually add tasks to the system
Remove a resource (24) hours before it needs to be used

## Design Concepts ##

**When to stop the algorithm**

Stop entire request

	- Single company: optimize for 5 minutes

Never stop it. Remove tasks a threshold before they need to be performed (and remove assigned availability on resources).

	- Single company: optimize until 12 hours before resources are used
	- Industry: lock tasks 24 hours before their service

**Meta heuristics**

- Intersection of resources' (by group) and tasks'
	- availability
	- skills
- Location of tasks and resources

**Scalability**

Partition by creating optimization requests for tasks and resources that do not have overlapping skills