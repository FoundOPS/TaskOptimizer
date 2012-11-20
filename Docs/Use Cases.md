###Terms###

**Resource** Ex. employee, vehicle

A set of skills for completing tasks

- Hard constraints 
	- date and time windows
	- resource group (can only work with other resources with that Id)
- Future enhancements
	- limited numeric capacity (ex. machine)

**Task** Ex. fix toilet

Rewards a monetary value

- Hard constraints: no value if these are not met
	- date and time windows
	- skill requirements
- Future enhancements
	- Dynamic value
		- worth more if performed by specific resources (higher rating resource group)
	- Soft constraints: penalty if not met

**Utility**

Value - cost

## Use Cases ##

**Single Company Use**

Goal: Optimize the utility of tasks & resources for the week

Parameters

- 10,000 Tasks

**Industry Use**

Goal: Optimize the industry utility of tasks & resource 

- 25,000 Resources
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