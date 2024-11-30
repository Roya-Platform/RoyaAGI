Feature: Agent Protocol Compliance Validation
  """
  This feature ensures that RoyaAI properly implements the Agent Protocol standard specification.
  
  Key capabilities:
  1. Test conformance to Agent Protocol endpoints
  2. Validate request/response formats
  3. Verify agent status and task management
  4. Ensure proper step and artifact handling
  5. Confirm webhook functionality
  
  Compliance standard: Agent Protocol v1 (https://agentprotocol.ai)
  """

  Background:
    Given the RoyaAI platform is running with Agent Protocol support enabled
    And the following API endpoints are available
      | Endpoint                               | Method | Purpose                    |
      | /agent                                 | GET    | Agent Status               |
      | /agent/tasks                           | POST   | Create Task                |
      | /agent/tasks/{taskId}                  | GET    | Get Task                   |
      | /agent/tasks/{taskId}/steps            | GET    | List Steps                 |
      | /agent/tasks/{taskId}/steps            | POST   | Create Step                |
      | /agent/tasks/{taskId}/steps/{stepId}   | GET    | Get Step                   |
      | /agent/tasks/{taskId}/steps/{stepId}   | POST   | Update Step                |
      | /agent/tasks/{taskId}/artifacts        | GET    | List Artifacts             |
      | /agent/tasks/{taskId}/artifacts        | POST   | Create Artifact            |
      | /agent/tasks/{taskId}/artifacts/{artifactId} | GET | Get Artifact           |
      | /agent/webhooks                        | POST   | Register Webhook           |
      | /agent/webhooks/{webhookId}            | DELETE | Unregister Webhook         |

  Scenario: Verify Agent Status Endpoint Compliance
    When a client sends a GET request to "/agent"
    Then the response status code should be 200
    And the response content type should be "application/json"
    And the response should contain the following fields
      | Field           | Type   |
      | version         | string |
      | model_name      | string |
      | description     | string |
      | agent_type      | string |
    And all fields should have non-empty values

  Scenario: Verify Task Creation Endpoint Compliance
    When a client sends a POST request to "/agent/tasks" with the following JSON body
      """
      {
        "input": "List all files in C:/Users/Jeff/repo",
        "additional_input": {
          "directory": "C:/Users/Jeff/repo"
        }
      }
      """
    Then the response status code should be 200
    And the response content type should be "application/json"
    And the response should contain the following fields
      | Field           | Type    |
      | task_id         | string  |
      | input           | string  |
      | additional_input| object  |
      | status          | string  |
      | created_at      | string  |
      | updated_at      | string  |
    And the "task_id" field should be a valid UUID
    And the "status" field should be one of ["created", "in_progress", "completed", "failed"]

  Scenario: Verify Task Retrieval Endpoint Compliance
    Given a task with id "task-123" exists in the system
    When a client sends a GET request to "/agent/tasks/task-123"
    Then the response status code should be 200
    And the response content type should be "application/json"
    And the response should contain the task details with id "task-123"
    And the response should comply with the Agent Protocol schema for tasks

  Scenario: Verify Step Creation and Update Compliance
    Given a task with id "task-123" exists in the system
    When a client sends a POST request to "/agent/tasks/task-123/steps" with the following JSON body
      """
      {
        "input": "Searching for files",
        "additional_input": {
          "pattern": "*.txt"
        }
      }
      """
    Then the response status code should be 200
    And the response should contain a valid "step_id" field
    And the step should be in "in_progress" status
    
    When a client sends a POST request to "/agent/tasks/task-123/steps/{step_id}" with the following JSON body
      """
      {
        "output": "Found 5 text files",
        "additional_output": {
          "files": ["file1.txt", "file2.txt", "file3.txt", "file4.txt", "file5.txt"]
        },
        "status": "completed"
      }
      """
    Then the response status code should be 200
    And the step status should be "completed"
    And the response should comply with the Agent Protocol schema for steps

  Scenario: Verify Artifact Management Compliance
    Given a task with id "task-123" exists in the system
    When a client sends a POST request to "/agent/tasks/task-123/artifacts" with the following JSON body
      """
      {
        "file_name": "results.json",
        "relative_path": "/",
        "data": "eyJyZXN1bHRzIjogWyJmaWxlMS50eHQiLCAiZmlsZTIudHh0Il19"
      }
      """
    Then the response status code should be 200
    And the response should contain a valid "artifact_id" field
    And the artifact should have a downloadable URL
    
    When a client sends a GET request to "/agent/tasks/task-123/artifacts"
    Then the response status code should be 200
    And the response should contain a list of artifacts
    And the list should include the artifact with file name "results.json"

  Scenario: Verify Webhook Registration Compliance
    When a client sends a POST request to "/agent/webhooks" with the following JSON body
      """
      {
        "url": "https://example.com/webhook",
        "events": ["task.created", "task.completed", "step.created", "step.completed"]
      }
      """
    Then the response status code should be 200
    And the response should contain a valid "webhook_id" field
    And the webhook should be registered for the specified events

  Scenario: Complete End-to-End API Workflow Compliance
    When a client creates a new task with input "Execute dir command in C:/Users/Jeff/repo"
    And the client creates a step for the task with input "Running dir command"
    And the client updates the step with output from running the command and status "completed"
    And the client creates an artifact containing the command results
    Then all API responses should comply with the Agent Protocol schema
    And the task should be marked as "completed"
    And the task should have 1 step and 1 artifact

  Scenario: Test Error Response Compliance
    When a client sends a GET request to "/agent/tasks/nonexistent-task"
    Then the response status code should be 404
    And the response should contain an error message
    And the error response format should comply with the Agent Protocol error schema 