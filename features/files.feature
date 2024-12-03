Feature: Agent CRUD Operations for OS Files
  """
  This feature enables agents to perform CRUD operations on readable files through kernel requests.

  Key capabilities:
  1. Multi-line file updates at specific locations
  2. Directory hierarchy traversal and reading
  3. File alias management for simplified access

  Supported file types: Human-readable files (text, json, cs, etc.)
  """

  Background:
    Given the following platform firewall rules are configured
      | RuleName   | Status  |
      | IO Access  | Disable |
    And a platform host node exists with the following configuration
      | Key              | Value        |
      | IP               | 10.1.10.101  |
      | OS               | windows      |
      | OS Build Version | <VERSION>    |
    And the following directory aliases are configured
      | Directory          | Alias    |
      | C:/Users/Jeff/repo | My Repo  |
    And the following agent request categories are available
      | CategoryName |
      | IO          |
      | Shell       |
    And the IO category supports the following request types
      | TypeName |
      | Tree     |
      | Read     |
      | Update   |
      | Create   |
      | Delete   |

  Scenario: Retrieve directory hierarchy tree
    When the agent sends the following request to the kernel
      | Key             | Value              |
      | Directory       | C:/Users/Jeff/repo |
      | RequestCategory | IO                 |
      | RequestType     | Tree               |
    Then the kernel should return the following directory structure
      """
      C:/Users/Jeff/repo/
      ├── Project1/
      │   ├── src/
      │   │   ├── main.cs
      │   │   └── helper.cs
      │   ├── tests/
      │   │   └── test.cs
      │   └── README.md
      ├── Project2/
      │   ├── docs/
      │   │   └── api.md
      │   └── config.json
      └── notes.txt
      """

  Scenario: Read file content using physical path
    When the agent sends the following request to the kernel
      | Key             | Value                               |
      | FilePath        | C:/Users/Jeff/repo/Project1/README.md |
      | RequestCategory | IO                                  |
      | RequestType     | Read                                |
    Then the kernel should return the following content
      """
      # Project1
      
      This is a sample project demonstrating the agent file operations.
      
      ## Features
      - Feature 1
      - Feature 2
      
      ## Getting Started
      See the documentation for more details.
      """

  Scenario: Read file content using directory alias
    When the agent sends the following request to the kernel
      | Key             | Value                        |
      | FilePath        | My Repo/Project2/config.json |
      | RequestCategory | IO                           |
      | RequestType     | Read                         |
    Then the kernel should return the following content
      """
      {
        "name": "Project2",
        "version": "1.0.0",
        "description": "Sample configuration",
        "settings": {
          "timeout": 30,
          "maxRetries": 3,
          "debug": false
        }
      }
      """

  Scenario: Update file content at specific location
    When the agent sends the following request to the kernel
      | Key             | Value                                |
      | FilePath        | C:/Users/Jeff/repo/Project1/src/main.cs |
      | RequestCategory | IO                                   |
      | RequestType     | Update                               |
      | LineNumber      | 5                                    |
      | Content         | """
                           // Adding a new method
                           public void NewFeature() {
                               Console.WriteLine("New feature added!");
                           }
                           """ |
    Then the kernel should confirm the update was successful
    And the file "C:/Users/Jeff/repo/Project1/src/main.cs" should contain the following content at line 5
      """
        // Adding a new method
        public void NewFeature() {
            Console.WriteLine("New feature added!");
        }
      """

  Scenario: Create a new file
    When the agent sends the following request to the kernel
      | Key             | Value                                    |
      | FilePath        | My Repo/Project1/docs/implementation.md |
      | RequestCategory | IO                                       |
      | RequestType     | Create                                   |
      | Content         | """
                          # Implementation Details

                          This document outlines the implementation specifics for Project1.

                          ## Architecture

                          The system follows a modular architecture with the following components:

                          - Component A
                          - Component B
                          - Component C
                          """ |
    Then the kernel should confirm the file was created successfully
    And the file "C:/Users/Jeff/repo/Project1/docs/implementation.md" should exist
    And the directory "C:/Users/Jeff/repo/Project1/docs" should exist

  Scenario: Delete a file
    When the agent sends the following request to the kernel
      | Key             | Value                         |
      | FilePath        | C:/Users/Jeff/repo/notes.txt |
      | RequestCategory | IO                            |
      | RequestType     | Delete                        |
    Then the kernel should confirm the file was deleted successfully
    And the file "C:/Users/Jeff/repo/notes.txt" should not exist

  Scenario: Update a file with complex multi-line changes
    Given the file "C:/Users/Jeff/repo/Project2/docs/api.md" contains the following content
      """
      # API Documentation
      
      ## Endpoints
      
      ### GET /api/v1/users
      Returns a list of users.
      
      ### POST /api/v1/users
      Creates a new user.
      """
    When the agent sends the following request to the kernel
      | Key             | Value                         |
      | FilePath        | My Repo/Project2/docs/api.md |
      | RequestCategory | IO                            |
      | RequestType     | Update                        |
      | LineNumber      | 7                             |
      | Content         | """
                          ### PUT /api/v1/users/{id}
                          Updates an existing user.

                          ### DELETE /api/v1/users/{id}
                          Deletes a user.
                          """ |
    Then the kernel should confirm the update was successful
    And the file "C:/Users/Jeff/repo/Project2/docs/api.md" should contain the following content
      """
      # API Documentation
      
      ## Endpoints
      
      ### GET /api/v1/users
      Returns a list of users.
      
      ### PUT /api/v1/users/{id}
      Updates an existing user.
      
      ### DELETE /api/v1/users/{id}
      Deletes a user.
      
      ### POST /api/v1/users
      Creates a new user.
      """

  Scenario: Handle error when file does not exist
    When the agent sends the following request to the kernel
      | Key             | Value                               |
      | FilePath        | C:/Users/Jeff/repo/nonexistent.txt |
      | RequestCategory | IO                                  |
      | RequestType     | Read                                |
    Then the kernel should return an error with message "File not found: C:/Users/Jeff/repo/nonexistent.txt"

  Scenario: Handle error when directory alias is invalid
    When the agent sends the following request to the kernel
      | Key             | Value                    |
      | FilePath        | Invalid Alias/config.json |
      | RequestCategory | IO                       |
      | RequestType     | Read                     |
    Then the kernel should return an error with message "Directory alias 'Invalid Alias' not found"

  Scenario: Navigate through deep directory structure
    When the agent sends the following request to the kernel
      | Key             | Value                    |
      | Directory       | C:/Users/Jeff/repo/Project1 |
      | RequestCategory | IO                       |
      | RequestType     | Tree                     |
      | Depth           | 2                        |
    Then the kernel should return the following directory structure
      """
      C:/Users/Jeff/repo/Project1/
      ├── src/
      │   ├── main.cs
      │   └── helper.cs
      ├── tests/
      │   └── test.cs
      ├── docs/
      │   └── implementation.md
      └── README.md
      """

  Scenario: Create a new file in a directory that does not exist
    When the agent sends the following request to the kernel
      | Key              | Value                                       |
      | FilePath         | My Repo/Project3/src/main.js               |
      | RequestCategory  | IO                                          |
      | RequestType      | Create                                      |
      | Content          | """
                           // Project3 main entry point

                           function initialize() {
                             console.log('Project3 initialized');
                           }

                           initialize();
                           """ |
      | CreateDirectories | true                                        |
    Then the kernel should confirm the file was created successfully
    And the file "C:/Users/Jeff/repo/Project3/src/main.js" should exist
    And the directory "C:/Users/Jeff/repo/Project3/src" should exist
    And the directory "C:/Users/Jeff/repo/Project3" should exist