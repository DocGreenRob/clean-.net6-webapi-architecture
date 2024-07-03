# Web Api Clean Architecture
This serivce is an example of [Clean Architure](https://github.com/ardalis/CleanArchitecture). This microservice's responsiblity is to manage [things related to a Users & Roles](https://miro.com/app/board/o9J_lBUMvaA=/?invite_link_id=344850719559).

This microservice solves all its [goals](https://github.com/ardalis/CleanArchitecture#goals). - a "... well-factored, SOLID applications using .NET Core."

It will demonstrate all SOLID principles. 
	1. [reference link 1](https://www.educative.io/blog/solid-principles-oop-c-sharp) 
	2. [reference link 2](https://dotnettutorials.net/course/solid-design-principles/)
	3. [reference link 3](https://www.c-sharpcorner.com/UploadFile/damubetha/solid-principles-in-C-Sharp/)

# Example of Clean Architecture

1. SOLID
2. N-Tiered Architecture
3. Core Components - `CGE.CleanCode.Common` [link](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Common)
	
	a. [Enums](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Common/Enums) - Shared Enums used in solution projects.
	
	b. [Extensions](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Common/Extensions)- Shared Extensions used in solution projects.
	
	c. [Models](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Common/Models) -
	1. Dto - [Data Transfer Objects](https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5) - Used by the Web Api to represnent Database Resources.
	2. Patches - Models used for `HTTPPatch` requests.
	
4. [SharedKernel Project](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Common) - `.Common` is our Shared Nuget package used by multiple  Microservices.
5. [Infrastructure Project](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Services) - `CGE.CleanCode.Services` - This is our Logic layer, the layer with all our Business Logic (Insfrastructure).
6. [Web Project](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/tree/main/src/CGE.CleanCode.Api) - `CGE.CleanCode.Api` - This is our internet facing service - the WebApi.


# Notes
Web Project - `CGE.CleanCode.Api`

	1. `BaseController` ([link](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/blob/main/src/CGE.CleanCode.Api/Controllers/BaseController.cs)) - used to hold objects that are common to all controllers (i.e., `TelemetryClient`, `Configuration`)
	
	2.  Lightweight Controller Methods

```
[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(string id)
		{
			await _branchService.DeleteAsync(id);
			await _serviceBusService.SendMessage(new BranchDto() { Id = id }, PayloadType.BranchDeleted.ToString());

			return Ok();
		}
```

- no try/catch logic and heavy controller methods 

- should use [Global Exception](https://github.com/DocGreenRob/clean-.net6-webapi-architecture/blob/main/src/CGE.CleanCode.Api/Middlewear/ExceptionMiddleware.cs) handler vs try/catch
- business logic should not be in controller

a. [link 1](https://jasonwatmore.com/post/2022/01/17/net-6-global-error-handler-tutorial-with-example "link 1")

b. [link 2](https://stackoverflow.com/questions/72061109/remove-try-catch-statements-from-asp-net-controllers "link 2")


