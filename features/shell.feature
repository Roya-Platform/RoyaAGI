Feature: Agent Shell Command Execution
  """
  This feature enables agents to execute shell commands on the operating system through kernel requests.

  Key capabilities:
  1. Execute basic OS commands with arguments
  2. Stream command output for long-running processes
  3. Support for environment variable configuration
  4. Working directory specification
  5. Command timeout management
  6. Error handling and reporting

  Supported shells: Command Prompt (Windows), PowerShell, Bash (Linux/macOS)
  """

  Background:
    Given the following platform firewall rules are configured
      | RuleName       | Status  |
      | Shell Access   | Enable  |
      | Network Access | Disable |
    And a platform host node exists with the following configuration
      | Key              | Value        |
      | IP               | 10.1.10.101  |
      | OS               | windows      |
      | OS Build Version | <VERSION>    |
    And the following directory aliases are configured
      | Directory          | Alias     |
      | C:/Users/Jeff/repo | My Repo   |
      | C:/Program Files   | Programs  |
    And the following agent request categories are available
      | CategoryName |
      | IO          |
      | Shell       |
    And the Shell category supports the following request types
      | TypeName    |
      | Execute     |
      | Stream      |
      | Terminate   |

  Scenario: List directory contents with 'dir' command
    When the agent sends the following request to the kernel
      | Key             | Value                |
      | RequestCategory | Shell               |
      | RequestType     | Execute             |
      | Command         | dir                 |
      | WorkingDirectory| C:/Users/Jeff/repo  |
    Then the kernel should execute the command successfully
    And the command output should contain "Project1"
    And the command output should contain "Project2"
    And the command exit code should be 0

  Scenario: Check system information with 'systeminfo' command
    When the agent sends the following request to the kernel
      | Key             | Value      |
      | RequestCategory | Shell      |
      | RequestType     | Execute    |
      | Command         | systeminfo |
      | Timeout         | 30         |
    Then the kernel should execute the command successfully
    And the command output should contain "OS Name"
    And the command output should contain "System Manufacturer"
    And the command exit code should be 0

  Scenario: Find files with 'find' command
    When the agent sends the following request to the kernel
      | Key             | Value                                   |
      | RequestCategory | Shell                                  |
      | RequestType     | Execute                                |
      | Command         | findstr /s /i "public class" *.cs      |
      | WorkingDirectory| C:/Users/Jeff/repo/Project1/src        |
    Then the kernel should execute the command successfully
    And the command output should contain "main.cs"
    And the command exit code should be 0

  Scenario: Check network status with 'ipconfig' command
    When the agent sends the following request to the kernel
      | Key             | Value     |
      | RequestCategory | Shell     |
      | RequestType     | Execute   |
      | Command         | ipconfig  |
    Then the kernel should execute the command successfully
    And the command output should contain "IPv4 Address"
    And the command exit code should be 0

  Scenario: Create a new directory with 'mkdir' command
    When the agent sends the following request to the kernel
      | Key             | Value                                |
      | RequestCategory | Shell                               |
      | RequestType     | Execute                             |
      | Command         | mkdir Project4                      |
      | WorkingDirectory| C:/Users/Jeff/repo                  |
    Then the kernel should execute the command successfully
    And the command exit code should be 0
    And the directory "C:/Users/Jeff/repo/Project4" should exist

  Scenario: Execute PowerShell script to get process information
    When the agent sends the following request to the kernel
      | Key             | Value                          |
      | RequestCategory | Shell                         |
      | RequestType     | Execute                       |
      | Command         | powershell -Command "Get-Process | Sort-Object -Property CPU -Descending | Select-Object -First 5" |
    Then the kernel should execute the command successfully
    And the command output should contain "CPU"
    And the command output should contain "NPM(K)"
    And the command exit code should be 0

  Scenario: Check disk usage with 'wmic' command
    When the agent sends the following request to the kernel
      | Key             | Value                              |
      | RequestCategory | Shell                             |
      | RequestType     | Execute                           |
      | Command         | wmic logicaldisk get deviceid, freespace, size |
    Then the kernel should execute the command successfully
    And the command output should contain "DeviceID"
    And the command output should contain "FreeSpace"
    And the command exit code should be 0

  Scenario: Stream long-running command output
    When the agent sends the following request to the kernel
      | Key             | Value                             |
      | RequestCategory | Shell                            |
      | RequestType     | Stream                           |
      | Command         | ping 127.0.0.1 -t                |
      | StreamId        | stream-ping-test                 |
    Then the kernel should start streaming the command output
    And the first stream output should be received within 2 seconds
    And the stream output should contain "Reply from 127.0.0.1"
    
    When the agent sends the following request to the kernel
      | Key             | Value                |
      | RequestCategory | Shell               |
      | RequestType     | Terminate           |
      | StreamId        | stream-ping-test    |
    Then the kernel should confirm the stream was terminated

  Scenario: Run command with environment variables
    When the agent sends the following request to the kernel
      | Key             | Value                               |
      | RequestCategory | Shell                              |
      | RequestType     | Execute                            |
      | Command         | echo %CUSTOM_VAR%                  |
      | Environment     | {"CUSTOM_VAR": "Hello from agent"} |
    Then the kernel should execute the command successfully
    And the command output should contain "Hello from agent"
    And the command exit code should be 0

  Scenario: Execute command with error handling
    When the agent sends the following request to the kernel
      | Key             | Value                    |
      | RequestCategory | Shell                   |
      | RequestType     | Execute                 |
      | Command         | nonexistent-command     |
    Then the kernel should return an error with message "Command not found: nonexistent-command"
    And the command exit code should not be 0

  Scenario: Execute multiple commands with concatenation
    When the agent sends the following request to the kernel
      | Key             | Value                                        |
      | RequestCategory | Shell                                       |
      | RequestType     | Execute                                     |
      | Command         | cd C:/Users/Jeff/repo && dir && echo Done!  |
    Then the kernel should execute the command successfully
    And the command output should contain "Project1"
    And the command output should contain "Done!"
    And the command exit code should be 0

  Scenario: Run command with elevated privileges
    When the agent sends the following request to the kernel
      | Key             | Value                          |
      | RequestCategory | Shell                         |
      | RequestType     | Execute                       |
      | Command         | net session                   |
      | RunAsAdmin      | true                          |
    Then the kernel should check agent privileges
    And if agent has admin rights, the command should execute successfully
    And if agent doesn't have admin rights, the kernel should return an error with message "Insufficient privileges"

  Scenario: Execute command with timeout constraints
    When the agent sends the following request to the kernel
      | Key             | Value                                    |
      | RequestCategory | Shell                                   |
      | RequestType     | Execute                                 |
      | Command         | ping 127.0.0.1 -n 10                    |
      | Timeout         | 2                                       |
    Then the kernel should return an error with message "Command execution timed out after 2 seconds"

  Scenario: Execute shell script from file
    Given the file "C:/Users/Jeff/repo/scripts/hello.bat" contains the following content
      """
      @echo off
      echo Hello, World!
      echo Current directory: %CD%
      echo Current time: %TIME%
      """
    When the agent sends the following request to the kernel
      | Key             | Value                             |
      | RequestCategory | Shell                            |
      | RequestType     | Execute                          |
      | Command         | C:/Users/Jeff/repo/scripts/hello.bat |
    Then the kernel should execute the command successfully
    And the command output should contain "Hello, World!"
    And the command output should contain "Current directory"
    And the command exit code should be 0
