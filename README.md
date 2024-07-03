# Web Api Clean Architecture - CDM
This serivce is our - the CDM team's - live Branch Microservice. It will be used as an example of [Clean Architure](https://github.com/ardalis/CleanArchitecture). This microservice's responsiblity is to manage [things related to a Branch](https://miro.com/app/board/o9J_lBUMvaA=/?invite_link_id=344850719559).

This microservice solves all its [goals](https://github.com/ardalis/CleanArchitecture#goals). - a "... well-factored, SOLID applications using .NET Core."

It will demonstrate all SOLID principles. 
	1. [reference link 1](https://www.educative.io/blog/solid-principles-oop-c-sharp) 
	2. [reference link 2](https://dotnettutorials.net/course/solid-design-principles/)
	3. [reference link 3](https://www.c-sharpcorner.com/UploadFile/damubetha/solid-principles-in-C-Sharp/)

# Example of Clean Architecture

1. SOLID
2. N-Tiered Architecture
3. Core Components - `CDM.Branch.Common` [link](https://ablcode.visualstudio.com/Enterprise%20Customer/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Common)
	
	a. [Enums](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Common/Enums "Enums") - Shared Enums used in solution projects.
	
	b. [Extensions](https://ablcode.visualstudio.com/Enterprise%20Customer/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Common/Extensions "Extensions")- Shared Extensions used in solution projects.
	
	c. [Models](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Common/Models "Models") -
	1. Dto - [Data Transfer Objects](https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5) - Used by the Web Api to represnent Database Resources.
	2. Patches - Models used for `HTTPPatch` requests.
	
4. [SharedKernel Project](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-nuget-common "SharedKernel Project") - `CDM.Common` is our Shared Nuget package used by multiple CDM Microservices.
5. [Infrastructure Project](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Service "Infrastructure Project") - `CDM.Branch.Service` - This is our Logic layer, the layer with all our Business Logic (Insfrastructure).
6. [Web Project](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Api "Web Project") - `CDM.Branch.Api` - This is our internet facing service - the WebApi.
7. Test Projects - 

a. `CDM.Branch.Api.Tests` ([link](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/Tests/CDM.Branch.Api.Tests "link"))

b. `CDM.Branch.Service.Api.Tests` ([link](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/Tests/CDM.Branch.Service.Tests "link"))

c. `CDM.Branch.DataAccess.Tests` (link) (coming soon)

** Note, we use `CDM.Branch.Tests.Common` ([link](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/Tests/CDM.Branch.Tests.Common "link")) as our Common (shared) project. This project has globals like `TestGlobals` which are [used to filter and properly organize our Unit Tests](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/Tests/CDM.Branch.Api.Tests/BranchControllerTests.cs&version=GBdevelop&line=45&lineEnd=46&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents "used to filter and properly organize our Unit Tests").


# Notes
Web Project - `CDM.Branch.Api`

	1. `BaseController` ([link](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Api/Controllers/BaseController.cs "link")) - used to hold objects that are common to all controllers (i.e., `TelemetryClient`, `Configuration`)
	
	2. **BranchController**
a. Constructor - [Properly validating parameters using an extension in our SharedKernel](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-nuget-common/src/CDM.Common/Extensions/GenericExtensions.cs&version=GBdevelop&line=112&lineEnd=113&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents "Properly validating parameters using an extension in our SharedKernel") as defined as best practices [here](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1062 "here").

b. Lightweight Controller Methods

```
[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(string id)
		{
			await _branchService.DeleteAsync(id);
			await _serviceBusService.SendMessage(new BranchDto() { Id = id }, PayloadType.BranchDeleted.ToString());

			return Ok();
		}
```

- no try/catch logic and heavy controller methods - messy ([link](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-api-distributor/cdm-api-distributor/Controllers/DistributorsController.cs&version=GBdevelop&line=29&lineEnd=30&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents "link"))

- should use [Global Exception](https://ablcode.visualstudio.com/_git/Enterprise%20Customer?path=/cdm-clean-architecture-api/src/BranchMicroservice/CDM.Branch.Api/Middlewares/ExceptionMiddleware.cs&version=GBdevelop&line=9&lineEnd=10&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents "Global Exception") handler vs try/catch
- business logic should not be in controller

a. [link 1](https://jasonwatmore.com/post/2022/01/17/net-6-global-error-handler-tutorial-with-example "link 1")

b. [link 2](https://stackoverflow.com/questions/72061109/remove-try-catch-statements-from-asp-net-controllers "link 2")

