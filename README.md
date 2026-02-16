# Basic Chat Loop Example

The `BasicChatLoop.cs` file demonstrates a foundational implementation of an interactive chat application using the Microsoft Agents AI Framework with OpenAI's GPT models. This example showcases how to create a stateful conversation loop where users can input messages through the console, and the AI agent responds in real-time using streaming responses. The implementation leverages the Agent Framework's `ChatClientAgent` to maintain session state across multiple turns of conversation, providing a seamless chat experience.

# Tools Example

The `Tools.cs` file illustrates advanced agent capabilities by combining internal tools with external MCP (Model Context Protocol) tools in a single agent. This example demonstrates how to create custom internal tools using `AIFunctionFactory`, integrate dependency injection into tool methods with different service lifetimes (singleton, scoped, and transient), and seamlessly merge tools from an MCP server with locally defined tools. The implementation shows a practical approach to building extensible agents that can leverage both custom business logic and external tool providers, enabling rich, multi-source tool invocation within a single conversational agent.
