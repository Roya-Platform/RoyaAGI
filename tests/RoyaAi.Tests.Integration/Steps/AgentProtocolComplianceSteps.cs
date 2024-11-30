using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RoyaAi.Tests.Integration.Steps
{
    [Binding]
    [Category("AgentProtocolCompliance")]
    public class AgentProtocolComplianceSteps
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _response;
        private readonly Dictionary<string, string> _createdResources = new();
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AgentProtocolComplianceSteps()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        [Given(@"the RoyaAI platform is running with Agent Protocol support enabled")]
        public async Task GivenTheRoyaAIPlatformIsRunningWithAgentProtocolSupportEnabled()
        {
            var response = await _client.GetAsync("/agent");
            response.IsSuccessStatusCode.Should().BeTrue("API should be available");
        }

        [Given(@"the following API endpoints are available")]
        public void GivenTheFollowingAPIEndpointsAreAvailable(Table table)
        {
            // Endpoints are verified in subsequent steps
        }

        [Given(@"a task with id ""(.*)"" exists in the system")]
        public async Task GivenATaskWithIdExistsInTheSystem(string taskId)
        {
            // Create a task if it doesn't exist
            var content = new
            {
                input = "Test task for compliance verification",
                additional_input = new { created_for = "compliance-testing" }
            };

            var response = await _client.PostAsJsonAsync("/agent/tasks", content);
            response.IsSuccessStatusCode.Should().BeTrue();

            var responseJson = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            _createdResources["task-123"] = responseJson["task_id"].ToString();
        }

        [When(@"a client sends a GET request to ""(.*)""")]
        public async Task WhenAClientSendsAGETRequestTo(string endpoint)
        {
            endpoint = ReplaceResourceIds(endpoint);
            _response = await _client.GetAsync(endpoint);
        }

        [When(@"a client sends a POST request to ""(.*)"" with the following JSON body")]
        public async Task WhenAClientSendsPOSTRequestToWithTheFollowingJSONBody(string endpoint, string jsonBody)
        {
            endpoint = ReplaceResourceIds(endpoint);
            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(endpoint, content);
        }

        [When(@"a client creates a new task with input ""(.*)""")]
        public async Task WhenAClientCreatesANewTaskWithInput(string input)
        {
            var content = new
            {
                input,
                additional_input = new { }
            };

            _response = await _client.PostAsJsonAsync("/agent/tasks", content);
            _response.IsSuccessStatusCode.Should().BeTrue();

            var responseJson = await _response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            _createdResources["end-to-end-task"] = responseJson["task_id"].ToString();
        }

        [When(@"the client creates a step for the task with input ""(.*)""")]
        public async Task WhenTheClientCreatesAStepForTheTaskWithInput(string input)
        {
            var content = new
            {
                input,
                additional_input = new { }
            };

            var taskId = _createdResources["end-to-end-task"];
            _response = await _client.PostAsJsonAsync($"/agent/tasks/{taskId}/steps", content);
            _response.IsSuccessStatusCode.Should().BeTrue();

            var responseJson = await _response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            _createdResources["end-to-end-step"] = responseJson["step_id"].ToString();
        }

        [When(@"the client updates the step with output from running the command and status ""(.*)""")]
        public async Task WhenTheClientUpdatesTheStepWithOutputFromRunningTheCommandAndStatus(string status)
        {
            var content = new
            {
                output = "Directory listing completed successfully",
                additional_output = new { files = new[] { "file1.txt", "file2.txt" } },
                status
            };

            var taskId = _createdResources["end-to-end-task"];
            var stepId = _createdResources["end-to-end-step"];
            _response = await _client.PostAsJsonAsync($"/agent/tasks/{taskId}/steps/{stepId}", content);
            _response.IsSuccessStatusCode.Should().BeTrue();
        }

        [When(@"the client creates an artifact containing the command results")]
        public async Task WhenTheClientCreatesAnArtifactContainingTheCommandResults()
        {
            var content = new
            {
                file_name = "results.json",
                relative_path = "/",
                data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("{\"files\":[\"file1.txt\",\"file2.txt\"]}"))
            };

            var taskId = _createdResources["end-to-end-task"];
            _response = await _client.PostAsJsonAsync($"/agent/tasks/{taskId}/artifacts", content);
            _response.IsSuccessStatusCode.Should().BeTrue();

            var responseJson = await _response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            _createdResources["end-to-end-artifact"] = responseJson["artifact_id"].ToString();
        }

        [Then(@"the response status code should be (.
    }
} 