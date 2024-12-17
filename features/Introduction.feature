Feature: This is the introduction of the system roles and basic understanding of the system design

"""
Roles of the system:

Kernel: The layer of the system which provides direct interaction with the OS APIs; agents can call these APIs
Agent: Any LLM models which are connected to the system, main role of the AGI platform with the name of Roya
Node: The dedicated machine, which is a part of the cluster, where an agent has access to the node kernel. Here node can also point to a dedicated machine where agent can apply actions
Firewall: A layer between kernel and agent, with predefined rules
Client: The dedicated user who asks their requests from agent; node is the client machine
Platform: The whole applicable system on the node
Engineer: Who deploys the platform on the client node
Guard: The main rule where agent cannot have access to the boundaries; guard is responsible for stopping agent in case of red

Here are the terms of the platform:
Memory: Is a dedicated data store, regardless if it's clustered or not. All the memory and history of the agent will be stored in the data store. The agent is stateless at first look and all state is in the memory,
which could contain conversations, commands, and anything else

The goal is client should be able to ask agent for specific actions, which could be as following:
IO actions: Like read and write of specific files, this could be applied for other IO connections on the OS
System information: Containing both hardware and software information, all should be accessible for the agent
Shell: The kernel should provide proper library for the agent in order to connect and send requests based on the client request to apply a shell command on the node

Startup introduction:
At startup, the agent should request from the kernel to get brief information of the node hardware and software information
including:
OS build information
General understanding of the node environment
Directory tree of the OS

Firewall:
The rules of the firewall should be manageable only before startup and manually using configuration files, where the engineer can only edit that

In this platform, at kernel layer we have to develop as much as we can to connect OS APIs to the agent, this would be applied using any AI agent development kit

"""