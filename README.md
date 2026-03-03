# Basic Chat Loop Example

The `BasicChatLoop.cs` file demonstrates a foundational implementation of an interactive chat application using the Microsoft Agents AI Framework with OpenAI's GPT models. This example showcases how to create a stateful conversation loop where users can input messages through the console, and the AI agent responds in real-time using streaming responses. The implementation leverages the Agent Framework's `ChatClientAgent` to maintain session state across multiple turns of conversation, providing a seamless chat experience.

# Tools Example

The `Tools.cs` file illustrates advanced agent capabilities by combining internal tools with external MCP (Model Context Protocol) tools in a single agent. This example demonstrates how to create custom internal tools using `AIFunctionFactory`, integrate dependency injection into tool methods with different service lifetimes (singleton, scoped, and transient), and seamlessly merge tools from an MCP server with locally defined tools. The implementation shows a practical approach to building extensible agents that can leverage both custom business logic and external tool providers, enabling rich, multi-source tool invocation within a single conversational agent.

# RAG Chat Agent Example

The `RagChatAgent.cs` file demonstrates a Retrieval-Augmented Generation (RAG) pattern using the Microsoft Agents AI Framework combined with Entity Framework Core and SQL Server vector search. This example showcases a software architecture advisory agent that retrieves governance-approved architecture documentation using semantic similarity, generating embeddings with OpenAI's `text-embedding-3-small` model and querying a vector-indexed database using cosine distance. The agent is equipped with two tools: one that fetches relevant documentation articles to ground its responses, and another that produces a structured deployment quest when the user is ready to act on the recommended architecture.

# AI Context Provider Example

The `AiContextProvider.cs` file illustrates how to use the `AIContextProvider` abstraction to intercept and enrich every LLM call in a structured and reusable way. This example demonstrates the full provider lifecycle — `ProvideAIContextAsync` runs before the LLM call to inject dynamic instructions or tools, while `StoreAIContextAsync` runs after to process the result, in this case extracting new facts about the user from their messages using a dedicated lightweight memory-extractor agent. The implementation shows a practical pattern for building agents with persistent, automatically-updated memory that evolves across conversation turns without polluting the core agent logic.
