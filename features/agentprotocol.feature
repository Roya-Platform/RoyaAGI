Feature: Agent Protocol API Integration
  As a developer integrating with RoyaAI
  I want to interact with RoyaAI through the Agent Protocol standard
  So that I can leverage standardized API interfaces for agent operations

  Background:
    Given the RoyaAI system is running
    And the Agent Protocol API is enabled
    And I have a valid API client

  Scenario: Check agent status
    When I send a GET request to the "/agent" endpoint
    Then I should receive a 200 OK response
    And the response should contain the agent version
    And the response should include available actions
    And the response should have a model name of "RoyaAI Agent"

  Scenario: Create a new task
    When I send a POST request to "/agent/tasks" with:
      | input              | List all files in the documents directory |
      | additional_input   | {"directory": "documents"}                |
    Then I should receive a 200 OK response
    And the response should contain a task_id
    And the task status should be "created"
    And the created_at timestamp should be the current time

  Scenario: Retrieve task details
    Given I have created a task with id "task-123"
    When I send a GET request to "/agent/tasks/task-123"
    Then I should receive a 200 OK response
    And the response should contain the task details
    And the task_id should be "task-123"

  Scenario: Create a step for a task
    Given I have a task with id "task-123"
    When I send a POST request to "/agent/tasks/task-123/steps" with:
      | input              | Searching for PDF files                   |
      | additional_input   | {"file_type": "pdf"}                      |
    Then I should receive a 200 OK response
    And the response should contain a step_id
    And the step should be associated with task "task-123"
    And the step status should be "in_progress"

  Scenario: List all steps for a task
    Given I have a task with id "task-123"
    And the task has 3 steps
    When I send a GET request to "/agent/tasks/task-123/steps"
    Then I should receive a 200 OK response
    And the response should contain a list of 3 steps
    And all steps should be associated with task "task-123"

  Scenario: Update a step with results
    Given I have a task with id "task-123"
    And the task has a step with id "step-456"
    When I send a POST request to "/agent/tasks/task-123/steps/step-456" with:
      | output             | Found 5 PDF files in the documents directory |
      | additional_output  | {"files": ["doc1.pdf", "doc2.pdf"]}          |
      | status             | completed                                     |
    Then I should receive a 200 OK response
    And the step status should be "completed"
    And the updated_at timestamp should be the current time

  Scenario: Create an artifact for a task
    Given I have a task with id "task-123"
    When I send a POST request to "/agent/tasks/task-123/artifacts" with:
      | file_name      | file_list.json                                |
      | relative_path  | /results                                       |
      | data           | {"files": ["doc1.pdf", "doc2.pdf"]}           |
    Then I should receive a 200 OK response
    And the response should contain an artifact_id
    And the artifact should be associated with task "task-123"
    And the artifact should have a downloadable URL

  Scenario: Retrieve all artifacts for a task
    Given I have a task with id "task-123"
    And the task has 2 artifacts
    When I send a GET request to "/agent/tasks/task-123/artifacts"
    Then I should receive a 200 OK response
    And the response should contain a list of 2 artifacts
    And all artifacts should be associated with task "task-123"

  Scenario: Register a webhook for task events
    When I send a POST request to "/agent/webhooks" with:
      | url            | https://my-service.example.com/webhook        |
      | events         | ["task.completed", "step.created"]            |
    Then I should receive a 200 OK response
    And the response should contain a webhook_id
    And the webhook should be registered for "task.completed" and "step.created" events

  Scenario: Complete a full task execution lifecycle
    Given I have a valid API client
    When I create a new task to "List all executable files in the system directory"
    Then the task should be created successfully with status "created"
    
    When I create a step for the task with input "Scanning system directory"
    Then the step should be created with status "in_progress"
    
    When I update the step with output "Found 25 executable files" and status "completed"
    Then the step should be updated successfully
    
    When I create another step for the task with input "Filtering by file permissions"
    Then the step should be created with status "in_progress"
    
    When I update the second step with output "12 files with execute permissions" and status "completed"
    Then the step should be updated successfully
    
    When I create an artifact with the list of executable files
    Then the artifact should be created successfully
    
    When I retrieve all artifacts for the task
    Then I should see 1 artifact in the list
    
    When I retrieve all steps for the task
    Then I should see 2 completed steps in the list 