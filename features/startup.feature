Feature: Here are the required actions at the startup of the application

"""
At the startup level of the platform, the following health checks should happen:
1. Firewall check
2. Memory health check
3. Guard health check
4. Health check of the kernel (could be applied by running tests against it and provide a report to the agent)
5. Node health check (including files health check)

Besides the health checks, the following information from node:
- Environment of the node
- The file directory of the node
- The default configuration of the node firewall
- Configuration of the platform firewall
- OS information like build version
- File hierarchy of the OS
"""