###Terms###

**Resource Group**: A group of resources available to complete tasks during a specified period of time

Ex. a route with 3 employees and 3 vehicles

Properties

- Scheduled time
	- 1/1/2012 3 pm to 1/1/2012 5 pm
- Overtime (if not defined: hard constraint)
	- 125%
- Skills/Cost
	- leaf collection, $10/hour
	- landscaping, $12/hour
	- tree removal, $15/hour
- Capacities
	- volume, 3000 (gallons)
	- weight, 1000 (lbs)

**Task** Ex. fix toilet

Properties

- Price: The monetary reward for completing a task
	- $200
- Skill Constraint
	- Leaf collection, hard
- Window Constraints
	- 3 - 5 pm, 125% penalty (soft)
- (FUTURE) Linked task - must be consecutively done

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

	- Single company: optimize for ideal amount (until efficiency gains <= computing cost)
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